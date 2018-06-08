//
//作成日時：5月22日
//ゲーム各値取得メソッド
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMaster : MonoBehaviour {

    Dictionary<string, GameObject> gameObjectMaster;//オブジェクト管理ディクショナリ―

    Dictionary<string, float> gameParameter;//パラメーター管理ディクショナリ―

	// Use this for initialization
	void Start () {
        //初期化
        gameObjectMaster = new Dictionary<string, GameObject>();
        gameParameter = new Dictionary<string, float>();

        //プレイヤーリスト取得
        GameObject[] playerList = GameObject.FindGameObjectsWithTag("Player");

        for(int i = 0; i < 4; i++)
        {
            //オブジェクトディクショナリ―にプレイヤー追加
            gameObjectMaster.Add(playerList[i].name, playerList[i]);
        }

        //風船取得
        GameObject balloon = GameObject.FindGameObjectWithTag("Balloon");

        //風船追加
        gameObjectMaster.Add("Balloon", balloon);

        //時間取得
        GameObject timeController = GameObject.Find("TimeController");

        //時間追加
        gameObjectMaster.Add("TimeController", timeController);

        /*
        ・gameParameterの中身（この名前で取り出せる）
        Player1ポイント
        Player1総ポイント
        Player2ポイント
        Player2総ポイント
        Player3ポイント
        Player3総ポイント
        Player4ポイント
        Player4総ポイント
        Balloon大きさ
        Balloon大きさ限界
        Balloonポイント
        Balloonポイント限界
        制限時間
        */

        //プレイヤーのパラメーターを追加
        for(int i = 1; i < 5; i++)
        {
            gameParameter.Add("Player" + i + "ポイント", gameObjectMaster["Player" + i].GetComponent<PlayerMove>().holdItemCount);
            gameParameter.Add("Player" + i + "総ポイント", gameObjectMaster["Player" + i].GetComponent<PlayerMove>().totalItemCount);
        }


        //制限時間を取得
        gameParameter.Add("制限時間", gameObjectMaster["TimeController"].GetComponent<TimeController>().gameTime);
    }
	
	// Update is called once per frame
	void Update () {

        //パラメーター更新
        for(int i = 1; i < 5; i++)
        {
            gameParameter["Player" + i + "ポイント"] = gameObjectMaster["Player" + i].GetComponent<PlayerMove>().holdItemCount;
            gameParameter["Player" + i + "総ポイント"] = gameObjectMaster["Player" + i].GetComponent<PlayerMove>().totalItemCount;
        }

        if (gameObjectMaster["Balloon"] == null)
        {
            gameObjectMaster["Balloon"] = GameObject.FindGameObjectWithTag("Balloon");
        }
        else
        {
            //風船のパラメーターを追加
            if (!gameParameter.ContainsKey("Balloon大きさ"))
                gameParameter.Add("Balloon大きさ", gameObjectMaster["Balloon"].GetComponent<BalloonOrigin>().scaleCount);
            if (!gameParameter.ContainsKey("Balloon大きさ限界"))
                gameParameter.Add("Balloon大きさ限界", gameObjectMaster["Balloon"].GetComponent<BalloonOrigin>().scaleLimit);
            if (!gameParameter.ContainsKey("Balloonポイント"))
                gameParameter.Add("Balloonポイント", gameObjectMaster["Balloon"].GetComponent<BalloonOrigin>().blastCount);
            if (!gameParameter.ContainsKey("Balloonポイント限界"))
                gameParameter.Add("Balloonポイント限界", gameObjectMaster["Balloon"].GetComponent<BalloonOrigin>().blastLimit);

            gameParameter["Balloon大きさ"] = gameObjectMaster["Balloon"].GetComponent<BalloonOrigin>().scaleCount;
            gameParameter["Balloonポイント"] = gameObjectMaster["Balloon"].GetComponent<BalloonOrigin>().blastCount;
        }
        gameParameter["制限時間"] = gameObjectMaster["TimeController"].GetComponent<TimeController>().gameTime;

        //表示
        //foreach(var cx in gameParameter)
        //{
        //    if (cx.Key != "制限時間")
        //    {
        //        Debug.Log(cx.Key + ":" + cx.Value);
        //    }
        //}
    }
}
