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

public class SelectScript : MonoBehaviour
{
    public Button Exit;
    public Button Title;

    public GameObject pausepanel;

    //制限時間オブジェクト
    GameObject timeController;

    // Use this for initialization
    void Start()
    {
        Title.Select();
        //時間オブジェクト取得
        timeController = GameObject.Find("TimeController");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p") || Input.GetButtonDown("Action"))
        {
            if (pausepanel.active == false)
            {
                //時間を止める
                timeController.GetComponent<TimeController>().isPause = true;
                pausepanel.active = true;
                Pauser.Pause();
                RBPauser.Pause();
                
                GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
                GamePad.SetVibration(PlayerIndex.Two, 0.0f, 0.0f);
                GamePad.SetVibration(PlayerIndex.Three, 0.0f, 0.0f);
                GamePad.SetVibration(PlayerIndex.Four, 0.0f, 0.0f);
            }
            else
            {
                //時間を動かす
                timeController.GetComponent<TimeController>().isPause = false;
                pausepanel.active = false;
                Pauser.Resume();
                RBPauser.Resume();
            }
        }
    }

    // Update is called once per frame
    public void ResetTime()
    {
        pausepanel.active = false;
        Pauser.Resume();
        RBPauser.Resume();
    }

    public void OnClick()
    {
        Pauser.Remove();
        RBPauser.Remove();
        SceneManager.LoadScene("Title");
    }
}
