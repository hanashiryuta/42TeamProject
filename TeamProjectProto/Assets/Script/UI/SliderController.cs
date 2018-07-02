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
    GameObject origin_dashSliderOBJ;//ダッシュ用スライダー
    GameObject dashSliderOBJ;
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
        dashSliderOBJ = Instantiate(origin_dashSliderOBJ);//生成
        dashSliderOBJ.name = player.name + dashSliderOBJ.name;//対応プレイヤーの名前を付ける
        dashSliderOBJ.transform.SetParent(GameObject.Find("DashSliders").transform);//キャンバスに移る
        dashCanvas = dashSliderOBJ.transform.GetComponent<CanvasGroup>();//キャンバスグループ(表示用)
        countDownText = dashSliderOBJ.GetComponentInChildren<Text>();
        player.HoldItemCountText = countDownText;//プレイヤー所持アイテム数
        dashSlider = dashSliderOBJ.transform.GetComponent<Slider>();

        //DashSliderTransform
        dashSliderTfm = dashSlider.GetComponent<RectTransform>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //ダッシュゲージ位置設定
        SetSliderPosition(dashSliderTfm, dashOffset);
        //ダッシュゲージ値設定
        SetSliderValue(dashSlider, player.DashLimitTime, player.DashCountDown);
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
    /// ゲージ値設定
    /// </summary>
    /// <param name="slider">ゲージ</param>
    /// <param name="limitValue">上限</param>
    /// <param name="currentValue">現在の値</param>
    void SetSliderValue(Slider slider, float limitValue, float currentValue)
    {
        slider.maxValue = limitValue;
        slider.value = currentValue;
    }

    /// <summary>
    /// 表示を隠す
    /// </summary>
    public void InvisibleSlider()
    {
        dashCanvas.alpha = 0;
    }

}
