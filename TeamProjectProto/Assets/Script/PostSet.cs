//
//作成日時：6月26日
//中心物体(ポスト)の生成処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostSet : MonoBehaviour {

    GameObject originPost;//ポスト
    [HideInInspector]
    public GameObject post = null;
    float originRespawnTime = 5.0f;//リスポーン時間
    float respawnTime;
    [HideInInspector]
    public bool isRespawn = false;//リスポーンしてるかどうか
    StartCountDown startCntDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script
    GameObject origin_Post_Target_Particle;//ポスト出現ターゲット
    GameObject post_Target_Particle;
    TimeController timeController;//時間管理クラス
    [HideInInspector]
    public bool isPost = false;

    // Use this for initialization
    void Start () {
        respawnTime = originRespawnTime;
        //スタートカウントダウン
        startCntDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        //終了合図
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();
        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
    }

    /// <summary>
    /// ポスト生成初期化
    /// </summary>
    /// <param name="respawnTime"></param>
    /// <param name="post"></param>
    public void StartSet(float respawnTime,GameObject post,GameObject post_Target_Particle)
    {
        //時間設定
        originRespawnTime = respawnTime;
        //ポスト設定
        originPost = post;
        origin_Post_Target_Particle = post_Target_Particle;
    }
	
	// Update is called once per frame
	void Update () {
        //カウントダウン中、終了中は何もしない
        if (startCntDown.IsCntDown || finishCall.IsCalling)
            return;

        //ロスタイムならすぐ生成
        if (timeController.timeState == TimeState.LOSSTIME)
            respawnTime = 0;
        //生成でき、ポストがなければ
        if (isRespawn&&post == null)
        {
            if (post_Target_Particle == null)
            {
                post_Target_Particle = Instantiate(origin_Post_Target_Particle, transform.position - new Vector3(0, 0.1f, 0), Quaternion.identity);
            }
            respawnTime -= Time.deltaTime;
            if(respawnTime <= 0)
            {
                //ポスト生成
                post = Instantiate(originPost, transform.position + new Vector3(0, 0.5f+30, 0), Quaternion.Euler(0, 45+180, 0), transform);
                //自身指定
                post.GetComponent<PostController>().postPoint = gameObject;
                respawnTime = originRespawnTime;
                Destroy(post_Target_Particle);
                isPost = true;
            }
        }
	}
}
