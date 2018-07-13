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
public enum RouletteState
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

    [HideInInspector]
    public RouletteState rouletteState = RouletteState.ENTRY;//ルーレット状態

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

    [HideInInspector]
    public GameObject[] pList;//プレイヤーリスト
    
    public GameObject originHaveItemCount;//所持アイテム表示オブジェクト
    GameObject haveItemCount;

    SEController se;//SE

    public GameObject originRoulettePanel;//ルーレット背景フェード
    GameObject roulettePanel;

    // Use this for initialization
    void Start () {
        ////ゲームを一時的に止める
        selectScript = GameObject.Find("PauseCtrl").GetComponent<SelectScript>();
        selectScript.pauseState = PauseState.OBJECTSET;
        selectScript.isRoulette = true;
        //時間を止める
        GameObject.Find("TimeController").GetComponent<TimeController>().isPause = true;
        //爆破したプレイヤーの番号取得
        playerIndex = jugglerPlayer.GetComponent<PlayerMove>().playerIndex;
        //playerIndex = 0;
        //プレイヤーリスト生成
        playerList = new List<GameObject>();
        foreach (var player in pList)
        {
            //爆破したプレイヤーは除く
            if (player != jugglerPlayer)
                playerList.Add(player);
        }
        //各リール設定
        //各リールに対応した割合を渡す
        firstReel = firstReelObj.GetComponent<ReelSpin>();
        firstReel.reelRateCountList = balloonMaster.GetComponent<BalloonMaster>().reelRateLists[0];
        secondReel = secondReelObj.GetComponent<ReelSpin>();
        secondReel.reelRateCountList = balloonMaster.GetComponent<BalloonMaster>().reelRateLists[1];
        thirdReel = thirdReelObj.GetComponent<ReelSpin>();
        thirdReel.reelRateCountList = balloonMaster.GetComponent<BalloonMaster>().reelRateLists[2];
        //回すプレイヤーの画像表示
        leverPlayer.GetComponent<Image>().sprite = jugglerSpriteList[(int)playerIndex];
        //Aボタン表示
        jugglerA.SetActive(true);
        //各リールストップボタン押せない状態の画像に設定
        foreach(var button in reelStopButtons)
        {
            button.GetComponent<Image>().sprite = reelStopButtonsState[1];
        }

        //レクト取得
        rectTransform = GetComponent<RectTransform>();

        //SE
        se = transform.GetComponent<SEController>();

        roulettePanel = Instantiate(originRoulettePanel,GameObject.FindGameObjectWithTag("Canvas").transform);

        roulettePanel.transform.SetAsLastSibling();
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
                //所持アイテム表示オブジェクト生成
                if (haveItemCount == null)
                {
                    //SE
                    se.PlayRouletteSE((int)SEController.RouletteSE.RouletteAppear);

                    haveItemCount = Instantiate(originHaveItemCount, GameObject.FindGameObjectWithTag("Canvas").transform);
                    //初期化処理
                    haveItemCount.GetComponent<HaveItemCounts>().RouletteStart(pList, this);
                }
                //中心に向けて移動
                rectTransform.DOLocalMoveY(-15, 1);
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
                    //各リールストップボタン押せる状態の画像に設定
                    foreach (var button in reelStopButtons)
                    {
                        button.GetComponent<Image>().sprite = reelStopButtonsState[0];
                    }
                    //状態変化
                    rouletteState = RouletteState.REEL1;
                    //SE
                    se.PlayRouletteSE((int)SEController.RouletteSE.Lever);
                }
                break;
            case RouletteState.REEL1://1つ目のリール
                //Aボタンが押されたら
                if (PushA())
                {
                    //SE
                    se.PlayRouletteSE((int)SEController.RouletteSE.PressA);
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
                    //SE
                    se.PlayRouletteSE((int)SEController.RouletteSE.PressA);
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
                    //SE
                    se.PlayRouletteSE((int)SEController.RouletteSE.PressA);
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
                    //ルーレット終了
                    balloonMaster.GetComponent<BalloonMaster>().isRoulette = false;
                    //終了フェード
                    roulettePanel.GetComponent<RouletteFade>().isEnd = true;
                    //状態変更
                    rouletteState = RouletteState.EXIT;
                }
                //振動停止
                GamePad.SetVibration(balloonMaster.GetComponent<BalloonMaster>().nextPlayerIndex, 0.0f, 0.0f);
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
                    //所持アイテム表示オブジェクト削除
                    Destroy(haveItemCount);
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
