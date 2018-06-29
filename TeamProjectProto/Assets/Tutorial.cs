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
    public List<Sprite> ruleSpriteList;//ルール表示画像リスト
    List<GameObject> ruleList;//ルールリスト
    PlayerIndex playerIndex;//プレイヤー番号
    GamePadState previousState;//1フレーム前のコントローラ
    GamePadState currentState;//現在のコントローラ
    [HideInInspector]
    public TutorialController tutorialController;//チュートリアル管理クラス
    TutorialState tutorialState = TutorialState.Rule;//チュートリアル表示状態
    public GameObject Rules;//ルール元オブジェクト
    public GameObject Operations;//操作説明元オブジェクト
    int imageCount = 0;//ルールページ数
    GameObject operation;
    float stayTime = 0.0f;
    public GameObject PageCount;
    [HideInInspector]
    public FinishCall finishObj;
    [HideInInspector]
    public BalloonMaster balloonMaster;
    [HideInInspector]
    public SelectScript selectScript;
    bool isOperation;

    // Use this for initialization
    void Start () {
        ruleList = new List<GameObject>();
        for (int i = 0; i < ruleSpriteList.Count; i++)
        {
            GameObject rule;
            rule = Instantiate(Rules, transform);
            rule.GetComponent<RectTransform>().position += new Vector3(i*1280,0, 0);
            rule.transform.GetChild(1).GetComponent<Image>().sprite = ruleSpriteList[i];
            ruleList.Add(rule);
        }

        PageCount.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        currentState = GamePad.GetState(playerIndex);
        switch (tutorialState)
        {
            case TutorialState.Rule:
                PageCount.GetComponent<Text>().text = HalfWidth2FullWidth.Set2FullWidth((imageCount + 1).ToString("0")) + "／" + HalfWidth2FullWidth.Set2FullWidth(ruleSpriteList.Count.ToString("0"));
                PageCount.transform.SetAsLastSibling();
                stayTime -= Time.deltaTime;
                if (stayTime <= 0)
                {
                    if (previousState.Buttons.A == ButtonState.Released &&
                    currentState.Buttons.A == ButtonState.Pressed)
                    {
                        stayTime = 1.0f;
                        imageCount++;
                        foreach (var rule in ruleList)
                        {
                            rule.GetComponent<RectTransform>().DOMoveX(rule.GetComponent<RectTransform>().position.x - 1280, 1.0f);
                        }
                    }
                    if (imageCount >= ruleSpriteList.Count)
                    {
                        PageCount.SetActive(false);
                        operation = Instantiate(Operations, transform);
                        tutorialController.isTutorial = false;
                        tutorialController.bgmController.SetSceneBGM(SceneManager.GetActiveScene(), "StageSelect");
                        tutorialState = TutorialState.Operation;
                    }
                }
                if (previousState.Buttons.Start == ButtonState.Released &&
                    currentState.Buttons.Start == ButtonState.Pressed)
                {
                    foreach (var rule in ruleList)
                    {
                        rule.SetActive(false);
                    }
                    tutorialController.isTutorial = false;
                    tutorialController.bgmController.SetSceneBGM(SceneManager.GetActiveScene(), "StageSelect");
                    PageCount.SetActive(false);
                    operation = Instantiate(Operations, transform);
                    tutorialState = TutorialState.Operation;
                }               
                    break;
            case TutorialState.Operation:
                stayTime -= Time.deltaTime;
                if(stayTime <= 0)
                {
                    operation.GetComponent<RectTransform>().DOMoveY(360, 1.0f);
                    tutorialState = TutorialState.Stay;
                }
                break;
            case TutorialState.Stay:
                if (finishObj == null)
                    finishObj = GameObject.Find("FinishCall").GetComponent<FinishCall>();
                if (balloonMaster == null)
                    balloonMaster = GameObject.Find("BalloonMaster(Clone)").GetComponent<BalloonMaster>();
                if (selectScript == null)
                    selectScript = GameObject.Find("PauseCtrl").GetComponent<SelectScript>();

                else
                {
                    if (finishObj.IsCalling)
                        operation.SetActive(false);
                    else if (balloonMaster.isRoulette)
                        operation.SetActive(false);
                    //else
                    //    operation.SetActive(true);
                    else if (selectScript.isTutorial)
                        operation.SetActive(false);
                    else
                        operation.SetActive(true);
                }
                break;
        }
    
        previousState = currentState;
    }
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
}
