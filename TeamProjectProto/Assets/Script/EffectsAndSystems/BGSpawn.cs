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
    public float zDistance = 20f;//ｚ軸距離

    // Use this for initialization
    void Start ()
    {
        Spawn();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /// <summary>
    /// 背景生成
    /// </summary>
    void Spawn()
    {
        //背景位置設定
        Vector3 bg_position = new Vector3(Screen.width / 2, Screen.height / 2, zDistance);
        //ワールド座標に転換
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(bg_position);
        //転換されたワールド座標で生成
        GameObject.Instantiate(bg, worldPoint, Camera.main.transform.rotation);
    }
}
