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

	// Use this for initialization
	void Start () {
        //初期化処理
        blastCount = 0;
        balloon = GameObject.FindGameObjectWithTag("Balloon");//爆発物取得
        specialWallPoint = GameObject.Find("SpecialWallPoint(Clone)");//特殊壁移動ポイント取得
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
            respawnCount = 0;
            specialWallPoint.GetComponent<SpecialWallRespawn>().SpecialRespawn(player);
            isRespawn = true;
        }

        //内容物が一つでもあれば
		if(blastCount>0)
        {
            giveTime -= Time.deltaTime;
            if (giveTime < 0)//一定時間ごとに
            {
                //balloon.GetComponent<BalloonController>().blastCount += 0.05f;//内容物を爆発物に移す
                balloon.GetComponent<BalloonController>().BalloonBlast(gameObject);
                blastCount--;//内容物の総数を減らす
                giveTime = 0.2f;
            }
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
	}
}
