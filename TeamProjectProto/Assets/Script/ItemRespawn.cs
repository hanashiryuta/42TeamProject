//
//作成日時：4月18日
//内容物生成処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawn : MonoBehaviour {

    public List<GameObject> itemList;//内容物リスト
    
    float respawnTime = 3.0f;//生成間隔
    GameObject item;//内容物

    GameObject itemRespawnLimit; //アイテムのリスポーン上限管理オブジェクト

	// Use this for initialization
	void Start () {
        itemRespawnLimit = GameObject.Find("ItemRespawnLimit(Clone)");
        respawnTime = 3.0f;

        //初期生成時に上限を超えていれば上限数以上生成できないようにする
        if (itemRespawnLimit.GetComponent<ItemRespawnLimit>().isRespawn()) {
            item = Instantiate(itemList[Random.Range(0, itemList.Count)], transform.position, Quaternion.identity, transform);//内容物生成

            itemRespawnLimit.GetComponent<ItemRespawnLimit>().Count(); //アイテム生成数をカウント
        }
	}
	
	// Update is called once per frame
	void Update () {
        //内容物がなければ
        // 追加：アイテム上限以上の場合は生成できない
        if (item == null && itemRespawnLimit.GetComponent<ItemRespawnLimit>().isRespawn())
        {
            respawnTime -= Time.deltaTime;
            if (respawnTime < 0)
            {
                item = Instantiate(itemList[Random.Range(0, itemList.Count)], transform.position, Quaternion.identity, transform);//内容物を生成する
                respawnTime = 3.0f;

                itemRespawnLimit.GetComponent<ItemRespawnLimit>().Count();　//アイテム生成数をカウント
            }
        }
	}
}
