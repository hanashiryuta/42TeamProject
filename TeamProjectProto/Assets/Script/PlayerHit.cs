//
//作成日時：5月19日
//プレイヤーあたり判定
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour {

    PlayerMove playerMove;//プレイヤー管理クラス

	// Use this for initialization
	void Start () {
        playerMove = GetComponentInParent<PlayerMove>();//プレイヤー管理クラス取得
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider col)
    {
        //内容物に当たったら
        if (col.gameObject.name.Contains("PointItem"))
        {
            if (col.gameObject.GetComponent<ItemController>().isGet && playerMove.balloon == null&&!playerMove.isStan)
            {
                col.gameObject.GetComponent<ItemController>().Item_Death_Particle();//アイテム取得時パーティクル生成
                playerMove.itemList.Add(col.name);//リスト追加
                Destroy(col.gameObject);//内容物破棄
                playerMove.holdItemCount += col.GetComponent<ItemController>().point; //内容物所持数を増やす  
                GetComponentInParent<AudioSource>().PlayOneShot(playerMove.soundSE2);
                //totalBlastCount += col.GetComponent<ItemController>().point;//内容物所持数累計を増やす
            }
        }
    }
}
