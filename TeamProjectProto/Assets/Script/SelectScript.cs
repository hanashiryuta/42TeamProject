﻿//
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
    NONEPAUSE,
    OBJECTSET,
    PAUSESTART,
    PAUSING,
    PAUSEEND,
}

public enum PauseSelect
{
    ToTitle,
    Tutorial,
    BackToGame,
}

public class SelectScript : MonoBehaviour
{
    public Button backToGame;
    public Button toTitle;
    public Button tutorial;
    Button nowSelectedBtn;

    public GameObject pausepanel;
    
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

    GamePadState previousState;
    GamePadState currentState;

    float moveY;

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
    GameSceneController scenecontroller;

    //ポーズ背景
    Image panelBG;
    List<Color> panelColor = new List<Color>() { Color.red, Color.blue, Color.yellow, Color.green };

    PauseSelect pauseSelect = PauseSelect.ToTitle;
    bool isSelect = true;
    [HideInInspector]
    public bool isTutorial = false;
    float tutorialCount = 0;

    public List<Sprite> ruleSpriteList;//ルール表示画像リスト
    List<GameObject> ruleList;//ルールリスト
    public GameObject Rules;//ルール元オブジェクト
    float stayTime =  0;
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
        scenecontroller = GameObject.Find("SceneController(Clone)").GetComponent<GameSceneController>();
        //ポーズ背景
        panelBG = pausepanel.transform.Find("Panel").GetComponent<Image>();

        ruleList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        //スタート、エンドのカウント中だったら
        if (startCntDown.IsCntDown || finishCall.IsCalling)
            return;

        //if(pauseState != PauseState.NONEPAUSE)
            HandleXInput();

        switch (pauseState)
        {
            case PauseState.NONEPAUSE://非ポーズ中では
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
                //パネル削除
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

        //STARTボタンを押したら（ポーズ解除）開始
        isStartBtnPushed = (previousState.Buttons.Start == ButtonState.Released &&
                            currentState.Buttons.Start == ButtonState.Pressed);

        if (pauseState == PauseState.PAUSING&&!isRoulette)
        {
            moveY = currentState.ThumbSticks.Left.Y;
            PausingXInput();
        }

        previousState = currentState;
    }

