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
    private float shakeSeconds = 1f;
    [SerializeField]
    private Vector3 shakeValue = new Vector3(1, 1, 0);
    private bool isShake = false;

    BalloonController balloonController;

    // Use this for initialization
    void Start ()
    {
        balloonController = GameObject.FindGameObjectWithTag("Balloon").GetComponent<BalloonController>();
    }

    // Update is called once per frame
    void Update ()
    {
        Shake(shakeSeconds, shakeValue);
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    /// <param name="time">秒数</param>
    /// <param name="value">揺らす度合い</param>
    void Shake(float time, Vector3 value)
    {
        if (balloonController.IsBlast)
        {
            isShake = true;
            Camera.main.DOShakePosition(time, value);
            balloonController.IsBlast = false;
        }
    }
}
