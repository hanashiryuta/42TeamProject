//
//作成日時：6月8日
//バルーン総合管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonMaster : MonoBehaviour {

    public List<GameObject> balloonList;//バルーンリスト
    GameObject nextSpawnBalloon;//次にセルバルーン
    GameObject nowBalloon;//今で出ているバルーン

    [HideInInspector]
    public GameObject nextPlayer;//次にバルーンがつくプレイヤー
    [HideInInspector]
    public GameObject nowPlayer;//今バルーンがついているプレイヤー

    public float originBalloonRespawnTime;//バルーン出現時間インターバル
    float balloonRespawnTime;

    [HideInInspector]
    public GameObject[] pList;//プレイヤーリスト

    StartCountDown startCntDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script

    public bool isCameraDistance = false;

    // Use this for initialization
    void Start (){
        //初期化処理
        pList = GameObject.FindGameObjectsWithTag("Player");
        nextPlayer = pList[Random.Range(0, pList.Length)];
        nowBalloon = null;
        balloonRespawnTime = originBalloonRespawnTime;
        nextSpawnBalloon = balloonList[0];

        startCntDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();
    }

    // Update is called once per frame
    void Update () {
        // カウントダウン中は何もしない
        if (startCntDown.IsCntDown || finishCall.IsCalling)
            return;

        //バルーンがなければ
        if (nowBalloon == null)
        {
            //時間経過
            balloonRespawnTime -= Time.deltaTime;
            if(balloonRespawnTime <= 0)
            {
                //今風船つくプレイヤーを指定
                nowPlayer = nextPlayer;
                //バルーン生成
                nowBalloon = Instantiate(nextSpawnBalloon,nowPlayer.transform.position,Quaternion.identity);
                //自分指定
                nowBalloon.GetComponent<BalloonOrigin>().balloonMaster = this;
                //バルーンにプレイヤー指定
                nowBalloon.GetComponent<BalloonOrigin>().player = nowPlayer;
                //プレイヤーにバルーン指定
                nowPlayer.GetComponent<PlayerMove>().balloon = nowBalloon;
                //次の爆弾指定
                nextSpawnBalloon = balloonList[Random.Range(0, balloonList.Count)];

                balloonRespawnTime = originBalloonRespawnTime;
            }
        }
	}
}
