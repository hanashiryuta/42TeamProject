/*
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
    int _cntDownTime = 3;//カウントダウン秒数
    public float waitTime = 0;//カウントダウン始まるまでの待ち時間

    bool _isCntingDown = true;//カウントダウン中か
    public bool IsCntingDown
    {
        get { return _isCntingDown; }
    }
    bool _isCntDownStarted = false;//カウントダウン開始したか

    //SE
    AudioSource audio;

    //チュートリアル
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

    /// <summary>
    /// カウントダウンコルーチン
    /// </summary>
    /// <returns></returns>
    IEnumerator CountdownCoroutine()
    {
        //フェード待ち
        yield return new WaitForSeconds(waitTime);

        _textCntDown.enabled = true;
        _bg.enabled = true;

        audio.Play();
        for (int i = _cntDownTime; i > 0; i--)
        {
            //秒数に沿って表示
            _textCntDown.text = HalfWidth2FullWidth.Set2FullWidth(i.ToString());
            yield return new WaitForSeconds(1.0f);
        }

        _textCntDown.text = "ＧＯ！";
        yield return new WaitForSeconds(1.0f);

        //カウントダウン終了
        _textCntDown.text = "";
        _isCntingDown = false;
        _textCntDown.enabled = false;
        _bg.enabled = false;
    }

}
