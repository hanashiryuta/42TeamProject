//
//作成日時：4月18日
//ゲームシーン管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using DG.Tweening;

public class GameSceneController : SceneController
{
    GameObject timeController;//時間管理オブジェクト
    GameObject playerRank;//プレイヤーランク管理オブジェクト

    //180601 何
    FinishCall finishCall;//終了コール

    //180614 何
    List<GameObject> playerList = new List<GameObject>();//生成したプレイヤーのリスト

    PostRespawn postRespawn;
    bool isPostFliedWhenFinish = false;//終了時貯金箱飛んでるか？

    //pause->toTitle
    bool isToTitle = false;//タイトルに行くか
    public bool IsToTitle
    {
        set { isToTitle = value; }
    }

    //SceneState
    GameSceneState sceneState = GameSceneState.FadeIn;
    GameLoad.Scenes nextScene;

    //DEMO
    RespawnController respawnController;

    // Use this for initialization
    public override void Start()
    { 
        timeController = GameObject.Find("TimeController");

        //ランク関連
        playerRank = GameObject.Find("PlayerRankController");
        playerRank.GetComponent<PlayerRank>().InitPlayerList();
        playerRank.GetComponent<PlayerRank>().IsInPlay = true;

        //終了関連
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();

        //players
        respawnController = GameObject.Find("PlayerRespawns").GetComponent<RespawnController>();
        playerList = respawnController.PlayerList;

        postRespawn = GameObject.Find("PostRespawnPoint").GetComponent<PostRespawn>();

        base.Start();
    }

    /// <summary>
    /// シーンの状態に沿ってメソッド実行
    /// </summary>
    public override void CheckSceneState()
    {
        switch (sceneState)
        {
            case GameSceneState.FadeIn://フェードイン中
                if (fadeController.IsFadeInFinish)
                {
                    if (respawnController.isDemo)
                    {
                        sceneState = GameSceneState.Demo;
                    }
                    else
                    {
                        sceneState = GameSceneState.None;
                    }
                }
                break;

            case GameSceneState.None://基準状態
                if (timeController.GetComponent<TimeController>().isEnd)
                {
                    nextScene = GameLoad.Scenes.Result;
                    sceneState = GameSceneState.ToNextScene;
                }
                if (isToTitle)
                {
                    nextScene = GameLoad.Scenes.Title;
                    sceneState = GameSceneState.ToTitleScene;
                }
                break;

            case GameSceneState.ToTitleScene://タイトルシーンに移行
                isEnd_WithoutFinishMotion(nextScene);
                //フェードアウト終わったら
                if (fadeController.IsFadeOutFinish)
                {
                    DOTween.KillAll();
                    //NextSceneLoad
                    gameLoad.LoadingStartWithOBJ();
                }
                break;

            case GameSceneState.ToNextScene://次のシーンに移行
                isEnd_WithFinishMotion(nextScene);
                //フェードアウト終わったら
                if (fadeController.IsFadeOutFinish)
                {
                    DOTween.KillAll();
                    //NextSceneLoad
                    gameLoad.LoadingStartWithoutOBJ();
                }
                break;

            case GameSceneState.Demo://デモモード
                DemoInput();
                if (timeController.GetComponent<TimeController>().isEnd)
                {
                    nextScene = GameLoad.Scenes.Title;
                    sceneState = GameSceneState.ToNextScene;
                }
                if (isToTitle)
                {
                    nextScene = GameLoad.Scenes.Title;
                    sceneState = GameSceneState.ToTitleScene;
                }
                break;
        }
    }

    /// <summary>
    /// 終了処理(Finish処理あり)
    /// </summary>
    void isEnd_WithFinishMotion(GameLoad.Scenes next)
    {
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

        int isPostFly = 0;

        foreach(var post in postRespawnPointList)
        {
            if (post.GetComponent<PostSet>().isPost == true)
            {
                if ((int)(post.GetComponent<PostSet>().post.GetComponent<PostController>().postState) >= (int)PostState.AIRSPAWN)
                {
                    isPostFly++;
                    isPostFliedWhenFinish = true;
                }
            }
        }

        if (isPostFly <= 0)
        {
            SetPlayerRank();
            gameLoad.NextScene = next;

            if (isPostFliedWhenFinish)//飛んだら
            {
                fadeOutDelayTime = 0;//貯金箱終了時フェード開始
            }
            else//飛んでなかったら
            {
                fadeOutDelayTime = finishCall._waitTime;//待ち時間終わったらフェード開始
            }
            isSceneChange = true;
        }
    }

    /// <summary>
    /// 終了処理(Finish処理なし)
    /// </summary>
    void isEnd_WithoutFinishMotion(GameLoad.Scenes next)
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
        gameLoad.NextScene = next;
        isSceneChange = true;
    }

    /// <summary>
    /// プレイヤーの順位とスコアを格納
    /// </summary>
    void SetPlayerRank()
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
    }

    /// <summary>
    /// デモ中のボタン操作
    /// </summary>
    void DemoInput()
    {
        for (int i = 0; i < 4; i++)
        {
            if (GamePad.GetState((PlayerIndex)i).IsConnected)
            {
                GamePadState demoState = GamePad.GetState((PlayerIndex)i);
                //Aボタン押したら
                if (demoState.Buttons.A == ButtonState.Pressed)
                {
                    //タイトルに
                    isToTitle = true;
                }
            }

        }
    }
}
