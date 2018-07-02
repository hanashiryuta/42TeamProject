/*
 * 作成日：180504
 * フェードコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeController : MonoBehaviour
{
    Image fadeImage; //フェード用イメージコンポーネント
    float _fadeAlphaValue = 255; //一時計算用アルファ値
    float _fadeSeconds = 1f;
    /// <summary>
    /// フェード時間
    /// </summary>
    public float FadeSeconds
    {
        get { return _fadeSeconds; }
        set { _fadeSeconds = value; }
    }

    //フェードイン関連
    bool _isFadeInFinish = false;
    /// <summary>
    /// フェードイン終わったか？
    /// </summary>
    public bool IsFadeInFinish
    {
        get { return _isFadeInFinish; }
    }
    
    //フェードアウト関連
    bool _isFadeOutFinish = false;
    /// <summary>
    /// フェードアウト終わったか？
    /// </summary>
    public bool IsFadeOutFinish
    {
        get { return _isFadeOutFinish; }
    }

	// Use this for initialization
	void Start ()
    {
        //Imageコンポーネント取得
        fadeImage = this.GetComponent<Image>();
        //初期色を黒に設定
        fadeImage.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, 255);

    }

    /// <summary>
    /// フェードイン
    /// </summary>
    public void FadeIn()
    {
        //フェードイン終わってなかったら
        if (!_isFadeInFinish)
        {
            //アルファ値減算
            _fadeAlphaValue -= (255 / _fadeSeconds * 60f) * (Time.deltaTime / 60f);
            //代入
            fadeImage.color = new Vector4(Color.black.r / 255f,
                                      Color.black.g / 255f,
                                      Color.black.b / 255f,
                                      _fadeAlphaValue / 255f);
            if (_fadeAlphaValue <= 0)
            {
                _fadeAlphaValue = 0;
                //フェードイン終わった
                _isFadeInFinish = true;
            }
        }
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public void FadeOut()
    {
        //フェードアウト終わってなかったら
        if (!_isFadeOutFinish)
        {
            //アルファ値加算
            _fadeAlphaValue += (255 / _fadeSeconds * 60f) * (Time.deltaTime / 60f);
            //代入
            fadeImage.color = new Vector4(Color.black.r / 255f,
                                      Color.black.g / 255f,
                                      Color.black.b / 255f,
                                      _fadeAlphaValue / 255f);
            if (_fadeAlphaValue >= 255)
            {
                //DOTween全削除
                DOTween.KillAll();
                _fadeAlphaValue = 255;
                //フェードアウト終わった
                _isFadeOutFinish = true;
            }
        }
    }

}
