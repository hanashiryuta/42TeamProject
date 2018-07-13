/*
 * 作成日時：180518
 * カメラを揺らすクラス
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    [SerializeField]
    private float shakeSeconds = 1f;//揺れの継続時間
    [SerializeField]
    private Vector3 shakeValue = new Vector3(5, 5, 0);//揺れ具合
    private bool isShaked = false;//揺れ始めたか？
    private bool currentIsShake = false;//今揺れているか？
    private bool previousIsShake = false;//先揺れているか？

    BalloonMaster _balloonM;//風船総合管理クラス
    public BalloonMaster BalloonM
    {
        get { return _balloonM; }
        set { _balloonM = value; }
    }

    // Use this for initialization
    void Start ()
    {
        previousIsShake = currentIsShake;
    }

    // Update is called once per frame
    void Update ()
    {
        //風船の爆発状態を取得
        currentIsShake = BalloonM.IsBlast;

        //爆発の瞬間
        if (currentIsShake == true &&
            previousIsShake == false) 
        {
            //揺れる判定に入った
            isShaked = true;
        }

        previousIsShake = currentIsShake;

        if (isShaked)
        {
            //揺らす
            Shake();
            isShaked = false;
        }
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    /// <param name="time">秒数</param>
    /// <param name="value">揺らす度合い</param>
    public void Shake()
    {
        DOTween.Shake(() => Camera.main.transform.position,
                        x => Camera.main.transform.position = x,
                        shakeSeconds, 
                        shakeValue); 

        BalloonM.IsBlast = false;//爆発によって揺れ始めたので爆発状態をfalseに
    }
}
