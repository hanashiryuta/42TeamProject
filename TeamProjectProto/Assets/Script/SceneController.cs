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
using DG.Tweening;

public class SceneController : MonoBehaviour {

    //GameObject balloon;//爆発物
    GameObject timeController;//時間管理オブジェクト
    GameObject playerRank;//プレイヤーランク管理オブジェクト

    //180601 何
    FinishCall finishCall;

    //180614 何
    List<GameObject> playerList = new List<GameObject>();

    // Use this for initialization
    void Start () {
        //balloon = GameObject.FindGameObjectWithTag("Balloon");//爆発物取得
        timeController = GameObject.Find("TimeController");

        //ランク関連
        playerRank = GameObject.Find("PlayerRankController");
        playerRank.GetComponent<PlayerRank>().InitPlayerList();
        playerRank.GetComponent<PlayerRank>().IsInPlay = true;

        //終了関連
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();

        //players
        playerList = GameObject.Find("PlayerRespawns").GetComponent<RespawnController>().PlayerList;
    }
	
	// Update is called once per frame
	void Update () {
        if(timeController.GetComponent<TimeController>().isEnd)
        {
            isEnd();
        }
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

        //終了合図
        finishCall.ShowUp();

        //ダッシュスライダーを隠す
        foreach(var p in playerList)
        {
            p.GetComponent<SliderController>().InvisibleSlider();
        }
        //DOTween全削除
        DOTween.KillAll();

        //シーン遷移
        Invoke("LoadResult", finishCall._waitTime);
    }

    /// <summary>
    /// 追加日：180601 追加者：何
    /// リザルトシーン遷移
    /// </summary>
    void LoadResult()
    {
        SceneManager.LoadScene("Result");
    }
}
