//
//作成日時：6月4日
//プレイヤージャンプ時のあたり判定
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpHit : MonoBehaviour {

    public bool isJumpHit = false;//地面と当たっているか
    public Rigidbody rigid;//リジッドボディ
    
    void Start()
    {
    }
    
    // Update is called once per frame
	void Update () {
        //下方向で当たっているもの取得
        RaycastHit[] colArray = rigid.SweepTestAll(-transform.up, 0.01f);

        foreach (var cx in colArray)
        {
            //当たっているものが床か、プレイヤーかだったら
            if ((cx.transform.tag == "Field") || (cx.transform.tag == "Player" && cx.transform.gameObject != gameObject))
            {
                //当たっている判定true
                isJumpHit = true;
                return;
            }
        }
        isJumpHit = false;
    }
}
