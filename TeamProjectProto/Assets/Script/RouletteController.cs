//
//作成日時：6月12日
//ルーレット管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;
using DG.Tweening;

//ルーレット状態
enum RouletteState
{
    ENTRY,//ルーレット出現
    START,//ルーレット開始
    REEL1,//1つ目のリール
    REEL2,//2つ目のリール
    REEL3,//3つ目のリール
    END,//ルーレット終了
    EXIT,//ルーレット退場
}

public class RouletteController : MonoBehaviour {

    [HideInInspector]
    public GameObject jugglerPlayer;//回すプレイヤー
    [HideInInspector]
    public GameObject balloonMaster;//バルーン管理クラス

    RouletteState rouletteState = RouletteState.ENTRY;//ルーレット状態

    [HideInInspector]
    public PlayerIndex playerIndex;//回すプレイヤー番号
    GamePadState previousState;//1フレーム前のボタン状態
    GamePadState currentState;//現在のボタン状態

    public GameObject firstReelObj;//1つ目のリールオブジェクト
    public GameObject secondReelObj;//2つ目のリールオブジェクト
    public GameObject thirdReelObj;//3つ目のリールオブジェクト

    ReelSpin firstReel;//1つ目のリール
    ReelSpin secondReel;//2つ目のリール
    ReelSpin thirdReel;//3つ目のリール

    [HideInInspector]
    public List<GameObject> playerList;//プレイヤーリスト

    public GameObject lever;//ルーレットレバー
    public GameObject leverPlayer;//回す人表示
    public List<Sprite> jugglerSpriteList;//回す人画像リスト
    public GameObject jugglerA;//レバーのAボタン 

    public List<GameObject> reelStopButtons;//各リールを止めるボタン
    public List<Sprite> reelStopButtonsState;//リール止めるボタン画像

    RectTransform rectTransform;//レクトトランスフォーム

    float waitTime = 1;//各待ち時間
    SelectScript selectScript;

    // Use this for initialization
    void Start () {
        ////ゲームを一時的に止める
        selectScript = GameObject.Find("PauseCtrl").GetComponent<SelectScript>();
        selectScript.pauseState = PauseState.OBJECTSET;
        selectScript.isRoulette = true;
        //時間を止める
        GameObject.Find("TimeController").GetComponent<TimeController>().isPause = true;
        //各リール取得
        firstReel = firstReelObj.GetComponent<ReelSpin>();
        secondReel = secondReelObj.GetComponent<ReelSpin>();
        thirdReel = thirdReelObj.GetComponent<ReelSpin>();
        //爆破したプレイヤーの番号取得
        playerIndex = jugglerPlayer.GetComponent<PlayerMove>().playerIndex;
        //playerIndex = 0;
        //プレイヤーリスト生成
        playerList = new List<GameObject>();
        foreach(var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            //爆破したプレイヤーは除く
            if (player != jugglerPlayer)
                playerList.Add(player);
        }
        //回すプレイヤーの画像表示
        leverPlayer.GetComponent<Image>().sprite = jugglerSpriteList[(int)playerIndex];
        //Aボタン表示
        jugglerA.SetActive(true);
        //各リールストップボタン押してない状態の画像に設定
        foreach(var button in reelStopButtons)
        {
            button.GetComponent<Image>().sprite = reelStopButtonsState[0];
        }

        //レクト取得
        rectTransform = GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update () {
        //Canvasの一番手前に表示
        transform.SetAsLastSibling();
        //ボタン取得
        currentState = GamePad.GetState(playerIndex);

        //コントローラーがなければreturn
        if (!currentState.IsConnected)
        {
            return;
        }

        //状態により行動変化
        switch (rouletteState)
        {
            case RouletteState.ENTRY://ルーレット出現
                //中心に向けて移動
                rectTransform.DOLocalMoveY(Vector3.zero.y, 1);
                waitTime -= Time.deltaTime;
                if (waitTime < 0)
                {
                    //振動停止
                    GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
                    GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
                    GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
                    GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
                    waitTime = 1;
                    //状態変化
                    rouletteState = RouletteState.START;
                }
                break;
            case RouletteState.START://ルーレット開始
                //Aボタンが押されたら
                if (PushA())
                {
                    //各リール回転開始
                    firstReel.isSpin = true;
                    secondReel.isSpin = true;
                    thirdReel.isSpin = true;
                    //レバーを下げる
                    lever.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
                    //レバーボタン消す
                    jugglerA.SetActive(false);
                    //状態変化
                    rouletteState = RouletteState.REEL1;
                }
                break;
            case RouletteState.REEL1://1つ目のリール
                //Aボタンが押されたら
                if (PushA())
                {
                    //1つ目のリール停止
                    firstReel.SpinEnd();
                    //停止ボタン押した状態の画像に変更
                    reelStopButtons[0].GetComponent<Image>().sprite = reelStopButtonsState[1];
                    //状態変更
                    rouletteState = RouletteState.REEL2;
                }
                break;
            case RouletteState.REEL2://2つ目のリール
                //Aボタンが押されたら
                if (PushA())
                {
                    //2つ目のリール停止
                    secondReel.SpinEnd();
                    //停止ボタン押した状態の画像に変更
                    reelStopButtons[1].GetComponent<Image>().sprite = reelStopButtonsState[1];
                    //状態変更
                    rouletteState = RouletteState.REEL3;
                }
                break;
            case RouletteState.REEL3://3つ目のリール
                //Aボタンが押されたら
                if (PushA())
                {
                    //3つ目のリール停止
                    thirdReel.SpinEnd();
                    //停止ボタン押した状態の画像に変更
                    reelStopButtons[2].GetComponent<Image>().sprite = reelStopButtonsState[1];
                    //状態変更
                    rouletteState = RouletteState.END;
                }
                break;
            case RouletteState.END://ルーレット終了
                //バルーン管理クラスに各リールの情報を渡す
                balloonMaster.GetComponent<BalloonMaster>().NextBalloonSet(
                    firstReel.ReelValue<float>(),
                    secondReel.ReelValue<bool>(),
                    thirdReel.ReelValue<GameObject>());
                //各リール停止
                firstReel.isSpin = false;
                secondReel.isSpin = false;
                thirdReel.isSpin = false;

                waitTime -= Time.deltaTime;
                if (waitTime < 0)
                {
                    waitTime = 1;
                    //状態変更
                    rouletteState = RouletteState.EXIT;
                }
                break;
            case RouletteState.EXIT://ルーレット退場
                //ゲームの停止状態を解除
                selectScript.pauseState = PauseState.PAUSEEND;
                selectScript.isRoulette = false;
                //時間の停止状態解除
                GameObject.Find("TimeController").GetComponent<TimeController>().isPause = false;
                //画面外へ移動
                rectTransform.DOLocalMoveY(-660, 1);
                //画面外まで行ったら
                if (rectTransform.localPosition.y < -650)
                {
                    //ルーレット終了
                    balloonMaster.GetComponent<BalloonMaster>().isRoulette = false;
                    //デストロイ
                    Destroy(gameObject);
                }
                break;
        }

        //ボタン状態更新
        previousState = currentState;
    }

    /// <summary>
    /// Aボタン押したら
    /// </summary>
    /// <returns></returns>
    bool PushA()
    {
        //押したときだけ判定
        if (previousState.Buttons.A == ButtonState.Released && currentState.Buttons.A == ButtonState.Pressed)
            return true;

        return false;
    }
}
