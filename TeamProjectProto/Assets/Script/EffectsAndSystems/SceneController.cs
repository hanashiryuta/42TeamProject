/*
 * 作成日時：180702
 * ゲームシーン管理クラス
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure; // Required in C#

/// <summary>
/// シーンコントローラー継承元クラス
/// </summary>
public class SceneController : MonoBehaviour
{
    //Load
    [HideInInspector]
    public GameLoad gameLoad;//ゲームロード

    //Fade
    [HideInInspector]
    public GameObject fadePanel;//フェードパネル
    [HideInInspector]
    public FadeController fadeController;//フェードコントローラー
    [HideInInspector]
    public bool isSceneChange = false;//シーン転換するか？
    [HideInInspector]
    public float fadeOutDelayTime = 0f;//fadeout遅延時間

    //XInput
    [HideInInspector]
    public PlayerIndex playerIndex;//操作プレイヤーインデックス
    [HideInInspector]
    public GamePadState previousState;//前のGamePad状態
    [HideInInspector]
    public GamePadState currentState;//今のGamePad状態
    [HideInInspector]
    public float moveX = 0;//X軸移動量
    [HideInInspector]
    public float moveY = 0;//Y軸移動量

    //InputDelay
    [HideInInspector]
    public float inputDelayCnt = 0; //遅延カウント
    public float inputDelayTime = 0.3f;//長押しの時の遅延
    [HideInInspector]
    public bool isInputDelay = false;//遅延中か？

    //SE
    [HideInInspector]
    public SEController se;//SEコントローラー

    //背景
    public GameObject bg;//背景オブジェ
    public float zDistance = 20f;//カメラとのｚ軸距離


    /// <summary>
    /// タイトルシーン状態
    /// </summary>
    public enum TitleSceneState
    {
        FadeIn, //フェードイン中
        None,   //基準状態
        Start,  //スタート
        Creadit,//クレジット
        Exit    //ゲーム終了
    }

    /// <summary>
    /// キャラセレクトシーン状態
    /// </summary>
    public enum CharacterSceneState
    {
        FadeIn,     //フェードイン中
        None,       //基準状態
        ToPreScene, //前のシーンに移行
        ToNextScene //次のシーンに移行
    }

    /// <summary>
    /// ステージシーン状態
    /// </summary>
    public enum StageSceneState
    {
        FadeIn,     //フェードイン中
        None,       //基準状態
        ToPreScene, //前のシーンに移行
        ToNextScene //次のシーンに移行
    }

    public enum GameSceneState
    {
        FadeIn,         //フェードイン中
        None,           //基準状態
        ToTitleScene,   //次のシーンに移行
        ToNextScene     //次のシーンに移行
    }

    /// <summary>
    /// リザルトシーン状態
    /// </summary>
    public enum ResultSceneState
    {
        FadeIn,     //フェードイン中
        RankAnim,   //アニメ中
        None,       //基準状態
        ToNextScene //次のシーンに移行
    }

    // Use this for initialization
    public virtual void Start ()
    {
        //load
        gameLoad = GetComponent<GameLoad>();
        //fade
        fadeController = GameObject.Find("FadePanel").GetComponent<FadeController>();
        //SE
        if (transform.GetComponent<SEController>() != null)
        {
            se = transform.GetComponent<SEController>();
        }
        //BG
        BG_Spawn();
    }

    // Update is called once per frame
    public virtual void Update ()
    {
        SetControllablePlayer();

        //フェードインが終わるまで
        if (fadeController.IsFadeInFinish == false)
        {
            //フェードインし続ける
            PanelFadeIn();
        }
        //フェードインが終わってから
        else
        {
            //シーン移行判定がtrueの時
            if (isSceneChange)
            {
                //フェードアウト
                Invoke("PanelFadeOut", fadeOutDelayTime);
            }
        }

        CheckSceneState();
    }

    /// <summary>
    /// シーンの状態に沿ってメソッド実行
    /// </summary>
    public virtual void CheckSceneState() { }

    /// <summary>
    /// パネルフェードイン
    /// </summary>
    void PanelFadeIn()
    {
        fadeController.FadeIn();
    }

    /// <summary>
    /// パネルフェードアウト
    /// </summary>
    void PanelFadeOut()
    {
        fadeController.FadeOut();
    }

    /// <summary>
    /// 入力遅延カウントダウン
    /// </summary>
    public void InputDelayTimeCountDown()
    {
        inputDelayCnt -= Time.deltaTime;
        if (inputDelayCnt <= 0)
        {
            isInputDelay = false;
        }
    }

    /// <summary>
    /// 操作可能なプレイヤーを選択(番号が一番小さい人、例P１)
    /// /// </summary>
    public void SetControllablePlayer()
    {
        if (GamePad.GetState(PlayerIndex.One).IsConnected)
        {
            playerIndex = PlayerIndex.One;
        }
        else if (GamePad.GetState(PlayerIndex.Two).IsConnected)
        {
            playerIndex = PlayerIndex.Two;
        }
        else if (GamePad.GetState(PlayerIndex.Three).IsConnected)
        {
            playerIndex = PlayerIndex.Three;
        }
        else if (GamePad.GetState(PlayerIndex.Four).IsConnected)
        {
            playerIndex = PlayerIndex.Four;
        }
    }

    /// <summary>
    /// 背景生成
    /// </summary>
    void BG_Spawn()
    {
        //背景位置設定
        Vector3 bg_position = new Vector3(Screen.width / 2, Screen.height / 2, zDistance);
        //ワールド座標に転換
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(bg_position);
        //転換されたワールド座標で生成
        GameObject.Instantiate(bg, worldPoint, Camera.main.transform.rotation);
    }

}
