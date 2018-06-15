/*
 * 作成日：180612
 * ステージセレクトコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XInputDotNetPure;

public class StageSelectController : MonoBehaviour
{
    [SerializeField]
    GameObject[] stageList;
    Dictionary<string, GameObject> stageDictionary = new Dictionary<string, GameObject>();
    GameObject stage;
    int nowStageIndex = 0;

    ConnectedPlayerStatus connectedPlayerStatus;//プレイヤーステータス(選択したステージをここに渡す)

    //シーン移転関連
    GameLoad gameload;
    [SerializeField]
    GameObject fadePanel;
    FadeController fadeController;
    bool isFaded = false;
    bool isSceneChage = false;

    [SerializeField]
    Button leftB;
    [SerializeField]
    Button rightB;

    float cnt = 0;
    float delayTime = 1f;
    public bool isDelay = false;

    PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;
    float moveX = 0;

    // Use this for initialization
    void Start ()
    {
        if (connectedPlayerStatus == null)
        {
            // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }

        for (int i = 0;i < stageList.Length; i++)
        {
            stageDictionary.Add(stageList[i].name, stageList[i]);
        }

        //一つ目のステージを出す
        stage = Instantiate(stageList[0]);

        gameload = this.GetComponent<GameLoad>();
        fadeController = fadePanel.GetComponent<FadeController>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }

        SetControllablePlayer();

        currentState = GamePad.GetState(playerIndex);

        moveX = currentState.ThumbSticks.Left.X;

        //ステージを選択（Aボタン）
        if (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed)
        {
            Start_Btn();
        }
        //左右ボタンの選択状態表示
        ShowBtnSelected();
        //表示しているステージを変更
        ShowStageChage();

        if (isSceneChage)
        {
            SceneLoad();
        }

        previousState = currentState;
    }

    /// <summary>
    /// 操作可能なプレイヤーを選択
    /// /// </summary>
    void SetControllablePlayer()
    {
        if (GamePad.GetState(PlayerIndex.One).IsConnected)
        {
            playerIndex = PlayerIndex.One;
        }
        else if(GamePad.GetState(PlayerIndex.Two).IsConnected)
        {
            playerIndex = PlayerIndex.Two;
        }
        else if (GamePad.GetState(PlayerIndex.Three).IsConnected)
        {
            playerIndex = PlayerIndex.Three;
        }
        else if (GamePad.GetState(PlayerIndex.Four).IsConnected)
        {
            playerIndex = PlayerIndex.Four;
        }
    }

    /// <summary>
    /// 選択したステージを表示
    /// </summary>
    void SelectStage(float rotate)
    {
        if(stage != null)
        {
            Destroy(stage);
        }

        stage = Instantiate(stageList[nowStageIndex], Vector3.zero, Quaternion.Euler(0, rotate, 0));
    }

    /// <summary>
    /// 左右ボタンの選択状態表示
    /// </summary>
    void ShowBtnSelected()
    {
        if(moveX >= 0.5f)
        {
            rightB.Select();
        }
        if(moveX <= -0.5f)
        {
            leftB.Select();
        }
        if (moveX > -0.05f ||
            moveX < 0.05f)
        {
            transform.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// 表示しているステージを変更
    /// </summary>
    void ShowStageChage()
    {
        //右
        if (moveX > 0.8f)
        {
            if (!isDelay)
            {
                cnt = delayTime;
                rightB.onClick.Invoke();

                isDelay = true;
            }
            else
            {
                cnt -= Time.deltaTime;
                if (cnt <= 0)
                {
                    isDelay = false;
                }
            }
        }

        //左
        if (moveX < -0.8f)
        {
            if (!isDelay)
            {
                cnt = delayTime;
                leftB.onClick.Invoke();

                isDelay = true;
            }
            else
            {
                cnt -= Time.deltaTime;
                if (cnt <= 0)
                {
                    isDelay = false;
                }
            }
        }

        //遅延初期化
        if (moveX > -0.05f &&
            moveX < 0.05f)
        {
            cnt = delayTime;
            isDelay = false;
        }
    }

    /// <summary>
    /// 右ボタン
    /// </summary>
    public void R_Btn()
    {
        if (nowStageIndex != stageList.Length - 1)
        {
            float rotate = stage.transform.eulerAngles.y;
            nowStageIndex++;
            SelectStage(rotate);
        }
    }

    /// <summary>
    /// 左ボタン
    /// </summary>
    public void L_Btn()
    {
        if(nowStageIndex != 0)
        {
            float rotate = stage.transform.eulerAngles.y;
            nowStageIndex--;
            SelectStage(rotate);
        }
    }

    /// <summary>
    /// 今のステージを選択し、格納
    /// </summary>
    void SetStage()
    {
        string stageName = stageList[nowStageIndex].name;
        connectedPlayerStatus.StageName = stageName;
    }

    void SceneLoad()
    {
        fadeController.FadeOut();
        if (fadeController.IsFadeOutFinish && !isFaded)
        {
            gameload.LoadingStartWithOBJ();
            isFaded = true;
        }
    }

    /// <summary>
    /// スタートボタン
    /// </summary>
    public void Start_Btn()
    {
        SetStage();
        isSceneChage = true;
    }
}
