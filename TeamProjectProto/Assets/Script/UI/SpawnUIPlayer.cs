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
    List<GameObject> positionOBJ = new List<GameObject>();//生成位置
    public List<GameObject> PositionOBJ
    {
        get { return positionOBJ; }
        set { positionOBJ = value; }
    }

    bool iscreated = false;//生成したか？
    List<string> pList;//プレイヤーリスト
    public List<string> PList
    {
        set { pList = value; }
    }

    List<int> rankList;//順位リスト
    public List<int> RankList
    {
        set { rankList = value; }
    }
    //順位のプレイヤースケール
    List<float> rankScaleSize = new List<float>() { 0.7f, 0.5f, 0.4f, 0.3f };

    // Update is called once per frame
    void Update ()
    {
        if (!iscreated)
        {
            SpawnPlayerCharacter();

            //プレイヤーが生成し終わったら受け取りオブジェを削除
            //「もう一回」でステージ選択シーンに移行するので廃止
            //connectedPlayerStatus.Created = false;
            //Destroy(connectedPlayerStatus.transform.gameObject);

            iscreated = true;
        }
        else
        {
            //位置合わせ
            for(int i = 0; i < connectedPlayerStatus.ConnectedPlayer.Count; i++)
            {
                player[i].transform.position =
                    new Vector3(Camera.main.ScreenToWorldPoint(positionOBJ[i].transform.position).x,
                                Camera.main.ScreenToWorldPoint(positionOBJ[i].transform.position).y,
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

            //スキン変更
            player[i].GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = tex[connectedPlayerStatus.ConnectedPlayer[pList[i]]];
            //順位に応じてスケール調整
            player[i].transform.localScale = new Vector3(rankScaleSize[i], rankScaleSize[i], rankScaleSize[i]);

            //一位だったら
            if (rankList[i] == 1)
            {
                //王冠を表示
                player[i].transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/Bone.004_end/Crown").gameObject.SetActive(true);
            }
        }
    }
}
