//
//6月28日
//チュートリアルクラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using XInputDotNetPure;

//チュートリアル表示状態
public enum TutorialState
{
    Rule,//ルール表示
    Operation,//操作説明表示
    Stay//待機状態
}

public class Tutorial : MonoBehaviour
{
    PlayerIndex playerIndex;//プレイヤー番号
    GamePadState previousState;//1フレーム前のコントローラ
    GamePadState currentState;//現在のコントローラ

    [HideInInspector]
    public TutorialController tutorialController;//チュートリアル管理クラス
    TutorialState tutorialState = TutorialState.Rule;//チュートリアル表示状態

    public GameObject originRules;//ルール元オブジェクト
    GameObject rules;
    Rule ruleScript;
    public GameObject Operations;//操作説明元オブジェクト
    GameObject operation;

    public GameObject PageCount;//ページ数
    float stayTime = 1.0f;//ボタン待機時間

    [HideInInspector]
    public FinishCall finishObj;//終了表示
    [HideInInspector]
    public BalloonMaster balloonMaster;//バルーン管理
    [HideInInspector]
    public SelectScript selectScript;//ポーズ管理
    
    // Use this for initialization
    void Start () {
        //ルール表示オブジェクト生成
        rules = Instantiate(originRules, transform);
        //スクリプト取得
        ruleScript = rules.GetComponent<Rule>();
        //ページ数設定
        ruleScript.PageCount = PageCount;
        
        //各スクリプト取得
        finishObj = GameObject.Find("FinishCall").GetComponent<FinishCall>();
        balloonMaster = GameObject.Find("BalloonMaster(Clone)").GetComponent<BalloonMaster>();
        selectScript = GameObject.Find("PauseCtrl").GetComponent<SelectScript>();
    }

    // Update is called once per frame
    void Update()
    {
        currentState = GamePad.GetState(playerIndex);
        //状態により処理変更
        switch (tutorialState)
        {
            case TutorialState.Rule://ルール表示
                Rule();
                break;
            case TutorialState.Operation://操作説明表示
                Operation();
                break;
            case TutorialState.Stay://待機状態
                Stay();
                break;
        }
    
        previousState = currentState;
    }

    /// <summary>
    /// 操作プレイヤー設定
    /// </summary>
    void SetControllablePlayer()
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
    /// ルール表示
    /// </summary>
    void Rule()
    {
        //ルール画像移動処理
        ruleScript.Move(previousState, currentState);
        //ルール終了状態なら
        if (ruleScript.isRuleEnd)
        {
            //チュートリアルしないフラグ
            tutorialController.isTutorial = false;
            //BGM変更
            tutorialController.bgmController.SetSceneBGM(SceneManager.GetActiveScene(), "StageSelect");
            //操作説明表示
            operation = Instantiate(Operations, transform);
            //状態遷移
            tutorialState = TutorialState.Operation;
        }
    }

    /// <summary>
    /// 操作説明
    /// </summary>
    void Operation()
    {
        //時間経過
        stayTime -= Time.deltaTime;
        //時間が来たら
        if (stayTime <= 0)
        {
            //画面下から操作説明出す
            operation.GetComponent<RectTransform>().DOMoveY(360, 1.0f);
            //状態遷移
            tutorialState = TutorialState.Stay;
        }
    }

    /// <summary>
    /// 待機
    /// </summary>
    void Stay()
    {
        //ゲーム終了時、ルーレット表示時、ポーズ表示時、操作説明は隠す
        if (finishObj.IsCalling || balloonMaster.isRoulette || selectScript.isTutorial)
            operation.SetActive(false);
        else
            operation.SetActive(true);//それ以外は表示
    }
}
