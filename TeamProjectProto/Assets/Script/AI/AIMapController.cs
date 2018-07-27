//
//作成日時：7月17日
//AI用影響マップ作製準備クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AIMapController : MonoBehaviour
{
    GameSetting gameSetting;//ゲームセッティング
    GameObject nowStage;//現在のステージ

    [HideInInspector]
    public float mapWidth;//マップ横幅
    [HideInInspector]
    public float mapHeight;//マップ縦幅

    [HideInInspector]
    public string[][] stageArray;//ステージセルリスト
    [HideInInspector]
    public Vector3[][] positionArray;//セルポジションリスト
    [HideInInspector]
    public float[][] postInfluenceMap;//ポストの影響マップ
    [HideInInspector]
    public float[][] balloonInfluenceMap;//バルーン持ってるやつの影響マップ

    // Use this for initialization
    void Awake () {
        //ゲームセッティング設定
        gameSetting = GameObject.Find("GameController").GetComponent<GameSetting>();
        //現在のステージ設定
        nowStage = gameSetting.gameObjectList[0];
        //ステージセル作成
        stageArray = ArraySet();
        //ステージ縦幅設定
        mapHeight = stageArray.Length;
        //ステージ横幅設定
        mapWidth = stageArray[0].Length;
        //各配列初期化処理
        positionArray = new Vector3[(int)mapHeight][];
        postInfluenceMap = new float[(int)mapHeight][];
        balloonInfluenceMap = new float[(int)mapHeight][];

        //全セル判定
        for (int i = 0; i < mapHeight; i++)
        {
            //各二重配列初期化
            positionArray[i] = new Vector3[(int)mapWidth];
            postInfluenceMap[i] = new float[(int)mapWidth];
            balloonInfluenceMap[i] = new float[(int)mapWidth];

            for (int j = 0;j< mapWidth; j++)
            {
                //ステージのセルをひとつづつ確認し、
                //セルに設定されたオブジェクトごとにポジション設定

                if (stageArray[i][j] == "-1")//プレイヤー進入禁止エリア
                    positionArray[i][j] = new Vector3(-(mapWidth / 2 - 0.5f) + j, 1 - 3, mapHeight / 2 - 0.5f - i);
                else if (stageArray[i][j] == "0")//床
                    positionArray[i][j] = new Vector3(-(mapWidth / 2 - 0.5f) + j, 0 - 3, mapHeight / 2 - 0.5f - i);
                else if (stageArray[i][j] == "1")//低い壁
                    positionArray[i][j] = new Vector3(-(mapWidth / 2 - 0.5f) + j, 1.5f - 3, mapHeight / 2 - 0.5f - i);
                else if (stageArray[i][j] == "2")//高い壁
                    positionArray[i][j] = new Vector3(-(mapWidth / 2 - 0.5f) + j,2.5f - 3, mapHeight / 2 - 0.5f - i);

                //各影響マップ初期化
                postInfluenceMap[i][j] = 0;
                balloonInfluenceMap[i][j] = 0;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        //各影響マップ作製
        MapSet();
    }

    /// <summary>
    /// ステージセル設定
    /// </summary>
    /// <returns>ステージセル配列</returns>
    string[][] ArraySet()
    {
        //現在のステージに対応したcsv読み込み
        StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/StageCSV/" + nowStage.name + ".csv");
        //リスト作成
        List<string[]> sList = new List<string[]>();
        //リストに各値収納
        while (sr.EndOfStream == false)
        {
            string line = sr.ReadLine();
            sList.Add(line.Split(','));
        }
        sr.Close();
        //リスト返す
        return sList.ToArray();
    }

    /// <summary>
    /// 影響マップ作製処理
    /// </summary>
    void MapSet()
    {
        //全セル判定
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {        
                //各影響マップの値初期化
                postInfluenceMap[i][j] = 0;
                balloonInfluenceMap[i][j] = 0;
            }
        }

        //ポストの影響マップ作製
        foreach (var post in GameObject.FindGameObjectsWithTag("Post"))
        {
            ObjectMapSet(post, false, postInfluenceMap);
        }

        //バルーン持っているプレイヤーの影響マップ作製
        foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PlayerMove>().balloon != null)
            {
                ObjectMapSet(player, true, balloonInfluenceMap);
            }
        }
    }

    /// <summary>
    /// 各オブジェクト影響マップ作成
    /// </summary>
    /// <param name="obj">判定オブジェクト</param>
    /// <param name="isReturn">影響値を反転させるかどうか</param>
    /// <param name="objInfluenceMap">各影響マップ</param>
    void ObjectMapSet(GameObject obj, bool isReturn, float[][] objInfluenceMap)
    {
        //全セル判定
        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                //オブジェクトとセルの距離判定
                float length = Vector3.Distance(positionArray[i][j], obj.transform.position);

                float influence = 0;//影響値

                //距離に応じた影響値を設定
                if (isReturn)
                    //反転させるなら
                    //近いほど影響値低く設定
                    influence = length / 20;
                else
                    //反転させないなら
                    //近いほど影響値高く設定
                    influence = 1 - length / 20;

                //影響値の範囲を0から1の間で設定
                if (influence < 0)
                    influence = 0;
                else if (influence > 1)
                    influence = 1;

                //取り出したセルの影響値より求めた影響値が高ければ
                if (objInfluenceMap[i][j] < influence)
                {
                    //影響値更新
                    objInfluenceMap[i][j] = influence;
                }
            }
        }
    }
}
