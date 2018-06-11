/*
 * 作成日時：180611
 * UI用プレイヤー生成
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnUIPlayer : MonoBehaviour
{
    ConnectedPlayerStatus connectedPlayerStatus;//接続したプレイヤー(ResultManagerから)
    public ConnectedPlayerStatus ConnectedPLStatus
    {
        set { connectedPlayerStatus = value; }
    }

    GameObject[] player = new GameObject[4];
    [SerializeField]
    GameObject playerPrefab;//キャラプレハブ
    [SerializeField]
    Texture[] tex;//テクスチャ
    [SerializeField]
    GameObject[] positionOBJ;//生成位置

    bool iscreated = false;//生成したか？
    List<string> pList;//プレイヤーリスト
    public List<string> PList
    {
        set { pList = value; }
    }


    // Update is called once per frame
    void Update ()
    {
        if (!iscreated)
        {
            SpawnPlayerCharacter();

            //プレイヤーが生成し終わったら受け取りオブジェを削除
            Destroy(connectedPlayerStatus.transform.gameObject);

            iscreated = true;
        }
        else
        {
            //位置合わせ
            foreach (var cntPlSta in connectedPlayerStatus.ConnectedPlayer)
            {
                player[cntPlSta.Value].transform.position =
                    new Vector3(Camera.main.ScreenToWorldPoint(positionOBJ[cntPlSta.Value].transform.position).x,
                                Camera.main.ScreenToWorldPoint(positionOBJ[cntPlSta.Value].transform.position).y - 1,
                                0);
            }
        }

    }

    /// <summary>
    /// プレイヤーキャラスポーン
    /// </summary>
    void SpawnPlayerCharacter()
    {
        for (int i = 0; i < connectedPlayerStatus.ConnectedPlayer.Count; i++)
        {
            player[i] = GameObject.Instantiate(
                                    playerPrefab,
                                    Camera.main.ScreenToWorldPoint(positionOBJ[i].transform.position),
                                    Quaternion.Euler(0, 180, 0));

            player[i].GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = tex[connectedPlayerStatus.ConnectedPlayer[pList[i]]];//テクスチャ変更

            //一位だったら
            if (i == 0)
            {
                //拡大
                player[i].transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}
