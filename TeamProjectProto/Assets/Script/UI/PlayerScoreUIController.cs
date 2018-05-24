/*
 * 作成日時：180523
 * プレイヤーのスコアUI関連処理
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerScoreUIController : MonoBehaviour
{
    [SerializeField]
    GameObject[] playerScoreUI; // player UI prefabs

    [SerializeField]
    RectTransform[] UIposition;

    [SerializeField]
    GameObject[] pList;

    // Use this for initialization
    void Start ()
    {
        pList = GameObject.FindGameObjectsWithTag("Player");//プレイ
    }
	
	// Update is called once per frame
	void Update ()
    {
        SetPlayerRank();

        //SetRankUIPosition();
        SetRankUIPositionWithAnim();

    }

    /// <summary>
    /// ランクに合わせてUIの位置を変更
    /// </summary>
    void SetRankUIPosition()
    {
        for (int i = 0; i < pList.Length; i++)
        {
            Vector3 tmp = Vector3.zero;

            switch (pList[i].name) //Rank
            {
                case "Player1":
                    playerScoreUI[0].transform.position = UIposition[i].position;
                    break;
                case "Player2":
                    playerScoreUI[1].transform.position = UIposition[i].position;
                    break;
                case "Player3":
                    playerScoreUI[2].transform.position = UIposition[i].position;
                    break;
                case "Player4":
                    playerScoreUI[3].transform.position = UIposition[i].position;
                    break;
            }
        }
    }

    /// <summary>
    /// ランクに合わせてUIの位置を変更(Anim)
    /// </summary>
    void SetRankUIPositionWithAnim()
    {
        for (int i = 0; i < pList.Length; i++)
        {
            Vector3 tmp = Vector3.zero;

            switch (pList[i].name) //Rank
            {
                case "Player1":
                    DOTween.To
                        (
                            () => playerScoreUI[0].transform.position,
                            (x) => playerScoreUI[0].transform.position = x,
                            UIposition[i].position,
                            1f
                        );
                    break;
                case "Player2":
                    DOTween.To
                        (
                            () => playerScoreUI[1].transform.position,
                            (x) => playerScoreUI[1].transform.position = x,
                            UIposition[i].position,
                            1f
                        );
                    break;
                case "Player3":
                    DOTween.To
                        (
                            () => playerScoreUI[2].transform.position,
                            (x) => playerScoreUI[2].transform.position = x,
                            UIposition[i].position,
                            1f
                        );
                    break;
                case "Player4":
                    DOTween.To
                        (
                            () => playerScoreUI[3].transform.position,
                            (x) => playerScoreUI[3].transform.position = x,
                            UIposition[i].position,
                            1f
                        );
                    break;
            }
        }
    }


    /// <summary>
    /// プレイヤーのランク付け
    /// </summary>
    void SetPlayerRank()
    {
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
    }
}
