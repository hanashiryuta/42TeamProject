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

	// Use this for initialization
	void Start () {
        respawnTime = 3.0f;

        item = Instantiate(itemList[Random.Range(0,itemList.Count)], transform.position, Quaternion.identity, transform);//内容物生成
	}
	
	// Update is called once per frame
	void Update () {
        //内容物がなければ
        if (item == null)
        {
            respawnTime -= Time.deltaTime;
            if (respawnTime < 0)
            {
                item = Instantiate(itemList[Random.Range(0, itemList.Count)], transform.position, Quaternion.identity, transform);//内容物を生成する
                respawnTime = 3.0f;
            }
        }
	}
}
