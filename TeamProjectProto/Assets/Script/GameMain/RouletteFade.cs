//
//作成日時：6月22日
//ルーレット背景用フェード
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// フェード状態
/// </summary>
public enum RouletteFadeState
{
    START,//開始
    END,//終了
}

public class RouletteFade : MonoBehaviour {

    Image fadeImage;//フェード画像
    [HideInInspector]
    public bool isEnd;//終了か
    float alpha = 0;//α値
    RouletteFadeState rouletteFadeState = RouletteFadeState.START;//フェード状態
    Color color;//α値変更用カラー

	// Use this for initialization
	void Start () {
        fadeImage = GetComponent<Image>();
        color = fadeImage.color;//カラー退避
    }
	
	// Update is called once per frame
	void Update ()
    {
        //α値設定
        color.a = alpha;
        //カラー設定
        fadeImage.color = color;

        //状態によって処理変更
        switch(rouletteFadeState)
        { 
            case RouletteFadeState.START://開始
                //α値増加
                if (alpha <= 0.6f)
                    alpha += 0.03f;
                if (isEnd)
                    rouletteFadeState = RouletteFadeState.END;//状態遷移
                break;
            case RouletteFadeState.END://終了
                alpha -= 0.03f;//α値減少
                if (alpha <= 0)
                    Destroy(gameObject);//削除
                break;
        }
	}
}
