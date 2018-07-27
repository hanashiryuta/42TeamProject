/*
 * 作成日：180727
 * デモテキスト
 * 作成者：何承恩
 */
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DemoText : MonoBehaviour
{
    public float textFadeTime = 1;//フェード時間
    CanvasGroup canvasGroup;//キャンバスグループ
    string preScene;//前のシーン

    int alphaValue = 1;//アルファ加算値

    // Use this for initialization
    void Start()
    {
        if (GameObject.Find("GameController") != null)
        {
            preScene = GameObject.Find("GameController").GetComponent<GameSetting>().preScene;
        }

        //DEMO(直接Title->Main)だったらキャンバスグループ取る
        if (preScene == "Title")
        {
            canvasGroup = transform.GetComponent<CanvasGroup>();
        }

    }

    void Update()
    {
        //DEMO(直接Title->Main)だったら文字点滅
        if (preScene == "Title")
        {
            Fade();
        }
    }

    /// <summary>
    /// 文字フェード
    /// </summary>
    void Fade()
    {
        canvasGroup.alpha += Time.deltaTime * alphaValue * (1 / textFadeTime);

        //０か１の時加算値反転
        if (canvasGroup.alpha <= 0 || canvasGroup.alpha >= 1)
        {
            alphaValue = -alphaValue;
        }
    }
}