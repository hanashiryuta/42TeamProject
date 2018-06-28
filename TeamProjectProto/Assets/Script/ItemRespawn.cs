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

    public float setRespawnTiem = 0.0f;
    float respawnTime;//生成間隔
    GameObject item;//内容物

    GameObject itemRespawnLimit; //アイテムのリスポーン上限管理オブジェクト

    StartCountDown startCntDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script
    
    // Use this for initialization
    void Start () {
        transform.tag = "Pausable";//タグ設定
        itemRespawnLimit = GameObject.Find("ItemRespawnLimit(Clone)");
        respawnTime = 0;
        startCntDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();
    }

    // Update is called once per frame
    void Update () {
        //内容物がなければ
        // 追加：アイテム上限以上の場合は生成できない
        //respawnTime -= Time.deltaTime;
        if (item == null && itemRespawnLimit.GetComponent<ItemRespawnLimit>().isRespawn()) {
            respawnTime -= Time.deltaTime;
            if (respawnTime < 0 && !startCntDown.IsCntDown && !finishCall.IsCalling) {
                item = Instantiate(itemList[Random.Range(0, itemList.Count)], transform.position + new Vector3(0,0.5f,0), Quaternion.Euler(90,0,0), transform);//内容物を生成する
                respawnTime = setRespawnTiem;

                itemRespawnLimit.GetComponent<ItemRespawnLimit>().Count();　//アイテム生成数をカウント
            }
        }
	}
}
