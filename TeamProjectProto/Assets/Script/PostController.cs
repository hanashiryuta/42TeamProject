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

	public GameObject specialWallPoint;// 特殊壁移動ポイント
	[HideInInspector]
	public GameObject player;//プレイヤー

	[HideInInspector]
	public bool activity;//PostRespawnで処理をおこなうためのbool型
    //[HideInInspector]
    //public int activeCount;//activityが切り替わるまでのアイテムの個数
    public MeshRenderer mesh;//ポストを透明化させるためのもの
    public BoxCollider bc;//あたり判定をなくす

    float interval;//表示するまでの間隔

    public float intervalLimit = 5;//再出現時間

    float limitCount;//移動するために必要なアイテムの個数
    public float inflateTime = 0.05f;//ポストから風船に内容物を膨らませるまでの時間
    public GameObject marker;

    // Use this for initialization
    void Start () {
		//初期化処理
		blastCount = 0;
		balloon = GameObject.FindGameObjectWithTag("Balloon");//爆発物取得
		specialWallPoint = GameObject.Find("SpecialWallPoint(Clone)");//特殊壁移動ポイント取得
        activity = false;
        //activeCount = 0;
        mesh = GetComponent<MeshRenderer>();
        bc = GetComponent<BoxCollider>();
	}

	// Update is called once per frame
	void Update () {

		//爆発物がなければ探す
		if(balloon == null)
		{
			balloon = GameObject.FindGameObjectWithTag("Balloon");
			return;
		}

		//5ポイント貯めたら特殊壁出して移動する
		if (respawnCount >= 5)
        {
            limitCount = respawnCount;//ポストに5ポイント以上入ったときに移動できるようにする
            respawnCount = 0;
            //specialWallPoint.GetComponent<SpecialWallRespawn>().SpecialRespawn(player);
            Debug.Log("移る");
            activity = false;
            isRespawn = true;
        }

		//内容物が一つでもあれば
		if(blastCount>0)
        {
            inflateTime -= Time.deltaTime;
            //5個以上あったとき
            if (limitCount >= 5)
            {
                if (inflateTime < 0)
                {
                    //内容物の総数分風船を膨らます
                    balloon.GetComponent<BalloonController>().BalloonBlast(gameObject);
                    blastCount--;//内容物の総数を減らす
                    inflateTime = 0.05f;
                }
            }

            //if (giveTime < 0)//一定時間ごとに
            //{
            //    //balloon.GetComponent<BalloonController>().blastCount += 0.05f;//内容物を爆発物に移す
            //    balloon.GetComponent<BalloonController>().BalloonBlast(gameObject);
            //    blastCount--;//内容物の総数を減らす
            //    giveTime = 0.2f;
            //    activeCount++;
            //}
        }
		else if(blastCount<0)
		{
			giveTime -= Time.deltaTime;
			if (giveTime < 0)//一定時間ごとに
			{
				//爆発物縮小
				balloon.GetComponent<BalloonController>().BalloonShrink(gameObject);
				blastCount++;//内容物の総数を増やす
				giveTime = 0.2f;
			}
		}
		else
		{
			giveTime = 0.2f;
		}

        //if (activeCount >= 5 && blastCount<=0)
        //{
        //    Debug.Log("移る");
        //    bc.enabled = false;
        //    mesh.enabled = false;
        //    activity = false;
        //    isRespawn = true;
        //    activeCount = 0;
        //}

        //activityがfalseのとき
        if (activity == false)
        {
            //MeshとColliderをfalseにする
            mesh.enabled = false;
            bc.enabled = false;
            interval++;
            //約5秒後に再出現させる。その際に移動させるために必要なものを初期化
            if (interval >= intervalLimit * 60)
            {
                interval = 0;
                limitCount = 0;
                mesh.enabled = true;
                bc.enabled = true;
                activity = true;
                gameObject.GetComponentInParent<PostRespawn>().isLimitReset = true;
                Instantiate(marker, gameObject.transform.position + new Vector3(0, 19.5f, 0), Quaternion.identity);//ポストが出現したらマーカーを出す
            }
        }

        //activityに応じて表示
        //this.gameObject.SetActive (activity);
	}
}
