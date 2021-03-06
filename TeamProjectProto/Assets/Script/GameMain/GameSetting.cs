﻿//
//作成日時：4月18日
//ゲーム開始時のセット処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour
{

    public List<GameObject> gameObjectList;//ゲーム開始時配置するオブジェクト

    public List<GameObject> stagesList;//ステージリスト

    ConnectedPlayerStatus connectedPlayerStatus;//プレイヤーステータス(選択したステージをここに渡す)

    //DEMO
    [HideInInspector]
    public string preScene;//前のシーン

    // Use this for initialization
    void Awake ()
    {
        if (GameObject.FindGameObjectWithTag("PlayerStatus") != null)
        {
            if (connectedPlayerStatus == null)
            {
                // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
                connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
            }

            //ステージシーンで選んだステージを選出
            foreach (var s in stagesList)
            {
                if (s.name == connectedPlayerStatus.StageName)
                {
                    //ゲーム開始時配置するオブジェクトに入れる
                    gameObjectList[0] = s;
                }
            }
        }

        if (GameObject.Find("BGMController(Clone)") != null)
        {
            preScene = GameObject.Find("BGMController(Clone)").GetComponent<BGMController>().preScene;
            
            //DEMO(直接Title->Main)だったらランダムステージ
            if (preScene == "Title")
            {
                int randomStageIndex = Random.Range(0, stagesList.Count);
                gameObjectList[0] = stagesList[randomStageIndex];
            }
        }

        for (int i = 0;i<gameObjectList.Count;i++)
        {
            GameObject obj = Instantiate(gameObjectList[i]);//生成
            if (gameObjectList[i].name == "PauseManager")
            {
                obj.transform.SetParent(GameObject.Find("PausePanel").transform);//キャンバスに移る
                obj.transform.localPosition = Vector3.zero;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

}
