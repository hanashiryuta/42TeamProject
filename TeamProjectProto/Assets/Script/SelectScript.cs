//
//5月18日
//ポーズ画面操作
//作成者：安部崇寛
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
using System;
using DG.Tweening;

//ポーズ状態
public enum PauseState
{
    NONEPAUSE,//非ポーズ
    OBJECTSET,//オブジェクトセット
    PAUSESTART,//ポーズ開始
    PAUSING,//ポーズ中
    PAUSEEND,//ポーズ終了
}

/// <summary>
/// ポーズ選択肢
/// </summary>
public enum PauseSelect
{
    ToTitle,//タイトルへ
    Tutorial,//チュートリアル表示
    BackToGame,//ゲームに戻る
}

public class SelectScript : MonoBehaviour
{
    public Button backToGame;//ゲームに戻るボタン
    public Button toTitle;//タイトルに戻るボタン
    public Button tutorial;//チュートリアルを表示するボタン
    Button nowSelectedBtn;//今選択しているボタン

    public GameObject pausepanel;//ポーズ表示パネル
    
    List<GameObject> pauseList;//ポーズするオブジェクト
    public List<string> tagNameList;//ポーズするタグ
    [HideInInspector]
    public PauseState pauseState = PauseState.NONEPAUSE;//ポーズ状態
    [HideInInspector]
    public bool isRoulette;//ルーレット状態か

    StartCountDown startCntDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script

    //制限時間オブジェクト
    GameObject timeController;

    GamePadState previousState;//前フレームコントローラ状態
    GamePadState currentState;//現在のコントローラ状態

    float moveY;//Y入力

    PlayerIndex pausePlayerIndex = 0;//プレイヤーインデックス
    public PlayerIndex PausePlayerIndex
    {
        get { return pausePlayerIndex; }
        set { pausePlayerIndex = value; }
    }
    bool isStartBtnPushed;//ポーズボタン押されたか
    public bool IsStartBtnPushed
    {
        get { return isStartBtnPushed; }
        set { isStartBtnPushed = value; }
    }

    //途中終了用シーンコントローラー
    GameSceneController gameSceneController;

    //ポーズ背景
    Image panelBG;
    //背景色
    List<Color> panelColor = new List<Color>() { Color.red, Color.blue, Color.yellow, Color.green };

    //ポーズ選択肢
    PauseSelect pauseSelect = PauseSelect.ToTitle;
    //選べるかどうか
    bool isSelect = true;
    //チュートリアル表示中かどうか
    [HideInInspector]
    public bool isTutorial = false;
    
    public GameObject originRules;//ルール元オブジェクト
    GameObject rules;
    //ページ数表示オブジェクト
    public GameObject pageCount;

    // Use this for initialization
    void Start()
    {
        //時間オブジェクト取得
        timeController = GameObject.Find("TimeController");
        pauseList = new List<GameObject>();

        // スタートカウントダウン
        startCntDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        // 終了合図
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();
        //途中終了用シーンコントローラー
        gameSceneController = GameObject.Find("SceneController(Clone)").GetComponent<GameSceneController>();
        //ポーズ背景
        panelBG = pausepanel.transform.Find("Panel").GetComponent<Image>();        
    }

    // Update is called once per frame
    void Update()
    {
        //スタート、エンドのカウント中だったら
        if (startCntDown.IsCntDown || finishCall.IsCalling)
            return;

        HandleXInput();

        //状態により処理変更
        switch (pauseState)
        {
            case PauseState.NONEPAUSE://非ポーズ中
                if (PushStart())
                {
                    pausepanel.SetActive(true);//パネル出現
                    SetPanelColorWithPlayerIndex(pausePlayerIndex);//パネル色合わせ
                    pauseState = PauseState.OBJECTSET;//状態変化
                }
                break;
            case PauseState.OBJECTSET://ポーズオブジェクトセット
                ObjectSet();//ポーズオブジェクトセット
                break;
            case PauseState.PAUSESTART://ポーズスタート
                PauseStart();//ポーズスタート
                break;
            case PauseState.PAUSING://ポーズ中
                if (PushStart())
                {
                    pauseState = PauseState.PAUSEEND;//状態変化
                }
                break;
            case PauseState.PAUSEEND://ポーズ終了
                //パネル非表示
                pausepanel.SetActive(false);
                PauseEnd();//ポーズ終了
                break;
        }
    }

