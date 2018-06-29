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
    ButtonState btnState = ButtonState.None;

    //XInput
    PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;
    float moveY = 0;

    float cnt = 0;
    public float delayTime = 0.5f;//長押しの時の遅延
    public bool isDelay = false;

    //SE
    SEController se;

    //Canvas
    [SerializeField]
    CanvasGroup titleCanvas, creditCanvas;//タイトルキャンバス、クレジットキャンバス

    /// <summary>
    /// 選択されたボタン
    /// </summary>
    enum ButtonState
    {
        None,
        Start,
        Creadit,
        Exit
    }

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

        //SE
        se = transform.GetComponent<SEController>();
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

            //選択されたボタンの沿ってメソッド実行
            CheckBtnState();

            if (isSceneChange)
            {
                fadeController.FadeOut();
            }
        }

        previousState = currentState;
    }

    /// <summary>
    /// 選択されたボタンの沿ってメソッド実行
    /// </summary>
    void CheckBtnState()
    {
        switch (btnState)
        {
            case ButtonState.None:
                //カーソル移動
                moveY = currentState.ThumbSticks.Left.Y;
                TitleXInput();
                break;
            case ButtonState.Start:
                //NextSceneLoad
                if (fadeController.IsFadeOutFinish && !isFadeOuted)
                {
                    gameload.LoadingStartWithOBJ();
                    isFadeOuted = true;
                }
                break;
            case ButtonState.Creadit:
                CreditXInput();//Bボタン押したらtitleCanvas戻す
                break;
            case ButtonState.Exit:
                //ExitFade
                if (fadeController.IsFadeOutFinish && !isFadeOuted)
                {
                    Application.Quit();
                }
                break;

        }
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
    /// タイトル入力
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
                se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
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
                se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
                isDelay = true;
            }
            else
            {
                DelayTimeCountDown();
            }
        }
        //A
        if (previousState.Buttons.A == XInputDotNetPure.ButtonState.Released &&
            currentState.Buttons.A == XInputDotNetPure.ButtonState.Pressed)
        {
            se.PlaySystemSE((int)SEController.SystemSE.OK);
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
    /// クレジット入力
    /// </summary>
    /// <returns></returns>
    void CreditXInput()
    {
        //B
        if (previousState.Buttons.B == XInputDotNetPure.ButtonState.Released &&
            currentState.Buttons.B == XInputDotNetPure.ButtonState.Pressed)
        {
            creditCanvas.alpha = 0;
            titleCanvas.alpha = 1;
            se.PlaySystemSE((int)SEController.SystemSE.Cancel);
            btnState = ButtonState.None;
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
        btnState = ButtonState.Start;
    }

    /// <summary>
    /// クレジット
    /// </summary>
    public void GameCredit()
    {
        titleCanvas.alpha = 0;
        creditCanvas.alpha = 1;
        btnState = ButtonState.Creadit;
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameExit()
    {
        isSceneChange = true;
        btnState = ButtonState.Exit;
    }
}
