//
//作成日時：6月8日
//バルーン総合管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

    public enum NowBalloonState
{
    SAFETY,//安全
    CAUTION,//注意
    DANGER,//危険
    NONE//何もない
}

public class BalloonMaster : MonoBehaviour {

    public List<GameObject> balloonList;//バルーンリスト
    GameObject nextSpawnBalloon;//次にセルバルーン
    public GameObject NextSpawnBalloon
    {
        get { return nextSpawnBalloon; }
    }
    GameObject nowBalloon;//今で出ているバルーン
    public GameObject NowBalloon
    {
        get { return nowBalloon; }
    }

    [HideInInspector]
    public GameObject nextPlayer;//次にバルーンがつくプレイヤー
    [HideInInspector]
    public GameObject nowPlayer;//今バルーンがついているプレイヤー

    public float originBalloonRespawnTime;//バルーン出現時間インターバル
    [HideInInspector]
    public float balloonRespawnTime;

    [HideInInspector]
    public GameObject[] pList;//プレイヤーリスト

    StartCountDown startCntDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script

    public bool isCameraDistance = false;

    PortalCircle portalCircle_ToOutside;//外向け円(Effect)
    PortalCircle portalCircle_ToInside;//内向け円(Effect)
    CameraShake cameraShake;//カメラ振動(Effect)

    public GameObject origin_BalloonEffectCenter;//風船用エフェクト用中心オブジェ
    GameObject balloonEffectCenter;
    public GameObject PortalCenter
    {
        get { return balloonEffectCenter; }
    }

    NowBalloonState _nowBalloonState;//風船色状態
    public NowBalloonState NowBState
    {
        get { return _nowBalloonState; }
    }

    bool _isColorChaged = false;//色変化したか（エフェクト用）
    public bool IsColorChanged
    {
        get { return _isColorChaged; }
    }

    bool _isBlast = false;//爆発したか（エフェクト用）
    public bool IsBlast
    {
        get { return _isBlast; }
        set { _isBlast = value; }
    }
    [HideInInspector]
    public float blastCount = 0;//内容物総数

    [HideInInspector]
    public bool isRoulette;//ルーレット状態かどうか

    public GameObject originRouletteObject;//ルーレット元オブジェクト
    GameObject rouletteObject;//ルーレットオブジェクト

    TimeController timeController;//タイムコントローラー

    SEController se;//SEコントローラー

    PlayerRank playerRank;//プレイヤーランク

    [HideInInspector]
    public List<List<float>> reelRateLists;//ルーレット各リール要素割合

    [HideInInspector]
    public PlayerIndex nextPlayerIndex;//次に風船を持つプレイヤーのコントローラー

    // Use this for initialization
    void Start ()
    {   
        //プレイヤーランク取得     
        playerRank = GameObject.Find("PlayerRankController").GetComponent<PlayerRank>();
        pList = playerRank.PlayerRankArray;
        foreach(var player in pList)
        {
            //各プレイヤーに自身を設定
            player.GetComponent<PlayerMove>().balloonMaster = gameObject;
        }
        //最初のプレイヤーを設定
        nextPlayer = pList[Random.Range(0, pList.Length)];
        //バルーン設定
        nowBalloon = null;
        balloonRespawnTime = originBalloonRespawnTime + nextPlayer.GetComponent<PlayerMove>().stanTime;
        nextSpawnBalloon = balloonList[0];
        //スタートカウント、終了オブジェ取得
        startCntDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();

        balloonEffectCenter = null;

        portalCircle_ToOutside = Camera.main.GetComponents<PortalCircle>()[0];//1個目がoutside
        portalCircle_ToInside = Camera.main.GetComponents<PortalCircle>()[1];//2個目がinside

        portalCircle_ToOutside.BalloonM = this;
        portalCircle_ToInside.BalloonM = this;

        cameraShake = Camera.main.GetComponent<CameraShake>();
        cameraShake.BalloonM = this;

        //時間オブジェ取得
        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();

        se = transform.GetComponent<SEController>();
        
        //ルーレット各リール要素割合初期値設定
        reelRateLists = new List<List<float>>
        {
            new List<float> {3,4,3},
            new List<float> {3,7},
            new List<float> {5,3,2},
        };
    }

