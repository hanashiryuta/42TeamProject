﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeController : MonoBehaviour
{
    Image image;
    float _tmpValue = 0;
    public float seconds;
    bool _isFadeFinish = false;
    public bool IsFadeFinish
    {
        get { return _isFadeFinish; }
    }

	// Use this for initialization
	void Start () {
        image = this.GetComponent<Image>();
        image.color = new Vector4(Color.black.r, Color.black.g, Color.black.b, 0);

    }

    public void ChangeAlpha() {
        //image.color.a = alpha;

        _tmpValue += (255 / seconds * 60f) * (Time.deltaTime / 60f);

        image.color = new Vector4(Color.black.r / 255f,
                                  Color.black.g / 255f, 
                                  Color.black.b / 255f, 
                                  _tmpValue / 255f);
        if (_tmpValue >= 255)
        {
            _tmpValue = 255;
            _isFadeFinish = true;
        }
    }

    // Update is called once per frame
    void Update () {
	    
	}
}
