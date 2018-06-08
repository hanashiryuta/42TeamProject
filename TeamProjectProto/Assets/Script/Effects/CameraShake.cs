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

    BalloonOrigin balloonController;
    GameObject balloonControllerObject;

    // Use this for initialization
    void Start ()
    {
    }

    // Update is called once per frame
    void Update ()
    {
        if (balloonControllerObject == null)
        {
            balloonControllerObject = GameObject.FindGameObjectWithTag("Balloon");
            return;
        }
        balloonController = balloonControllerObject.GetComponent<BalloonOrigin>();
        //Shake(shakeSeconds, shakeValue);
    }

    /// <summary>
    /// カメラを揺らす
    /// </summary>
    /// <param name="time">秒数</param>
    /// <param name="value">揺らす度合い</param>
    public void Shake()
    {
        if (balloonController.IsBlast)
        {
            isShake = true;
            Camera.main.DOShakePosition(shakeSeconds, shakeValue);
            balloonController.IsBlast = false;
        }
    }
}
