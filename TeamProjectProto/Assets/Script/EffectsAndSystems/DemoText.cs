using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DemoText : MonoBehaviour
{
    public float textFadeTime = 1;
    CanvasGroup canvasGroup;
    string preScene;//前のシーン

    int value = 1;

    // Use this for initialization
    void Start()
    {
        if (GameObject.Find("GameController") != null)
        {
            preScene = GameObject.Find("GameController").GetComponent<GameSetting>().preScene;
        }

        //DEMO(直接Title->Main)だったら文字点滅
        if (preScene == "Title")
        {
            canvasGroup = transform.GetComponent<CanvasGroup>();
        }

    }

    void Update()
    {
        if (preScene == "Title")
        {
            Fade();
        }
    }

    void Fade()
    {
        canvasGroup.alpha += Time.deltaTime * value * (1 / textFadeTime);

        if (canvasGroup.alpha <= 0)
        {
            value = 1;
        }

        if (canvasGroup.alpha >= 1)
        {
            value = -1;
        }
    }
}