/*
 * 作成日時：180213
 * CRTコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRTController : MonoBehaviour {

    [SerializeField]
    Camera mainCamera;
    BalloonController balloonController;

    float cnt = 0;      //カウンター
    [SerializeField]
    bool isCnt = false; //カウントしているか？
    public float effectSeconds = 1; //CRT実行秒数

    void Start()
    {
        balloonController = GameObject.FindGameObjectWithTag("Balloon").GetComponent<BalloonController>();
    }

    // Update is called once per frame
    void Update()
    {
        StartCnt();

        if (isCnt)
        {
            cnt += 1 * Time.deltaTime;
        }

        //時間が達したら
        if (cnt >= effectSeconds)
        {
            //コンポーネントをOFF
            mainCamera.GetComponent<CRT>().enabled = false;
            //初期化
            cnt = 0;
            isCnt = false;
        }
    }

    /// <summary>
    /// CRT開始
    /// </summary>
    private void StartCnt()
    {
        //風船爆発したら
        if(balloonController.IsBlast)
        {
            //コンポーネントをON
            mainCamera.GetComponent<CRT>().enabled = true;
            isCnt = true; ;
        }
    }
}
