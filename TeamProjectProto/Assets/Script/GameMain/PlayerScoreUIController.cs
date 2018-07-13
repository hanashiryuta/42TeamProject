/*
 * 作成日時：180621
 * プレイヤーのスコアUI関連処理
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerScoreUIController : MonoBehaviour
{
    [SerializeField]
    GameObject[] playerScoreUI; // player UI prefabs

    [SerializeField]
    Sprite[] offlineTexs;//不参加テクスチャ

    RespawnController playerRespawn;
    List<GameObject> _pList = new List<GameObject>(); // Player List

    // Use this for initialization
    void Start()
    {
        playerRespawn = GameObject.Find("PlayerRespawns").GetComponent<RespawnController>();
        _pList = playerRespawn.PlayerList;//現在いるプレイヤーのリストをもらう

        SetOfflinePlayerImage();
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// オフラインプレイヤーのUIを変える
    /// </summary>
    void SetOfflinePlayerImage()
    {
        //プレイヤーがオンラインかどうかを格納する
        Dictionary<string, bool> playerOnlineStatus = new Dictionary<string, bool>();
        //最初全員オフライン認定
        for (int i = 1; i <= 4; i++)
        {
            playerOnlineStatus.Add("Player" + i, false);
        }

        //プレイヤーリストにいるならオンライン認定
        List<string> keyList = new List<string>(playerOnlineStatus.Keys);//KeyList
        for (int i = 0;i < _pList.Count; i++)
        {
            foreach(var ps in keyList)
            {
                if(_pList[i].name == ps)
                {
                    playerOnlineStatus[ps] = true;
                }
            }
        }

        //プレイヤーがオフラインだったら
        for(int i = 0; i < playerScoreUI.Length; i++)
        {
            if (playerOnlineStatus["Player" + (i + 1)] == false)
            {
                //表示画像がオフライン用に変わる
                playerScoreUI[i].GetComponent<Image>().sprite = offlineTexs[i];
                //該当するtotalCountオブジェを非表示
                playerScoreUI[i].transform.Find("TotalCountBackGround" + (i + 1)).gameObject.SetActive(false);
            }
        }
    }

}
