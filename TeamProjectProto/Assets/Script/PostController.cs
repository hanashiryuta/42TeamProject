//
//作成日時：4月18日
//中心物体(ポスト)の処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

//ポスト状態
public enum PostState
{
    ENTRY,//入場
    STAY,//待機
    AIRSPAWN,//Air生成
    SPIN,//回転
    ANGLESET,//方向指定
    FLY,//飛翔
    ITEM_UI,//アイテム生成
    EXIT,//退場
}

public class PostController : MonoBehaviour {

	[HideInInspector]
	public float blastCount = 0;//内容物総数
    
	[HideInInspector]
	public bool isRespawn = false;//中心物体が移動するかどうか
	[HideInInspector]
	public float respawnCount = 0;//中心物体が移動するまでのカウント
    [HideInInspector]
    public SphereCollider bc;//あたり判定をなくす
    
    public float inflateTime = 0.05f;//ポストから風船に内容物を膨らませるまでの時間
    public GameObject originMakar;
    bool isMakar;//マーカーがあるかどうか

    public GameObject air;//風船に移るオブジェクト

    public bool inflateObj = true;//風船に移るオブジェクトを作るかどうか：true=作る、false=作らない

    public GameObject origin_Pig_ToCoin_Particle;//貯金時パーティクル
    GameObject pig_ToCoin_Particle;
    
    GameObject timeController;//時間管理オブジェクト

    [HideInInspector]
    public PostState postState = PostState.ENTRY;//ポスト状態

    [HideInInspector]
    public GameObject player;//プレイヤーネーム

    [HideInInspector]
    public GameObject postPoint;//ポスト生成位置
    public GameObject effect;//UIコイン

    float coinCount;//コイン生成数

    float stayTime = 0.0f;//各処理待機時間

    float flyAngle = 405;//回転角度

    public GameObject origin_Post_Fly_Particle;//飛翔パーティクル
    GameObject post_Fly_Particle;
    float particleTime = 0.1f;//パーティクル生成間隔

    //float Xangle = 0;

    Animator postAnim;
    float animSpeed = 1.0f;

    AudioSource postAudio;//コイン加算時のSE入り
    FinishCall finishCall;//終了合図Script

    List<PlayerMove> playerHitList;//当たっているプレイヤーリスト

    GameObject[] postTargetList;//目指すUIの位置リスト
    Vector3 target;//目指すUIの位置

    Dictionary<string, Vector3> targetDic;//目指す位置ディクショナリ

    // Use this for initialization
    void Start () {
		//初期化処理
		blastCount = 0;
        bc = GetComponent<SphereCollider>();
        bc.enabled = false;
        postAnim = GetComponentInChildren<Animator>();
        
        timeController = GameObject.Find("TimeController");

        postAudio = GetComponent<AudioSource>();
        // 終了合図
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();

        playerHitList = new List<PlayerMove>();
        //ターゲット取得
        postTargetList = GameObject.FindGameObjectsWithTag("PostPosition");
        //ターゲット管理ディクショナリ設定
        targetDic = new Dictionary<string, Vector3>()
        {
            { postTargetList[0].name, Camera.main.ScreenToWorldPoint(postTargetList[0].transform.position)},
            { postTargetList[1].name, Camera.main.ScreenToWorldPoint(postTargetList[1].transform.position)},
            { postTargetList[2].name, Camera.main.ScreenToWorldPoint(postTargetList[2].transform.position)},
            { postTargetList[3].name, Camera.main.ScreenToWorldPoint(postTargetList[3].transform.position)},
        };
    }

    // Update is called once per frame
    void Update () {
        //ポスト状態により処理を変える
        switch (postState)
        {
            case PostState.ENTRY://入場
                Entry();
                break;
            case PostState.STAY://待機
                Stay();
                break;
            case PostState.AIRSPAWN://Air生成
                AirSpawn();
                break;
            case PostState.SPIN://回転
                Spin(1.5f);
                break;
            case PostState.ANGLESET://方向指定
                AngleSet(0.2f);
                break;
            case PostState.FLY://飛翔  
                Fly(2.0f);
                break;
            case PostState.ITEM_UI://アイテム生成
                Item_UI(0.1f);
                break;
            case PostState.EXIT://退場
                Exit(1.0f);
                break;
        }
    }

    /// <summary>
    /// アイテムを入れた際のパーティクル生成メソッド
    /// </summary>
    public void Pig_ToCoin_Particle()
    {
        if(pig_ToCoin_Particle == null)
        {
            //なければ生成
            pig_ToCoin_Particle = Instantiate(origin_Pig_ToCoin_Particle, transform);
        }
    }

