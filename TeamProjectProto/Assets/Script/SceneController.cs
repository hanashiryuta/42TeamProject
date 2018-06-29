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

    //fade
    FadeController fadeController;
    float cnt = 0;

    PostRespawn postRespawn;

    //pause->toTitle
    bool isToTitle = false;
    public bool IsToTitle
    {
        set { isToTitle = value; }
    }

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

        //fade
        fadeController = GameObject.Find("FadePanel").GetComponent<FadeController>();

        postRespawn = GameObject.Find("PostRespawnPoint").GetComponent<PostRespawn>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }

        if (timeController.GetComponent<TimeController>().isEnd)
        {
            isEnd_ToResult();
        }

        if (isToTitle)
        {
            isEnd_ToTitle();
        }
	}

    /// <summary>
    /// 終了処理(Result)
    /// </summary>
    public void isEnd_ToResult()
    {
        // PlayerRankの順位更新を停止
        playerRank.GetComponent<PlayerRank>().IsInPlay = false;

        //ゲーム中の順位の名前を記録してリザルトシーン用に保存
        List<string> tmp = new List<string>();
        //スコア保存用リスト
        List<float> score = new List<float>();
        foreach (var player in playerRank.GetComponent<PlayerRank>().PlayerRankArray)
        {
            tmp.Add(player.name);
            score.Add(player.GetComponent<PlayerMove>().totalItemCount);
        }
        playerRank.GetComponent<PlayerRank>().ResultRank = tmp;
        playerRank.GetComponent<PlayerRank>().PlayerRankScore = score;

        //万が一シーンが切り替わると同時にコントローラーが振動し始めたときにコントローラーの振動を停止する処理
        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);

        //終了合図
        finishCall.ShowUp();

        //ダッシュスライダーを隠す
        foreach (var p in playerList)
        {
            p.GetComponent<SliderController>().InvisibleSlider();
        }

        List<GameObject> postRespawnPointList = postRespawn.isPostList;

        bool isPostFly = false;

        foreach(var post in postRespawnPointList)
        {
            if (post.GetComponent<PostSet>().isPost == true)
            {
                if ((int)(post.GetComponent<PostSet>().post.GetComponent<PostController>().postState) >= (int)PostState.AIRSPAWN)
                    isPostFly = true;
            }
        }

        cnt += Time.deltaTime;
        if (!isPostFly)
        { 
            //fadeout
            if (fadeController.IsFadeOutFinish == false && cnt >= 3)
            {
                fadeController.FadeOut();
            }

            //シーン遷移
            Invoke("LoadResult", finishCall._waitTime);
        }
    }

    /// <summary>
    /// 追加日：180601 追加者：何
    /// リザルトシーン遷移
    /// </summary>
    void LoadResult()
    {
        SceneManager.LoadScene("Result");
    }

    
    /// <summary>
    /// 終了処理(Title)
    /// </summary>
    public void isEnd_ToTitle()
    {
        // PlayerRankの順位更新を停止
        playerRank.GetComponent<PlayerRank>().IsInPlay = false;

        //万が一シーンが切り替わると同時にコントローラーが振動し始めたときにコントローラーの振動を停止する処理
        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);

        //DOTween全削除
        DOTween.KillAll();

        //fadeout
        fadeController.FadeOut();


        //シーン遷移
        Invoke("LoadTitle", finishCall._waitTime - 3);
    }

    /// <summary>
    /// 追加日：180601 追加者：何
    /// リザルトシーン遷移
    /// </summary>
    void LoadTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
