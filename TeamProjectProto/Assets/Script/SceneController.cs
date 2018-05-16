//
//作成日時：4月18日
//シーン管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    //GameObject balloon;//爆発物
    GameObject timeController;//時間管理オブジェクト
    GameObject playerRank;//プレイヤーランク管理オブジェクト

	// Use this for initialization
	void Start () {
        //balloon = GameObject.FindGameObjectWithTag("Balloon");//爆発物取得
        timeController = GameObject.Find("TimeController");
        playerRank = GameObject.Find("PlayerRank");
    }
	
	// Update is called once per frame
	void Update () {
        if(timeController.GetComponent<TimeController>().isEnd)
        {
            isEnd();
        }
        ////爆発物がなければ探す
        //if(balloon == null)
        //{
        //    balloon = GameObject.FindGameObjectWithTag("Balloon");
        //    return;
        //}


        ////プレイヤーの人数が1人になったらシーンをリロードする
        //if (balloon.GetComponent<BalloonController>().isEnd)
        //{
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}
		
	}

    /// <summary>
    /// 終了処理
    /// </summary>
    public void isEnd()
    {
        GameObject[] pList = GameObject.FindGameObjectsWithTag("Player");//プレイ

        //ソート（小さい順に）
        for(int i = 0;i< pList.Length-1; i++)
        {
            for (int j = i+1; j < pList.Length; j++)
            {
                if(pList[i].GetComponent<PlayerMove>().totalBlastCount > pList[j].GetComponent<PlayerMove>().totalBlastCount)
                {
                    GameObject p = pList[j];
                    pList[j] = pList[i];
                    pList[i] = p;
                }
            }
        }

        playerRank.GetComponent<PlayerRank>().Reset(); //ランクリストをリセット

        //ランク登録
        foreach(var cx in pList)
        {
            playerRank.GetComponent<PlayerRank>().playerRankList.Add(cx.name);
        }
        
        //シーン遷移
        SceneManager.LoadScene("Result");
    }
}