    /// <summary>
    ///ポストが飛んでいるときのパーティクル生成メソッド 
    /// </summary>
    void Post_Fly_Particle()
    {
        if (post_Fly_Particle == null)
        {
            post_Fly_Particle = Instantiate(origin_Post_Fly_Particle, transform.position, Quaternion.identity,transform);
        }
    }

    /// <summary>
    /// 入場処理
    /// </summary>
    void Entry()
    {
        if (!isMakar)
        {
            //マーカーがなければ生成
            Instantiate(originMakar, gameObject.transform.parent.transform.position + new Vector3(0, 19.5f, 0), Quaternion.identity);//ポストが出現したらマーカーを出す
            isMakar = true;
        }
        //入場Vector
        Vector3 posEntry = (transform.parent.position + new Vector3(0, 0.5f, 0)) - transform.position;
        //入場
        transform.position += posEntry * Time.deltaTime * 3.5f;
        if (Mathf.Abs(posEntry.y) <= 0.1f)
        {
            AnimSpeedSet(0);
            bc.enabled = true;
            postState = PostState.STAY;//状態遷移
        }
    }

    /// <summary>
    /// 待機処理
    /// </summary>
    void Stay()
    {
        if(finishCall.IsShowing)
        {
            bc.enabled = false;
            return;
        }

        if (playerHitList.Count >= 1)
        {
            PlayerHit();
        }
    }

    /// <summary>
    /// Air生成処理
    /// </summary>
    void AirSpawn()
    {
        //Balloonがステージ上にない場合、生成しない
        var balloon = GameObject.FindGameObjectWithTag("Balloon");
        if (balloon != null)
        {
            inflateTime -= Time.deltaTime;
            if (inflateTime < 0)
            {
                //一定間隔で出す
                if (inflateObj && blastCount > 0)
                {
                    //風船膨らませるオブジェクト生成
                    Instantiate(air, gameObject.transform.position + Vector3.up, Quaternion.identity);
                }
                blastCount--;//内容物の総数を減らす
                inflateTime = 0.05f;
                //ポイント分生成したらポストを移動させる＆パーティクルが消えたら
                if (blastCount <= 0 && pig_ToCoin_Particle == null)
                {
                    blastCount = 0;
                    AnimSpeedSet(animSpeed);
                    postState = PostState.SPIN;
                }
            }
        }
        else
        {
            postState = PostState.SPIN;
        }
    }

    /// <summary>
    /// 回転処理
    /// </summary>
    /// <param name="spinTime"></param>
    void Spin(float spinTime)
    {
        Post_Fly_Particle();
        if (stayTime <= 0.0f)
        {
            //回転角度遷移
            DOTween.To(
                () => flyAngle,
                (x) => flyAngle = x,
                0,
                spinTime).SetEase(Ease.InSine);
            //DOTween.To(
            //    () => Xangle,
            //    (x) => Xangle = x,
            //    -80,
            //    1.5f).SetEase(Ease.InSine);
        }
        //オブジェクト回転
        transform.rotation = Quaternion.Euler(0, 270 - flyAngle, 0);
        //移動半径設定
        float radius = (360 - flyAngle) / 360 * 3;
        //位置回転角度設定
        float angle = (flyAngle - 90) * Mathf.PI / 180;
        //位置移動（円を描くように）
        transform.position = transform.parent.transform.position + new Vector3(0, 0.5f, 0) + new Vector3(radius * Mathf.Cos(angle), radius, radius * Mathf.Sin(angle));
        stayTime += Time.deltaTime;
        if (stayTime >= spinTime)
        {
            stayTime = 0.0f;
            postState = PostState.ANGLESET;//状態遷移
        }
    }

    /// <summary>
    /// 方向設定処理
    /// </summary>
    /// <param name="setTime"></param>
    void AngleSet(float setTime)
    {
        if (stayTime <= 0.0f)
        {
            //方向設定
            Vector3 forward = (target - transform.position).normalized;
            //徐々にその方向を向く
            DOTween.To(
                () => transform.forward.x,
                (x) => transform.forward = new Vector3(x, transform.forward.y,transform.forward.z),
                forward.x,
                setTime);
            DOTween.To(
                () => transform.forward.y,
                (y) => transform.forward = new Vector3(transform.forward.x, y,transform.forward.z),
                forward.y,
                setTime);
            DOTween.To(
                () => transform.forward.z,
                (z) => transform.forward = new Vector3(transform.forward.x, transform.forward.y, z),
                forward.z,
                setTime);
        }
        stayTime += Time.deltaTime;
        if (stayTime >= setTime)
        {
            //ロスタイムなら飛ぶときに生成可能にする
            if (timeController.GetComponent<TimeController>().timeState == TimeState.LOSSTIME)
                postPoint.GetComponent<PostSet>().isRespawn = false;
            stayTime = 0.0f;
            postState = PostState.FLY;//状態遷移
        }
    }

