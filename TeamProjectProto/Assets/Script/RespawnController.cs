//
//作成日時：4月16日
//プレイヤーのリスポーン処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour {

    public GameObject player;//プレイヤー
    List<GameObject> playerList;//プレイヤーリスト

	// Use this for initialization
	void Start () {
        playerList = new List<GameObject>();

        //生成位置を数だけプレイヤーを生成する
        for (int i = 0; i < transform.childCount; i++) 
        {
            GameObject p = Instantiate(player, transform.GetChild(i).transform.position,Quaternion.identity);//プレイヤー生成
            playerList.Add(p);//リストに追加
            p.name = "Player" + (i + 1);//名前変更
            p.GetComponent<PlayerMove>().horizontal = "Horizontal" + (i + 1);//そのプレイヤーの使うInput指定
            p.GetComponent<PlayerMove>().vertical = "Vertical" + (i + 1);//そのプレイヤーの使うInput指定
            p.GetComponent<PlayerMove>().jump = "Jump" + (i + 1);//そのプレイヤーの使うInput指定
        }
	}
	
	// Update is called once per frame
	void Update () {
       
	}
}
