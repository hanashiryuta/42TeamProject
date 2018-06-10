/*
 * 作成日時：180518
 * カメラを揺らす
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
    private bool isShaked = false;//
    private bool currentIsShake = false;
    private bool previousIsShake = false;

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
        currentIsShake = BalloonM.IsBlast;

        if (currentIsShake == true &&
            previousIsShake == false) 
        {
            isShaked = true;
        }

        previousIsShake = currentIsShake;

        if (isShaked)
        {
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

        BalloonM.IsBlast = false;
    }
}