    /// <summary>
    /// 追加日：180616 追加者：何
    /// ポーズ中入力
    /// </summary>
    void PausingXInput()
    {
        switch(pauseSelect)
        {
            case PauseSelect.ToTitle:
                if (nowSelectedBtn != toTitle)
                {
                    toTitle.Select();
                    nowSelectedBtn = toTitle;
                }
                if (moveY <= -0.8f && isSelect)
                {
                    isSelect = false;
                    pauseSelect = PauseSelect.Tutorial;
                }
                break;
            case PauseSelect.Tutorial:
                if (nowSelectedBtn != tutorial)
                {
                    tutorial.Select();
                    nowSelectedBtn = tutorial;
                }
                stayTime -= Time.deltaTime;
                if (moveY >= 0.8f && isSelect && !isTutorial)
                {
                    tutorialCount = 0;
                    isSelect = false;
                    pauseSelect = PauseSelect.ToTitle;
                }
                else if (moveY <= -0.8f && isSelect && !isTutorial)
                {
                    tutorialCount = 0;
                    isSelect = false;
                    pauseSelect = PauseSelect.BackToGame;
                }
                break;
            case PauseSelect.BackToGame:
                if (nowSelectedBtn != backToGame)
                {
                    backToGame.Select();
                    nowSelectedBtn = backToGame;
                }
                if (moveY >= 0.8f && isSelect)
                {
                    isSelect = false;
                    pauseSelect = PauseSelect.Tutorial;
                }
                break;
        }

        if(moveY < 0.8f&&moveY > -0.8f)
        {
            isSelect = true;
        }

        ////up
        //if (moveY >=0.8f && nowSelectedBtn != toTitle)
        //{
        //    toTitle.Select();
        //    nowSelectedBtn = toTitle;
        //}

        ////down
        //if (moveY <= -0.8f && nowSelectedBtn != backToGame)
        //{
        //    backToGame.Select();
        //    nowSelectedBtn = backToGame;
        //}

        //A
        if (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed)
        {
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
        scenecontroller.IsToTitle = true;
    }

    public void ShowTutorial()
    {       
        if (tutorialCount <= 0 && !isTutorial)
        {
            pageCount.SetActive(true);
            pausepanel.SetActive(false);
            ruleList = new List<GameObject>();
            for (int i = 0; i < ruleSpriteList.Count; i++)
            {
                GameObject rule;
                rule = Instantiate(Rules, transform);
                rule.GetComponent<RectTransform>().position += new Vector3(i * Screen.width, 0, 0);
                rule.transform.GetChild(1).GetComponent<Image>().sprite = ruleSpriteList[i];
                ruleList.Add(rule);
            }
            isTutorial = true;
        }
        else
        {
            if (stayTime <= 0)
            {
                stayTime = 1.0f;
                tutorialCount++;
                foreach (var rule in ruleList)
                {
                    rule.GetComponent<RectTransform>().DOMoveX(rule.GetComponent<RectTransform>().position.x - Screen.width, 1.0f);
                }
            }
            if (tutorialCount >= ruleSpriteList.Count)
            {
                foreach (var tutorial in ruleList)
                {
                    Destroy(tutorial);
                }
                ruleList.Clear();
                tutorialCount = 0;
                stayTime = 0.0f;
                pageCount.SetActive(false);
                pausepanel.SetActive(true);
                isTutorial = false;
            }
        }

        pageCount.GetComponent<Text>().text = HalfWidth2FullWidth.Set2FullWidth((tutorialCount + 1).ToString("0")) + "／" + HalfWidth2FullWidth.Set2FullWidth(ruleSpriteList.Count.ToString("0"));
    }

    /// <summary>
    /// オブジェクトセット
    /// </summary>
    void ObjectSet()
    {
        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
        GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
        pauseList.Clear();
        foreach (var tagName in tagNameList)
        {
            GameObject[] objList = GameObject.FindGameObjectsWithTag(tagName);

            foreach (var obj in objList)
            {
                pauseList.Add(obj);
            }
        }
        pauseState = PauseState.PAUSESTART;
    }

    /// <summary>
    /// ポーズスタート
    /// </summary>
    void PauseStart()
    {
        pauseSelect = PauseSelect.ToTitle;
        //時間を止める
        timeController.GetComponent<TimeController>().isPause = true;
        foreach (var pauseObj in pauseList)
        {
            if (pauseObj == null)
                continue;
            Behaviour[] pauseBehavs = Array.FindAll(pauseObj.GetComponentsInChildren<Behaviour>(), (obj) => { return obj.enabled; });
            foreach (var com in pauseBehavs)
            {
                com.enabled = false;
            }

            Rigidbody[] rgBodies = Array.FindAll(pauseObj.GetComponentsInChildren<Rigidbody>(), (obj) => { return !obj.IsSleeping(); });
            if (rgBodies.Length != 0)
            {
                Vector3[] rgBodyVels = new Vector3[rgBodies.Length];
                Vector3[] rgBodyAVels = new Vector3[rgBodies.Length];
                for (var i = 0; i < rgBodies.Length; ++i)
                {
                    rgBodyVels[i] = rgBodies[i].velocity;
                    rgBodyAVels[i] = rgBodies[i].angularVelocity;
                    rgBodies[i].Sleep();
                }
            }
        }
        toTitle.Select();
        nowSelectedBtn = toTitle;
        pauseState = PauseState.PAUSING;
    }

    /// <summary>
    /// ポーズ終了
    /// </summary>
    void PauseEnd()
    {
        foreach (var tutorial in ruleList)
        {
            Destroy(tutorial);
        }
        ruleList.Clear();
        tutorialCount = 0;
        stayTime = 0.0f;
        pageCount.SetActive(false);
        isTutorial = false;
        timeController.GetComponent<TimeController>().isPause = false;
        foreach (var pauseObj in pauseList)
        {
            if (pauseObj == null)
                continue;
            Behaviour[] pauseBehavs = Array.FindAll(pauseObj.GetComponentsInChildren<Behaviour>(), (obj) => { return !obj.enabled; });
            foreach (var com in pauseBehavs)
            {
                com.enabled = true;
            }

            Rigidbody[] rgBodies = Array.FindAll(pauseObj.GetComponentsInChildren<Rigidbody>(), (obj) => { return obj.IsSleeping(); });
            if (rgBodies.Length != 0)
            {
                Vector3[] rgBodyVels = new Vector3[rgBodies.Length];
                Vector3[] rgBodyAVels = new Vector3[rgBodies.Length];
                for (var i = 0; i < rgBodies.Length; ++i)
                {
                    rgBodies[i].WakeUp();
                    rgBodies[i].velocity = rgBodyVels[i];
                    rgBodies[i].angularVelocity = rgBodyAVels[i];
                }
            }
        }

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
