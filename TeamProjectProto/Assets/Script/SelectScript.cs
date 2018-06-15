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

//ポーズ状態
public enum PauseState
{
    NONEPAUSE,
    OBJECTSET,
    PAUSESTART,
    PAUSING,
    PAUSEEND,
}

public class SelectScript : MonoBehaviour
{
    public Button Exit;
    public Button Title;

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

    // Use this for initialization
    void Start()
    {
        Title.Select();
        //時間オブジェクト取得
        timeController = GameObject.Find("TimeController");
        pauseList = new List<GameObject>();

        // スタートカウントダウン
        startCntDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        // 終了合図
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();
    }

    // Update is called once per frame
    void Update()
    {
        //スタート、エンドのカウント中だったら
        if (startCntDown.IsCntDown || finishCall.IsCalling)
            return;

        switch (pauseState)
        {
            case PauseState.NONEPAUSE://ポーズ中では
                if (PushStart())
                {
                    pausepanel.SetActive(true);//パネル出現
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

        //if (Input.GetKeyDown("p") || Input.GetButtonDown("Action"))
        //{
        //    if (pausepanel.active == false)
        //    {
        //        //時間を止める
        //        timeController.GetComponent<TimeController>().isPause = true;
        //        pausepanel.active = true;
        //        isObjSet = true;
        //        //Pauser.Pause();
        //        //RBPauser.Pause();

        //        GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        //        GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
        //        GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
        //        GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
        //    }
        //    else
        //    {
        //        //時間を動かす
        //        timeController.GetComponent<TimeController>().isPause = false;
        //        pausepanel.active = false;
        //        isPause = false;
        //        //Pauser.Resume();
        //        //RBPauser.Resume();
        //    }
        //}
    }

    /// <summary>
    /// スタートボタン押したとき
    /// </summary>
    /// <returns></returns>
    bool PushStart()
    {
        if (isRoulette)
            return false;
        return (Input.GetKeyDown("p") || Input.GetButtonDown("Action"));
    }

    // Update is called once per frame
    public void ResetTime()
    {
        pausepanel.active = false;
    }

    public void OnClick()
    {
        SceneManager.LoadScene("Title");
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
        pauseState = PauseState.PAUSING;
    }

    /// <summary>
    /// ポーズ終了
    /// </summary>
    void PauseEnd()
    {
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
}
