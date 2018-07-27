//
//作成日時：4月16日
//プレイヤーのリスポーン処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#
using System.Linq;

public class RespawnController : MonoBehaviour {

    public GameObject player;//プレイヤー
    List<GameObject> playerList;//プレイヤーリスト
    public List<GameObject> PlayerList
    {
        get { return playerList; }
    }
    List<Color> colorList;//カラーリスト
    [SerializeField]
    List<Texture> texList;//テクスチャリスト

    public GameObject shadow;//影のオブジェクト

    ConnectedPlayerStatus connectedPlayerStatus;//接続したプレイヤー
    int spawnPoint = 0;//生成ポイントインデックス

    //DEMO
    string preScene;//前のシーン
    public bool isDemo = false;

    // Use this for initialization
    void Awake ()
    {
        //初期化処理
        playerList = new List<GameObject>();
        colorList = new List<Color>()
        {
            new Color(204 / 255f, 0 / 255f, 0 / 255f),//赤
            new Color(15 / 255f, 82 / 255f, 188 / 255f),//青
            new Color(255 / 255f, 255 / 255f, 20 / 255f),//黄色
            new Color(0 / 255f, 255 / 255f, 65 / 255f)//緑
        };

        if(GameObject.FindGameObjectWithTag("PlayerStatus") == null)
        {
            DemoDebugMode();
        }
        else
        {
            SpawnPlayerByStatus();
        }
    }

    /// <summary>
    /// SelectシーンからもらったStatusでキャラ生成
    /// </summary>
    void SpawnPlayerByStatus()
    {
        if (connectedPlayerStatus == null)
        {
            // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }

        //シャッフルする生成位置配列
        int[] defalt_positionAry = new int[] { 0, 1, 2, 3 };
        //シャッフルする
        int[] positionAry = defalt_positionAry.OrderBy(i => System.Guid.NewGuid()).ToArray();


        //接続しているプレイヤー数だけプレイヤーを生成する
        foreach (var cntPlSta in connectedPlayerStatus.ConnectedPlayer)
        {
            GameObject p = Instantiate(player, transform.GetChild(positionAry[spawnPoint]).transform.position, Quaternion.Euler(0,180,0));//プレイヤー生成
            Debug.Log("rndSpawnPoint =" + transform.GetChild(positionAry[spawnPoint]).name);

            playerList.Add(p);//リストに追加
            p.name = cntPlSta.Key;//名前変更

            p.GetComponent<XInputConfig>().playerIndex = (PlayerIndex)cntPlSta.Value; //XInput指定

            //180518 何　追加
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = texList[cntPlSta.Value];//テクスチャ変更
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f));
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].SetTexture("_EmissionMap", texList[cntPlSta.Value]);
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].EnableKeyword("_EMISSION");

            //180508 何　追加　アウトラインの色
            foreach (var outline in p.GetComponentsInChildren<Outline>())
            {
                outline.color = cntPlSta.Value;
            }

            //影をPlayerの子にして生成
            GameObject s = Instantiate(shadow, p.transform.position - Vector3.down, Quaternion.identity, p.transform);

            if (connectedPlayerStatus.IsAIList[cntPlSta.Value])
            {
                int aiMode = Random.Range(1, 3 + 1);//AIMODEをランダムに
                p.GetComponent<PlayerMove>().PlayerAIState = (PlayerState)aiMode;
            }

            //一回生成したポイントで二度生成しないように削除
            Destroy(transform.GetChild(positionAry[spawnPoint]).gameObject);

            spawnPoint++;
        }
    }

    /// <summary>
    /// デバッグ用&デモ用
    /// </summary>
    void DemoDebugMode()
    {
        Debug.Log("PlayerSpawn_Debug");
        if(GameObject.Find("GameController") != null)
        {
            preScene = GameObject.Find("GameController").GetComponent<GameSetting>().preScene;
        }

        //生成位置を数だけプレイヤーを生成する
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject p = Instantiate(player, transform.GetChild(i).transform.position, Quaternion.Euler(0,180,0));//プレイヤー生成
            playerList.Add(p);//リストに追加
            p.name = "Player" + (i + 1);//名前変更
            p.GetComponent<XInputConfig>().playerIndex = (PlayerIndex)i; //XInput指定

            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = texList[i];//テクスチャ変更
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f));
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].SetTexture("_EmissionMap", texList[i]);
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].EnableKeyword("_EMISSION");

            foreach(var outline in p.GetComponentsInChildren<Outline>())
            {
                outline.color = i;
            }

            //影をPlayerの子にして生成
            GameObject s = Instantiate(shadow, p.transform.position - Vector3.down, Quaternion.identity, p.transform);

            //リザルトシーンから直接来たらDEMOのAIに切り替える
            if (preScene == "Title")
            {
                int aiMode = Random.Range(1, 3 + 1);//AIMODEをランダムに
                p.GetComponent<PlayerMove>().PlayerAIState = (PlayerState)aiMode;
            }
        }

        if (preScene == "Title")
        {
            isDemo = true;
        }
    }
}
