//
//作成日時：5月23日
//一位に王冠付けるメソッド
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownManager : MonoBehaviour {

    GameObject[] pList;//プレイヤーリスト

    // Use this for initialization
    void Start() {
        pList = GameObject.FindGameObjectsWithTag("Player");//プレイヤーリスト取得
    }
	
	// Update is called once per frame
	void Update () {
        //ソート（大きい順に）
        for (int i = 0; i < pList.Length - 1; i++)
        {
            for (int j = i + 1; j < pList.Length; j++)
            {
                if (pList[i].GetComponent<PlayerMove>().totalBlastCount < pList[j].GetComponent<PlayerMove>().totalBlastCount)
                {
                    GameObject p = pList[j];
                    pList[j] = pList[i];
                    pList[i] = p;
                }
            }
        }

        foreach(var player in pList)
        {
            //1位の王冠を見えるようにする
            //1位タイも王冠を見えるようにする
            if(player.GetComponent<PlayerMove>().totalBlastCount >= pList[0].GetComponent<PlayerMove>().totalBlastCount)
            {
                player.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/Bone.004_end/Crown").GetComponent<MeshRenderer>().enabled = true;
            }
            //それ以外のプレイヤーの王冠は消す
            else
            {
                player.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/Bone.004_end/Crown").GetComponent<MeshRenderer>().enabled = false;
            }
        }

    }
}
