//
//作成日時：5月14日
//Item生成の上限管理クラス
//作成者：安部崇寛
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRespawnLimit : MonoBehaviour {

    public int ItemLimit; //Itemの生成上限
    public int ItemCount = 0; //Itemの生成数のカウント

    GameObject postRespawn;

    private void Start() {
        ItemCount = 0;
        postRespawn = GameObject.Find("PostRespawnPoint");
    }

    // Update is called once per frame
    void Update () {
        if (postRespawn.GetComponent<PostRespawn>().isLimit()) {
            ItemCount = 0;
            postRespawn.GetComponent<PostRespawn>().isLimitReset = false;
        }
	}

    //生成したアイテムをItemRespawn.csでカウントするためのメソッド
    public void Count() {
        ItemCount++;//
    }

    //ItemRespawn.csで上限以上生成できないようにする時の判定用メソッド
    public bool isRespawn() {
        return ItemCount < ItemLimit;
    }
}
