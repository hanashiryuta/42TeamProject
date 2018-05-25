//
//作成日時：4月18日
//シーン管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;


public class SceneController : MonoBehaviour {

    //GameObject balloon;//爆発物
    GameObject timeController;//時間管理オブジェクト
    GameObject playerRank;//プレイヤーランク管理オブジェクト

	// Use this for initialization
	void Start () {
        //balloon = GameObject.FindGameObjectWithTag("Balloon");//爆発物取得
        timeController = GameObject.Find("TimeController");
        playerRank = GameObject.Find("PlayerRankController");
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
        // PlayerRankの順位更新を停止
        playerRank.GetComponent<PlayerRank>().IsInPlay = false;

        //ゲーム中の順位の名前を記録してリザルトシーン用に保存
        List<string> tmp = new List<string>();
        foreach(var player in playerRank.GetComponent<PlayerRank>().PlayerRankArray)
        {
            tmp.Add(player.name);
        }
        playerRank.GetComponent<PlayerRank>().ResultRank = tmp;

        //万が一シーンが切り替わると同時にコントローラーが振動し始めたときにコントローラーの振動を停止する処理
        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
        
        //シーン遷移
        SceneManager.LoadScene("Result");

    }
}
