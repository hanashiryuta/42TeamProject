/*
 * 作成日：180604
 * タイトルシーンコントローラー
 * 作成者：安部→何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class TitleScene : SceneController
{
    //ボタン
    [SerializeField]
    Button gameStartBtn, creditBtn, gameExitBtn;//ボタンたち
    List<Button> titleBtnList = new List<Button>();//ボタンリスト
    Button nowSelectedBtn;//今選択しているボタンOBJ
    int nowSelectedBtnIndex = 0;//今選択しているボタンのインデックス

    //SceneState
    TitleSceneState sceneState = TitleSceneState.FadeIn;//今のシーン状態

    //Canvas
    [SerializeField]
    CanvasGroup titleCanvas, creditCanvas;//タイトルキャンバス、クレジットキャンバス
  
    //BGMController
    [SerializeField]
    GameObject bgmControllerOBJ;

    // Use this for initialization
    public override void Start()
    {
        //ボタン格納
        titleBtnList.Add(gameStartBtn);
        titleBtnList.Add(creditBtn);
        titleBtnList.Add(gameExitBtn);

        //最初にゲームスタートを選択状態に
        titleBtnList[nowSelectedBtnIndex].Select();
        nowSelectedBtn = titleBtnList[nowSelectedBtnIndex];

        //BGM
        if (!BGMController.created)
        {
            Instantiate(bgmControllerOBJ).GetComponent<BGMController>();
        }

        base.Start();
    }

    /// <summary>
    /// シーンの状態に沿ってメソッド実行
    /// </summary>
    public override void CheckSceneState()
    {
        //Get XInput
        currentState = GamePad.GetState(playerIndex);

        switch (sceneState)
        {
            case TitleSceneState.FadeIn://フェードイン中
                if (fadeController.IsFadeInFinish)
                    sceneState = TitleSceneState.None;
                break;

            case TitleSceneState.None://基準状態
                moveY = currentState.ThumbSticks.Left.Y;
                TitleXInput();
                break;

            case TitleSceneState.Start://スタート
                //フェードアウト終わったら
                if (fadeController.IsFadeOutFinish)
                    //NextSceneLoad
                    gameLoad.LoadingStartWithOBJ();
                break;

            case TitleSceneState.Creadit://クレジット
                CreditXInput();//Bボタン押したらtitleCanvas戻す
                break;

            case TitleSceneState.Exit://ゲーム終了
                //ExitFade
                if (fadeController.IsFadeOutFinish)
                    Application.Quit();
                break;
        }

        previousState = currentState;
    }

    /// <summary>
    /// タイトル入力
    /// </summary>
    void TitleXInput()
    {
        //up
        if (moveY >= 0.8f && nowSelectedBtn != gameStartBtn)
        {
            if (!isInputDelay)
            {
                inputDelayCnt = inputDelayTime;
                ChooseNextBtn("up");
                se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
                isInputDelay = true;
            }
            else
            {
                InputDelayTimeCountDown();
            }
        }
        //down
        if (moveY <= -0.8f && nowSelectedBtn != gameExitBtn)
        {
            if (!isInputDelay)
            {
                inputDelayCnt = inputDelayTime;
                ChooseNextBtn("down");
                se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
                isInputDelay = true;
            }
            else
            {
                InputDelayTimeCountDown();
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
            inputDelayCnt = inputDelayTime;
            isInputDelay = false;
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
            sceneState = TitleSceneState.None;
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
        sceneState = TitleSceneState.Start;
    }

    /// <summary>
    /// クレジット
    /// </summary>
    public void GameCredit()
    {
        titleCanvas.alpha = 0;
        creditCanvas.alpha = 1;
        sceneState = TitleSceneState.Creadit;
    }

    /// <summary>
    /// ゲーム終了
    /// </summary>
    public void GameExit()
    {
        isSceneChange = true;
        sceneState = TitleSceneState.Exit;
    }
}
