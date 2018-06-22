//
//作成日時：4月18日
//中心物体(ポスト)の処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostController : MonoBehaviour {

	[HideInInspector]
	public float blastCount = 0;//内容物総数

	GameObject balloon;//爆発物

	float giveTime = 0.2f;//爆発物に内容物を渡すインターバル

	[HideInInspector]
	public bool isRespawn = false;//中心物体が移動するかどうか
	[HideInInspector]
	public float respawnCount = 0;//中心物体が移動するまでのカウント

    [HideInInspector]
	public GameObject specialWallPoint;// 特殊壁移動ポイント
	[HideInInspector]
	public GameObject player;//プレイヤー

	[HideInInspector]
	public bool activity;//PostRespawnで処理をおこなうためのbool型
    //[HideInInspector]
    //public int activeCount;//activityが切り替わるまでのアイテムの個数
    [HideInInspector]
    public MeshRenderer mesh;//ポストを透明化させるためのもの
    [HideInInspector]
    public SphereCollider bc;//あたり判定をなくす

    float interval;//表示するまでの間隔

    public float intervalLimit = 5;//再出現時間

    float limitCount;//移動するために必要なアイテムの個数
    public float inflateTime = 0.05f;//ポストから風船に内容物を膨らませるまでの時間
    public GameObject marker;

    public GameObject air;//風船に移るオブジェクト
    float airCount = 0;//風船に移るオブジェクトの数

    public bool inflateObj = true;//風船に移るオブジェクトを作るかどうか：true=作る、false=作らない
    StartCountDown startCountDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script

    public GameObject origin_Pig_ToCoin_Particle;
    GameObject pig_ToCoin_Particle;

    [HideInInspector]
    public bool isBalloon;

    bool isMove = false;//出現したときに画面外から動かすかどうか

    // Use this for initialization
    void Start () {
		//初期化処理
		blastCount = 0;
		balloon = GameObject.FindGameObjectWithTag("Balloon");//爆発物取得
		specialWallPoint = GameObject.Find("SpecialWallPoint(Clone)");//特殊壁移動ポイント取得
        activity = false;
        //activeCount = 0;
        mesh = GetComponentInChildren<MeshRenderer>();
        bc = GetComponent<SphereCollider>();
        startCountDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();
    }

    // Update is called once per frame
    void Update () {

		//5ポイント貯めたら特殊壁出して移動する
		if (respawnCount >= 1)
        {
            limitCount = respawnCount;//ポストに5ポイント以上入ったときに移動できるようにする
            respawnCount = 0;
            Debug.Log("移る");
            activity = false;
        }

		//内容物が一つでもあれば
		if(blastCount>0)
        {
            inflateTime -= Time.deltaTime;
            //5個以上あったとき
            if (limitCount >= 1)
            {
                if (inflateTime < 0)
                {
                    ////内容物の総数分風船を膨らます
                    //balloon.GetComponent<BalloonController>().BalloonBlast(gameObject);
                    if(inflateObj)
                    {
                        Instantiate(air, gameObject.transform.position + Vector3.up, Quaternion.identity);
                    }
                    airCount++;//生成されるたびに加算していく
                    blastCount--;//内容物の総数を減らす
                    inflateTime = 0.05f;
                    //ポイント分生成したらポストを移動させる＆パーティクルが消えたら
                    if (airCount >= limitCount&&pig_ToCoin_Particle == null)
                    {
                        isRespawn = true;
                        airCount = 0;
                    }
                }
            }
        }

        //activityがfalseのとき
        if (activity == false)
        {
            //MeshとColliderをfalseにする
            //Meshはパーティクルが消えたら
            if (pig_ToCoin_Particle == null)
            mesh.enabled = false;
            bc.enabled = false;
            if(!startCountDown.IsCntDown && !finishCall.IsCalling)
                interval++;
            //約5秒後に再出現させる。その際に移動させるために必要なものを初期化
            if (isBalloon)
            {
                interval = 0;
                limitCount = 0;
                mesh.enabled = true;
                //bc.enabled = true;
                activity = true;
                gameObject.GetComponentInParent<PostRespawn>().isLimitReset = true;
                Instantiate(marker, gameObject.transform.position + new Vector3(0, 19.5f, 0), Quaternion.identity);//ポストが出現したらマーカーを出す
                isBalloon = false;
                isMove = false;
            }
        }

        //acivityがtrueのとき
        else
        {

            if (!isMove)
            {
                //目標の座標点の上空に配置する
                transform.position = transform.parent.position + new Vector3(0, 30, 0);
                isMove = true;//目標の座標点に動かさせる
            }
            else
            {
                //目標の座標点までの距離を求め、一定以下になるまで近づける
                var pos = (transform.parent.position + new Vector3(0, 0.5f, 0)) - transform.position;
                if (pos.y <= 0.5f && pos.y >= -0.5f)
                {
                    //一定以下になったら、あたり判定を付けなおす
                    bc.enabled = true;
                    return;
                }
                transform.position += pos * Time.deltaTime * 3.5f;
            }
        }
    }

    /// <summary>
    /// アイテムを入れた際のパーティクル生成メソッド
    /// </summary>
    public void Pig_ToCoin_Particle()
    {
        if(pig_ToCoin_Particle == null)
        {
            pig_ToCoin_Particle = Instantiate(origin_Pig_ToCoin_Particle, transform);
        }
    }
}