    // Update is called once per frame
    void Update ()
    {
        // カウントダウン中は何もしない
        if (startCntDown.IsCntDown || finishCall.IsCalling)
            return;

        //ルーレット呼び出し処理
        RouletteSet();

        //バルーンがなければ
        if (nowBalloon == null)
        {
            //ルーレットがあるなら何もしない
            if (rouletteObject != null)
                return;
            blastCount = 0;
            _nowBalloonState = NowBalloonState.NONE;

            //時間経過
            balloonRespawnTime -= Time.deltaTime;
            if(balloonRespawnTime <= 0)
            {
                //今風船つくプレイヤーを指定
                nowPlayer = nextPlayer;
                //バルーン生成
                nowBalloon = Instantiate(nextSpawnBalloon,nowPlayer.transform.position,Quaternion.identity);
                se.PlayBalloonSE((int)SEController.BalloonSE.Spawn);
                //自分指定
                nowBalloon.GetComponent<BalloonOrigin>().balloonMaster = this;
                //バルーンにプレイヤー指定
                nowBalloon.GetComponent<BalloonOrigin>().player = nowPlayer;
                //プレイヤーにバルーン指定
                nowPlayer.GetComponent<PlayerMove>().balloon = nowBalloon;
                //次の爆弾指定
                nextSpawnBalloon = balloonList[Random.Range(0, balloonList.Count)];
                //ダッシュ回復
                //nowPlayer.GetComponent<PlayerMove>().DashCountDown = nowPlayer.GetComponent<PlayerMove>().DashLimitTime;

                balloonRespawnTime = originBalloonRespawnTime;

                //風船の色状態
                _nowBalloonState = (NowBalloonState)(nowBalloon.GetComponent<BalloonOrigin>().BalloonState);

                if (balloonEffectCenter != null)
                {
                    Destroy(balloonEffectCenter);
                    //中心座標替わり
                    balloonEffectCenter = Instantiate(origin_BalloonEffectCenter, nowBalloon.transform.position, Quaternion.identity);
                }
                else
                {
                    //中心座標替わり
                    balloonEffectCenter = Instantiate(origin_BalloonEffectCenter, nowBalloon.transform.position, Quaternion.identity);
                }
            }

        }
        //バルーンがある時
        else
        {
            //位置合わせ
            balloonEffectCenter.transform.position = nowBalloon.transform.position;
            _isColorChaged = nowBalloon.GetComponent<BalloonOrigin>().IsColorChanged;

            _nowBalloonState = (NowBalloonState)(nowBalloon.GetComponent<BalloonOrigin>().BalloonState);
            blastCount = nowBalloon.GetComponent<BalloonOrigin>().blastCount;

            portalCircle_ToOutside.TargetCenter = balloonEffectCenter;
            portalCircle_ToInside.TargetCenter = balloonEffectCenter;
        }
    }

    /// <summary>
    /// ルーレット生成メソッド
    /// </summary>
    void RouletteSet()
    {
        if(isRoulette)
        {
            //ルーレットがないのなら
            if(rouletteObject == null)
            {
                //バルーンタイプリールの割合を時間で設定
                reelRateLists[1] = timeController.ReelRateSet();
                //プレイヤー数により割合変更
                if (pList.Length == 3)
                    reelRateLists[2] = new List<float> { 6, 4 };
                else
                    reelRateLists[2] = new List<float> { 5, 3, 2 };
                //Canvasの下にルーレット生成
                rouletteObject = Instantiate(originRouletteObject,GameObject.FindGameObjectWithTag("Canvas").transform);
                //回すプレイヤーを渡す
                rouletteObject.GetComponent<RouletteController>().jugglerPlayer = nowPlayer;
                //自分を渡す
                rouletteObject.GetComponent<RouletteController>().balloonMaster = gameObject;
                //プレイヤーリストを渡す
                rouletteObject.GetComponent<RouletteController>().pList = playerRank.PlayerRankArray;
            }
        }
    }

    /// <summary>
    /// 次回生成バルーン設定
    /// </summary>
    /// <param name="second">秒数</param>
    /// <param name="isTotal">爆発タイプ</param>
    /// <param name="player">次のプレイヤー</param>
    public void NextBalloonSet(float second,bool isTotal,GameObject player)
    {
        //トータルから減らすバルーン
        if (isTotal)
            nextSpawnBalloon = balloonList[1];
        //手持ちから減らすバルーン
        else
            nextSpawnBalloon = balloonList[0];

        //秒数設定
        nextSpawnBalloon.GetComponent<BalloonOrigin>().blastLimit = second;
        //プレイヤー設定
        nextPlayer = player;

        timeController.LossTimeStart(second + originBalloonRespawnTime + nextPlayer.GetComponent<PlayerMove>().originStanTime, this);//ロスタイム判定

        nextPlayerIndex = nextPlayer.GetComponent<PlayerMove>().playerIndex;
        GamePad.SetVibration(nextPlayerIndex, 0.0f, 1.0f);
        GameObject.Find("PostRespawnPoint").GetComponent<PostRespawn>().isBalloon = true;
    }
    
}
