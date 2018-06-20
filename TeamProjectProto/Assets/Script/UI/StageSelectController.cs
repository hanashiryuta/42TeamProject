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
using DG.Tweening;

public class StageSelectController : MonoBehaviour
{
    [SerializeField]
    GameObject[] stageList;
    //Dictionary<string, GameObject> stageDictionary = new Dictionary<string, GameObject>();
    GameObject stage;//今表示しているステージ
    int nowStageIndex = 0;//今表示しているステージのインデックス

    ConnectedPlayerStatus connectedPlayerStatus;//プレイヤーステータス(選択したステージをここに渡す)

    //シーン移転関連
    GameLoad gameload;
    [SerializeField]
    GameObject fadePanel;
    FadeController fadeController;
    bool isFaded = false;
    bool isSceneChage = false;

    [SerializeField]
    Button leftBtn;
    [SerializeField]
    Button rightBtn;

    float cnt = 0;
    public float delayTime = 0.5f;//長押しの時の遅延
    public bool isDelay = false;

    PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;
    float moveX = 0;
    
    [SerializeField]
    GameObject stagePointsSet;//ポイント生成場所
    [SerializeField]
    GameObject stagePoint;//プレハブ
    List<GameObject> stagePointsList;
    public float pointsOffset = 40f;
    float firstPointX = 0;

    // Use this for initialization
    void Start ()
    {
        if (connectedPlayerStatus == null)
        {
            // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }

        //for (int i = 0;i < stageList.Length; i++)
        //{
        //    stageDictionary.Add(stageList[i].name, stageList[i]);
        //}

        //一つ目のステージを出す
        stage = Instantiate(stageList[0]);

        gameload = this.GetComponent<GameLoad>();
        fadeController = fadePanel.GetComponent<FadeController>();

        SetStagePoints();
    }

    /// <summary>
    /// ステージのポイント生成
    /// </summary>
    void SetStagePoints()
    {
        stagePointsList = new List<GameObject>();

        for(int i = 0; i < stageList.Length; i++)
        {
            if(i == 0)//一回目だけ
            {
                firstPointX = (stageList.Length / 2) * (-pointsOffset);//最初の位置を設定
                if (stageList.Length % 2 == 0)//偶数だったら
                {
                    firstPointX += pointsOffset / 2;//半分ずらす
                }
            }
            //ステージのポイント生成
            stagePointsList.Add(Instantiate(stagePoint, stagePointsSet.transform));
            stagePointsList[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(firstPointX + i * pointsOffset, 0, 0);
        }
        //セットの大きさ
        Vector2 setSize = stagePointsSet.transform.GetComponent<RectTransform>().sizeDelta;
        stagePointsSet.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(firstPointX), setSize.y);
        //最初表示するポイント
        PointToSelected(nowStageIndex);
    }

    // Update is called once per frame
    void Update ()
    {
        SetControllablePlayer();

        //フェードイン中か
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }
        else
        {
            //XInput
            currentState = GamePad.GetState(playerIndex);
            moveX = currentState.ThumbSticks.Left.X;

            if (isSceneChage)
            {
                SceneLoad();
            }
        }

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
    void ShowSeletedStage(float rotate)
    {
        if(stage != null)
        {
            Destroy(stage);
        }

        stage = Instantiate(stageList[nowStageIndex], new Vector3(0, 0.5f, 0), Quaternion.Euler(0, rotate, 0));
    }

    /// <summary>
    /// 左右ボタンの選択状態表示
    /// </summary>
    void ShowBtnSelected()
    {
        if (nowStageIndex <= 0)
        {
            leftBtn.gameObject.SetActive(false);
        }
        else if (nowStageIndex >= stageList.Length - 1)
        {
            rightBtn.gameObject.SetActive(false);
        }
        else
        {
            leftBtn.gameObject.SetActive(true);
            rightBtn.gameObject.SetActive(true);
        }

        if (moveX >= 0.5f)
        {
            rightBtn.Select();
        }
        if(moveX <= -0.5f)
        {
            leftBtn.Select();

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
                rightBtn.onClick.Invoke();
                DOTween.Punch(() => rightBtn.gameObject.transform.position,
                                x => rightBtn.gameObject.transform.position = x,
                                new Vector3(5, 0, 0),
                                delayTime,
                                5);
                isDelay = true;
            }
            else
            {
                DelayTimeCountDown();
            }
        }

        //左
        if (moveX < -0.8f)
        {
            if (!isDelay)
            {
                cnt = delayTime;
                leftBtn.onClick.Invoke();
                DOTween.Punch(() => leftBtn.gameObject.transform.position,
                                x => leftBtn.gameObject.transform.position = x,
                                new Vector3(-5, 0, 0),
                                delayTime,
                                5);
                isDelay = true;
            }
            else
            {
                DelayTimeCountDown();
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
            PointToDefalt(nowStageIndex);

            float rotate = stage.transform.eulerAngles.y;
            nowStageIndex++;
            ShowSeletedStage(rotate);

            PointToSelected(nowStageIndex);
        }
    }

    /// <summary>
    /// 左ボタン
    /// </summary>
    public void L_Btn()
    {
        if(nowStageIndex != 0)
        {
            PointToDefalt(nowStageIndex);

            float rotate = stage.transform.eulerAngles.y;
            nowStageIndex--;
            ShowSeletedStage(rotate);

            PointToSelected(nowStageIndex);
        }
    }

    /// <summary>
    /// ポイントの外見を元に
    /// </summary>
    /// <param name="index"></param>
    void PointToDefalt(int index)
    {
        //色
        stagePointsList[nowStageIndex].transform.GetComponent<Image>().color = Color.white;
        //大きさ
        stagePointsList[nowStageIndex].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
    }
    /// <summary>
    /// ポイントの外見を選択状態に
    /// </summary>
    /// <param name="index"></param>
    void PointToSelected(int index)
    {
        //色
        stagePointsList[nowStageIndex].transform.GetComponent<Image>().color = Color.yellow;
        //大きさ
        stagePointsList[nowStageIndex].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
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
        DOTween.KillAll();
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

    /// <summary>
    /// 遅延カウントダウン
    /// </summary>
    void DelayTimeCountDown()
    {
        cnt -= Time.deltaTime;
        if (cnt <= 0)
        {
            isDelay = false;
        }
    }
}
