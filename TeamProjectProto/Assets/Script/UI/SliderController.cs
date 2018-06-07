/*
 * 作成日時：180529
 * スライダーコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    PlayerMove player;

    //ダッシュゲージ関連
    [SerializeField]
    GameObject dashSliderOBJ;//ダッシュ用スライダー
    Slider dashSlider;
    RectTransform dashSliderTfm;
    CanvasGroup dashCanvas;
    [SerializeField]
    Vector3 dashOffset = new Vector3(0, -20f, 0);
    Text countDownText;

	// Use this for initialization
	void Start ()
    {
        //PlayerMove
        player = transform.GetComponent<PlayerMove>();

        //DashSlider
        GameObject ds = Instantiate(dashSliderOBJ);//生成
        ds.name = player.name + ds.name;//対応プレイヤーの名前を付ける
        ds.transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);//キャンバスに移る
        dashCanvas = ds.transform.GetComponent<CanvasGroup>();//キャンバスグループ(表示用)
        countDownText = ds.GetComponentInChildren<Text>();
        dashSlider = ds.transform.GetComponent<Slider>();

        //DashSliderTransform
        dashSliderTfm = dashSlider.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        ////ダッシュゲージ表示
        //if (player.IsDash) dashCanvas.alpha = 1;
        //else dashCanvas.alpha = 0;

        //ダッシュゲージ位置設定
        SetSliderPosition(dashSliderTfm, dashOffset);
        //ダッシュゲージ値設定
        SetSliderValue(dashSlider, player.DashLimitTime, player.DashCountDown);
        //カウントダウンテキスト
        countDownText.text = player.DashCountDown.ToString("00.0");
	}

    /// <summary>
    /// ゲージ位置設定
    /// </summary>
    /// <param name="slider">ゲージ</param>
    /// <param name="offset"></param>
    void SetSliderPosition(RectTransform slider, Vector3 offset)
    {
        slider.position
            = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position + offset);
    }
    /// <summary>
    /// ダッシュゲージ値設定
    /// </summary>
    /// <param name="slider">ゲージ</param>
    /// <param name="limitValue">上限</param>
    /// <param name="currentValue">現在の値</param>
    void SetSliderValue(Slider slider, float limitValue, float currentValue)
    {
        slider.maxValue = limitValue;
        slider.value = currentValue;
    }
}
