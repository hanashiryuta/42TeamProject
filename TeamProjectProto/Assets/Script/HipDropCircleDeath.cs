//
//作成日時：5月7日
//衝撃波削除メソッド
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipDropCircleDeath : MonoBehaviour {

    public float deathTime = 0.5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //時間が来たら死亡
        deathTime -= Time.deltaTime;
        if(deathTime <= 0)
        {
            Destroy(gameObject);
        }
	}
}