    /// <summary>
    /// 追加日：180529 追加者：何
    /// XBoxコントローラー入力
    /// </summary>
    void HandleXInput()
    {
        currentState = GamePad.GetState(pausePlayerIndex);
        if (!currentState.IsConnected)
        {
            return;
        }

        //チュートリアル中かどうか
        if (isTutorial)
        {
            //ルール画像移動処理
            rules.GetComponent<Rule>().Move(previousState, currentState);
            //ルール画像終了なら
            if (rules.GetComponent<Rule>().isRuleEnd)
            {
                //ポーズメニュー表示
                pausepanel.SetActive(true);
                //フラグ設定
                isTutorial = false;
            }
        }
        //チュートリアルではないなら
        else
        {
            //STARTボタンを押したら（ポーズ解除）開始
            isStartBtnPushed = (previousState.Buttons.Start == ButtonState.Released &&
                                currentState.Buttons.Start == ButtonState.Pressed);

            //ポーズ中でなくルーレットもなければ
            if (pauseState == PauseState.PAUSING && !isRoulette)
            {
                //Y入力取得
                moveY = currentState.ThumbSticks.Left.Y;
                //ポーズ中入力
                PausingXInput();
            }
        }

        previousState = currentState;
    }

    /// <summary>
    /// 追加日：180616 追加者：何
    /// ポーズ中入力
    /// </summary>
    void PausingXInput()
    {
        //選択しているものにより処理変更
        switch(pauseSelect)
        {
            case PauseSelect.ToTitle://タイトルへ
                //今選ばれているボタンが「タイトル」でなければ
                if (nowSelectedBtn != toTitle)
                {
                    //「タイトル」ボタン選択
                    toTitle.Select();
                    nowSelectedBtn = toTitle;
                }
                //選ぶことが可能で、左スティックを下に倒したら
                if (moveY <= -0.8f && isSelect)
                {
                    //選べない
                    isSelect = false;
                    //状態遷移
                    pauseSelect = PauseSelect.Tutorial;
                }
                break;
            case PauseSelect.Tutorial://チュートリアル表示
                //今選ばれているボタンが「チュートリアル」でなければ
                if (nowSelectedBtn != tutorial)
                {
                    //「チュートリアル」ボタン選択
                    tutorial.Select();
                    nowSelectedBtn = tutorial;
                }

                //選ぶことが可能で、左スティックを上に倒したら
                if (moveY >= 0.8f && isSelect && !isTutorial)
                {
                    //選べない
                    isSelect = false;
                    //状態遷移
                    pauseSelect = PauseSelect.ToTitle;
                }
                //選ぶことが可能で、左スティックを下に倒したら
                else if (moveY <= -0.8f && isSelect && !isTutorial)
                {
                    //選べない
                    isSelect = false;
                    //状態遷移
                    pauseSelect = PauseSelect.BackToGame;
                }
                break;
            case PauseSelect.BackToGame://ゲームへ戻る
                //今選ばれているボタンが「ゲームへ戻る」でなければ
                if (nowSelectedBtn != backToGame)
                {
                    //「ゲームへ戻る」ボタン選択
                    backToGame.Select();
                    nowSelectedBtn = backToGame;
                }
                //選ぶことが可能で、左スティックを上に倒したら
                if (moveY >= 0.8f && isSelect)
                {
                    //選べない
                    isSelect = false;
                    //状態遷移
                    pauseSelect = PauseSelect.Tutorial;
                }
                break;
        }

        //一度左スティックを戻さないと
        if(moveY < 0.8f&&moveY > -0.8f)
        {
            //選択可能にならない
            isSelect = true;
        }
        
        //Aボタンが押されたら
        if (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed)
        {
            //現在押されているボタン処理を行う
            BtnPushed(nowSelectedBtn);
        }
    }

    /// <summary>
    /// 選択しているボタンのonClickイベントを呼ぶ
    /// </summary>
    /// <param name="btn"></param>
    void BtnPushed(Button btn)
    {
        btn.onClick.Invoke();
    }

    /// <summary>
    /// スタートボタン押したとき
    /// </summary>
    /// <returns></returns>
    bool PushStart()
    {
        if (isRoulette)
            return false;
        return (isStartBtnPushed);
    }

    /// <summary>
    /// ポーズ状態解除ボタン
    /// </summary>
    public void BackToGame()
    {
        pauseState = PauseState.PAUSEEND;
    }

    /// <summary>
    /// タイトルに戻るボタン
    /// </summary>
    public void BackToTitle()
    {
        //SceneManager.LoadScene("Title");
        gameSceneController.IsToTitle = true;
    }

