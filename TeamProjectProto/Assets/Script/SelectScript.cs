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

public class SelectScript : MonoBehaviour
{
    public Button Exit;
    public Button Title;

    public GameObject pausepanel;

    // Use this for initialization
    void Start()
    {
        Title.Select();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p") || Input.GetButtonDown("Action"))
        {
            if (pausepanel.active == false)
            {
                pausepanel.active = true;
                Pauser.Pause();
                RBPauser.Pause();
            }
            else
            {
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