    /// <summary>
    /// 飛翔処理
    /// </summary>
    /// <param name="flyTime"></param>
    void Fly(float flyTime)
    {
        if (stayTime <= 0.0f)
        {
            //移動処理
            transform.DOMove(target, flyTime).SetEase(Ease.OutCubic);
        }
        stayTime += Time.deltaTime;
        if (stayTime>= flyTime)
        {
            //真横向く
            if (player.name.Contains("1") || player.name.Contains("2"))
                transform.DORotate(new Vector3(0, -90, -45), 1.0f).SetEase(Ease.OutQuint);
            else
                transform.DORotate(new Vector3(0, 90, 45), 1.0f).SetEase(Ease.OutQuint);

            stayTime = 0.0f;
            AnimSpeedSet(0);
            postState = PostState.ITEM_UI;//状態遷移
        }
    }

    /// <summary>
    /// アイテム生成処理
    /// </summary>
    /// <param name="itemTime"></param>
    void Item_UI(float itemTime)
    {
        stayTime += Time.deltaTime;
        //一定間隔で生成
        if (stayTime >= itemTime)
        {
            effect.GetComponent<ScoreEffect>().playerName = player.name; //プレイヤーの名前を代入
                                                                         //エフェクトを生成
            Instantiate(effect, RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position), Quaternion.identity, GameObject.Find("PlayerScoreUI").transform);

            //トータル表示を増やす
            player.GetComponent<PlayerMove>().totalItemCount_For_Text++;
            //コインSE
            postAudio.Play();
            stayTime = 0.0f;
        }
        //設定数以上生成したら
        //if (coinCount <= 0)
        if(player.GetComponent<PlayerMove>().totalItemCount_For_Text >= player.GetComponent<PlayerMove>().totalItemCount)
        {
            //トータル表示がトータルを超えないように
            player.GetComponent<PlayerMove>().totalItemCount_For_Text = player.GetComponent<PlayerMove>().totalItemCount;
            stayTime = 0.0f;
            AnimSpeedSet(animSpeed);
            postState = PostState.EXIT;//状態遷移
        }
    }

    /// <summary>
    /// 退場処理
    /// </summary>
    /// <param name="exitTime"></param>
    void Exit(float exitTime)
    {
        if (stayTime <= 0.0f)
        {
            //移動処理
            if (player.name.Contains("1") || player.name.Contains("2"))
                transform.DOMoveX(Camera.main.ScreenToWorldPoint(new Vector3(0-50,0)).x, exitTime);
            else
                transform.DOMoveX(Camera.main.ScreenToWorldPoint(new Vector3(1280+50, 0)).x, exitTime);
        }
        stayTime += Time.deltaTime;
        if (stayTime >= exitTime)
        {
            //リスポーンしてない状態に
            if(postPoint.GetComponent<PostSet>().isRespawn)
            postPoint.GetComponent<PostSet>().isRespawn = false;

            postPoint.GetComponent<PostSet>().isPost = false;
            //削除
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// アニメスピード変更
    /// </summary>
    /// <param name="animspeed"></param>
    void AnimSpeedSet(float animspeed)
    {
        postAnim.speed = animspeed;
    }

    /// <summary>
    /// プレイヤーと当たった際の処理
    /// </summary>
    void PlayerHit()
    {
        //当たっているプレイヤーリストで判定
        foreach(var player in playerHitList)
        {
            //アイテムを持っていたら
            if (player.holdItemCount >= 1)
            {
                //プレイヤーのポスト当たった処理
                player.HitPost(gameObject);
                //あたり判定消す
                bc.enabled = false;
                //パーティクル出す
                Pig_ToCoin_Particle();
                //ターゲット設定
                target = targetDic[player.name + "PostPosition"];
                //状態遷移
                postState = PostState.AIRSPAWN;
                break;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && col.GetComponent<PlayerMove>().balloon == null) 
        {
            //当たったプレイヤーリスト追加
            playerHitList.Add(col.gameObject.GetComponent<PlayerMove>());
        }
    }
    void OnTriggerExit(Collider col)
    {
        if (col.gameObject.tag == "Player" && col.GetComponent<PlayerMove>().balloon == null)
        {
            //離れたプレイヤーリスト除外
            playerHitList.Remove(col.gameObject.GetComponent<PlayerMove>());
        }
    }
}
