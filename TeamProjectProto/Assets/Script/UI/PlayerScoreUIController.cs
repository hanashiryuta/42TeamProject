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

    GameObject[] _pList; // Player's Rank List

    [SerializeField]
    GameObject playerRank; //PlayerRankオブジェ

    // Use this for initialization
    void Start()
    {
        _pList = playerRank.GetComponent<PlayerRank>().PlayerRankArray;
    }

    // Update is called once per frame
    void Update()
    {
        SetRankUIPositionWithAnim();
    }

    /// <summary>
    /// ランクに合わせてUIの位置を変更
    /// </summary>
    void SetRankUIPosition()
    {
        for (int i = 0; i < _pList.Length; i++)
        {
            Vector3 tmp = Vector3.zero;

            switch (_pList[i].name) //Rank
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
        for (int i = 0; i < _pList.Length; i++)
        {
            Vector3 tmp = Vector3.zero;

            switch (_pList[i].name) //Rank
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
}