    /// <summary>
    /// チュートリアル表示ボタン
    /// </summary>
    public void ShowTutorial()
    {       
        if (!isTutorial)
        {
            //ポーズパネルを非表示
            pausepanel.SetActive(false);
            //ルール画像表示オブジェクト生成
            rules = Instantiate(originRules, transform);
            //ページカウント設定
            rules.GetComponent<Rule>().PageCount = pageCount;
            //チュートリアル状態
            isTutorial = true;
        }
    }

    /// <summary>
    /// オブジェクトセット
    /// </summary>
    void ObjectSet()
    {
        //振動止める
        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
        //ポーズリストクリア
        pauseList.Clear();
        //設定したタグをポーズ対象に選択
        foreach (var tagName in tagNameList)
        {
            //タグで探す
            GameObject[] objList = GameObject.FindGameObjectsWithTag(tagName);

            //リスト追加
            foreach (var obj in objList)
            {
                pauseList.Add(obj);
            }
        }
        //状態遷移
        pauseState = PauseState.PAUSESTART;
    }

    /// <summary>
    /// ポーズスタート
    /// </summary>
    void PauseStart()
    {
        //ボタン選択
        pauseSelect = PauseSelect.ToTitle;
        //時間を止める
        timeController.GetComponent<TimeController>().isPause = true;
        //ポーズ対象から1つずつ呼び出す
        foreach (var pauseObj in pauseList)
        {
            //空なら飛ばす
            if (pauseObj == null)
                continue;

            //スクリプト取得
            Behaviour[] pauseBehavs = Array.FindAll(pauseObj.GetComponentsInChildren<Behaviour>(), (obj) => { return obj.enabled; });
            //スクリプトを一時非アクティブ化
            foreach (var com in pauseBehavs)
            {
                com.enabled = false;
            }

            //リジットボディ取得
            Rigidbody[] rgBodies = Array.FindAll(pauseObj.GetComponentsInChildren<Rigidbody>(), (obj) => { return !obj.IsSleeping(); });
            //リジッドボディがあれば
            if (rgBodies.Length != 0)
            {
                //移動量保存用配列
                Vector3[] rgBodyVels = new Vector3[rgBodies.Length];
                Vector3[] rgBodyAVels = new Vector3[rgBodies.Length];
                for (var i = 0; i < rgBodies.Length; ++i)
                {
                    //移動量保存用
                    rgBodyVels[i] = rgBodies[i].velocity;
                    rgBodyAVels[i] = rgBodies[i].angularVelocity;
                    //リジッドボディ止める
                    rgBodies[i].Sleep();
                }
            }
        }
        //タイトルボタン選択
        toTitle.Select();
        nowSelectedBtn = toTitle;
        //状態遷移
        pauseState = PauseState.PAUSING;
    }

    /// <summary>
    /// ポーズ終了
    /// </summary>
    void PauseEnd()
    {
        //もしチュートリアル状態なら
        if (isTutorial)
        {
            //ルール画像終了処理
            rules.GetComponent<Rule>().Death();
            //状態変化
            isTutorial = false;
        }
        //タイム開始
        timeController.GetComponent<TimeController>().isPause = false;
        //ポーズ対象から1つずつ呼び出す
        foreach (var pauseObj in pauseList)
        {
            //空なら飛ばす
            if (pauseObj == null)
                continue;

            //スクリプト取得
            Behaviour[] pauseBehavs = Array.FindAll(pauseObj.GetComponentsInChildren<Behaviour>(), (obj) => { return !obj.enabled; });
            //スクリプトをアクティブ化
            foreach (var com in pauseBehavs)
            {
                com.enabled = true;
            }

            //リジットボディ取得
            Rigidbody[] rgBodies = Array.FindAll(pauseObj.GetComponentsInChildren<Rigidbody>(), (obj) => { return obj.IsSleeping(); });
            if (rgBodies.Length != 0)
            {
                //移動量配列
                Vector3[] rgBodyVels = new Vector3[rgBodies.Length];
                Vector3[] rgBodyAVels = new Vector3[rgBodies.Length];
                for (var i = 0; i < rgBodies.Length; ++i)
                {
                    //リジッドボディ止める
                    rgBodies[i].WakeUp();
                    //移動量回帰
                    rgBodies[i].velocity = rgBodyVels[i];
                    rgBodies[i].angularVelocity = rgBodyAVels[i];
                }
            }
        }
        //状態遷移
        pauseState = PauseState.NONEPAUSE;
    }

    /// <summary>
    /// パネルの色をポーズ押したプレイヤーに合わせる
    /// </summary>
    /// <param name="playeIndex"></param>
    void SetPanelColorWithPlayerIndex(PlayerIndex playeIndex)
    {
        panelBG.color = panelColor[(int)playeIndex];
    }
}
