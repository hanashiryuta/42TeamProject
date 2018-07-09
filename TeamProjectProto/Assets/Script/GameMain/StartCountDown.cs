﻿/*
 * 作成日時：180529
 * ゲームスタートの時のカウントダウン
 * 作成者：何承恩
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class StartCountDown : MonoBehaviour
{
    Text _textCntDown; //カウントダウン用テキスト
    [SerializeField]
    Image _bg;

    [SerializeField]
    int _cntDownTime = 3;
    public float waitTime = 0;

    bool _isCntDown = true;
    public bool IsCntDown
    {
        get { return _isCntDown; }
    }
    bool _isCntDownStarted = false;

    //SE
    AudioSource audio;

    TutorialController tutorialController;
    bool isTuto = true;

	// Use this for initialization
	void Start ()
    {
        _textCntDown = GetComponent<Text>();
        _textCntDown.text = "";

        audio = transform.GetComponent<AudioSource>();
        try
        {
            tutorialController = GameObject.Find("TutorialController").GetComponent<TutorialController>();
        }
        catch(NullReferenceException ne)
        {
            isTuto = false;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (isTuto)
        {
            if (!_isCntDownStarted && !tutorialController.isTutorial)
            {
                StartCoroutine(CountdownCoroutine());
                _isCntDownStarted = true;
            }
        }
        else
        {
            if (!_isCntDownStarted)
            {
                StartCoroutine(CountdownCoroutine());
                _isCntDownStarted = true;
            }
        }

    }

    IEnumerator CountdownCoroutine()
    {
        yield return new WaitForSeconds(waitTime);

        _textCntDown.enabled = true;
        _bg.enabled = true;

        audio.Play();
        for (int i = _cntDownTime; i > 0; i--)
        {
            _textCntDown.text = HalfWidth2FullWidth.Set2FullWidth(i.ToString());
            yield return new WaitForSeconds(1.0f);
        }

        _textCntDown.text = "ＧＯ！";
        yield return new WaitForSeconds(1.0f);

        _textCntDown.text = "";
        _isCntDown = false;
        _textCntDown.enabled = false;
        _bg.enabled = false;
    }

}