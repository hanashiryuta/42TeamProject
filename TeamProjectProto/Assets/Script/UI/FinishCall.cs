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

    public float _waitTime = 2;

    bool _isCalling = false;
    public bool IsCalling
    {
        get { return _isCalling; }
    }

    // Use this for initialization
    void Start ()
    {
        _text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update ()
    {

	}

    public void ShowUp()
    {
        _isCalling = true;
        _text.enabled = true;
        _bg.enabled = true;
    }

    public void ShutDown()
    {
        _text.text = "";
        _text.enabled = false;
        _bg.enabled = false;
    }
}
