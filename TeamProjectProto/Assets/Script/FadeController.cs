using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeController : MonoBehaviour
{
    Image image;
    float _tmpValue = 255;
    public float seconds;
    bool _isFadeInFinish = false;
    public bool IsFadeInFinish
    {
        get { return _isFadeInFinish; }
    }
    bool _isFadeOutFinish = false;
    public bool IsFadeOutFinish
    {
        get { return _isFadeOutFinish; }
    }

	// Use this for initialization
	void Start ()
    {
        image = this.GetComponent<Image>();
        image.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, 255);

    }

    public void FadeOut()
    {
        if (!_isFadeOutFinish)
        {
            _tmpValue += (255 / seconds * 60f) * (Time.deltaTime / 60f);

            image.color = new Vector4(Color.black.r / 255f,
                                      Color.black.g / 255f,
                                      Color.black.b / 255f,
                                      _tmpValue / 255f);
            if (_tmpValue >= 255)
            {
                //DOTween全削除
                DOTween.KillAll();
                _tmpValue = 255;
                _isFadeOutFinish = true;
            }
        }
    }

    public void FadeIn()
    {
        if (!_isFadeInFinish)
        {
            _tmpValue -= (255 / seconds * 60f) * (Time.deltaTime / 60f);

            image.color = new Vector4(Color.black.r / 255f,
                                      Color.black.g / 255f,
                                      Color.black.b / 255f,
                                      _tmpValue / 255f);
            if (_tmpValue <= 0)
            {
                _tmpValue = 0;
                _isFadeInFinish = true;
            }
        }
    }

    // Update is called once per frame
    void Update () {
	    
	}
}
