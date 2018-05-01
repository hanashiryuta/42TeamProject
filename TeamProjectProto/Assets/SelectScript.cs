using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectScript : MonoBehaviour
{
    Button Exit;
    Button Title;
    private GameObject instancePauseUI;
    //PauseScript pauseScript;

    // Use this for initialization
    void Start()
    {
        Title = GameObject.Find("/Pause(Clone)/Title").GetComponent<Button>();
        Exit = GameObject.Find("/Pause(Clone)/Exit").GetComponent<Button>();

        Exit.Select();
    }

    // Update is called once per frame
    public void ResetTime()
    {
        Time.timeScale = 1f;
    }

    public void OnClick()
    {
        SceneManager.LoadScene("Title");
    }
}
