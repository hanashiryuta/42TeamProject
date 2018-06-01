/*
 *作成日時：180522
 *背景のスポーン処理
 *作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSpawn : MonoBehaviour
{
    public GameObject bg;//背景オブジェ
    private Camera mainCam;
    public float zDistance = 20f;

    // Use this for initialization
    void Start ()
    {
        this.mainCam = Camera.main;

        Start_Spawn();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /// <summary>
    /// 背景生成(初期)
    /// </summary>
    void Start_Spawn()
    {
        Vector3 bg_position = new Vector3(Screen.width / 2, Screen.height / 2, zDistance);

        Vector3 worldPoint = mainCam.ScreenToWorldPoint(bg_position);

        GameObject.Instantiate(bg, worldPoint, mainCam.transform.rotation);
    }
}
