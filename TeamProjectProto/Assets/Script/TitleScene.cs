/*
 * 作成日：180604
 * タイトルシーンコントローラー
 * 作成者：阿部→何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class TitleScene : MonoBehaviour
{
    GameLoad gameload;//ゲームロード

    [SerializeField]
    GameObject fadePanel;//フェードパネル
    FadeController fadeController;//フェードコントローラー
    bool isSceneChange = false;//シーン転換するか？
    bool isFadeOuted = false;//フェードアウトしたか？

    //ボタン
    [SerializeField]
    Button gameStartBtn, creditBtn, gameExitBtn;//ボタンたち
    List<Button> titleBtnList = new List<Button>();//ボタンリスト
    Button nowSelectedBtn;//今選択しているボタン
    int nowSelectedBtnIndex = 0;

    //XInput
    PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;
    float moveY = 0;

    float cnt = 0;
    public float delayTime = 0.5f;//長押しの時の遅延
    public bool isDelay = false;

    // Use this for initialization
    void Start()
    {
        gameload = this.GetComponent<GameLoad>();
        fadeController = fadePanel.GetComponent<FadeController>();

        //ボタン格納
        titleBtnList.Add(gameStartBtn);
        titleBtnList.Add(creditBtn);
        titleBtnList.Add(gameExitBtn);
        //最初にゲームスタートを選択状態に
        titleBtnList[nowSelectedBtnIndex].Select();
        nowSelectedBtn = titleBtnList[nowSelectedBtnIndex];
        //現在接続しているプレイヤーの中で番号が一番小さいやつを選択プレイヤーにする
        SetControllablePlayer();
    }

    // Update is called once per frame
    void Update()
    {
        SetControllablePlayer();

        //フェードイン中か
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }
        else
        {
            //Get XInput
            currentState = GamePad.GetState(playerIndex);
            moveY = currentState.ThumbSticks.Left.Y;
            TitleXInput();

            if (isSceneChange)
            {
                fadeController.FadeOut();
            }
        }

        //NextSceneLoad
        if (fadeController.IsFadeOutFinish && !isFadeOuted)
        {
            gameload.LoadingStartWithOBJ();
            isFadeOuted = true;
        }

        previousState = currentState;
    }

    /// <summary>
    /// 操作可能なプレイヤーを選択(番号が一番小さい人、例１P)
    /// /// </summary>
    void SetControllablePlayer()
    {
        if (GamePad.GetState(PlayerIndex.One).IsConnected)
        {
            playerIndex = PlayerIndex.One;
        }
        else if (GamePad.GetState(PlayerIndex.Two).IsConnected)
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
    /// 入力
    /// </summary>
    void TitleXInput()
    {
        //up
        if (moveY >= 0.8f && nowSelectedBtn != gameStartBtn)
        {
            if (!isDelay)
            {
                cnt = delayTime;
                ChooseNextBtn("up");
                isDelay = true;
            }
            else
            {
                DelayTimeCountDown();
            }
        }

        //down
        if (moveY <= -0.8f && nowSelectedBtn != gameExitBtn)
        {
            if (!isDelay)
            {
                cnt = delayTime;
                ChooseNextBtn("down");
                isDelay = true;
            }
            else
            {
                DelayTimeCountDown();
            }
        }

        //A
        if (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed)
        {
            BtnPushed(nowSelectedBtn);
        }

        //遅延初期化
        if (moveY > -0.05f &&
            moveY < 0.05f)
        {
            cnt = delayTime;
            isDelay = false;
        }
    }

    /// <summary>
    /// 次のボタンを選択
    /// </summary>
    /// <param name="indexDirection">次のボタンの位置</param>
    void ChooseNextBtn(string direction)
    {
        switch (direction)
        {
            case "up":
                nowSelectedBtnIndex--;
                titleBtnList[nowSelectedBtnIndex].Select();
                nowSelectedBtn = titleBtnList[nowSelectedBtnIndex];
                break;
            case "down":
                nowSelectedBtnIndex++;
                titleBtnList[nowSelectedBtnIndex].Select();
                nowSelectedBtn = titleBtnList[nowSelectedBtnIndex];
                break;
        }
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

    /// <summary>
    /// 選択しているボタンのonClickイベントを呼ぶ
    /// </summary>
    /// <param name="btn"></param>
    void BtnPushed(Button btn)
    {
        btn.onClick.Invoke();
    }

    /// <summary>
    /// ゲームスタート
    /// </summary>
    public void GameStart()
    {
        isSceneChange = true;
    }

    /// <summary>
    /// クレジット
    /// </summary>
    public void GameCredit()
    {

    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameExit()
    {
        Application.Quit();
    }
}
