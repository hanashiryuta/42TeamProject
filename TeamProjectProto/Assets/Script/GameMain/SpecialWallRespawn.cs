//
//作成日時：4月27日
//特殊壁生成処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialWallRespawn : MonoBehaviour {

    public GameObject originSpecialWall;//特殊壁
    List<GameObject> childList;//生成場所リスト

    void Start()
    {
        //生成場所リスト生成
        childList = new List<GameObject>();
        for(int i = 0;i<transform.childCount;i++)
        {
            childList.Add(transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 特殊壁生成処理
    /// </summary>
    /// <param name="player">プレイヤー</param>
    public void SpecialRespawn(GameObject player)
    {
        //まだ生成場所が残っているなら
        if (childList.Count > 0)
        {
            int rand = Random.Range(0, childList.Count);

            //特殊壁生成
            GameObject specialWall = Instantiate(originSpecialWall, childList[rand].transform.position, Quaternion.identity, transform.GetChild(rand).transform);

            //生成場所減少
            childList.RemoveAt(rand);

            //生成したプレイヤーの名前を追加
            specialWall.name += player.name;
            //プレイヤーの色に変更
            specialWall.GetComponent<Renderer>().material.color = player.GetComponent<Renderer>().material.color;
        }
    }
}
