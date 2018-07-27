/*
 * 作成日時：180601
 * ゲームス終了時のコール
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishCall : MonoBehaviour
{
    Text _text; //カウントダウン用テキスト
    [SerializeField]
    Image _bg;

    public float _waitTime = 3;//フェードまでの待ち時間

    bool _isShowing = false;//表示中か
    public bool IsShowing
    {
        get { return _isShowing; }
    }

    //SE
    AudioSource audio;
    bool isSE = false;

    // Use this for initialization
    void Start ()
    {
        _text = GetComponent<Text>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update ()
    {

	}

    /// <summary>
    /// 表示
    /// </summary>
    public void ShowUp()
    {
        _isShowing = true;
        _text.enabled = true;
        _bg.enabled = true;

        if (!isSE)
        {
            audio.Play();
            isSE = true;
        }
    }

    /// <summary>
    /// 非表示
    /// </summary>
    public void ShutDown()
    {
        _text.text = "";
        _text.enabled = false;
        _bg.enabled = false;
    }
}
