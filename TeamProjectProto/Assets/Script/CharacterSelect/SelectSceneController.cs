/*
 * 作成日時：180605
 *  キャラ選択コントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure; // Required in C#

public class SelectSceneController : MonoBehaviour
{
    [SerializeField]
    CheckPlayerStandby[] standbyCheck;//プレイヤースタンドバイチェック

    [SerializeField]
    Text mainText;
    
    bool _isAISpawned = false;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (IsPlayerStandby())
        {
            if (!_isAISpawned)
            {
                Invoke("AICharacterSpawn", 1f);
                _isAISpawned = true;
                //AICharacterSpawn();
            }
            ToGameScene();
        }
	}

    /// <summary>
    /// 全員準備完了か
    /// </summary>
    /// <returns></returns>
    bool IsPlayerStandby()
    {
        bool allReady = false;
        for(int i = 0; i < ConnectedPlayerNums(); i++)
        {
            if(standbyCheck[i].IsSpawn == false)
            {
                allReady = false;
                break;
            }
            else
            {
                allReady = true;
            }
        }

        return allReady;
    }

    /// <summary>
    /// 接続しているプレイヤーの人数をカウント
    /// </summary>
    /// <returns>接続しているプレイヤー数</returns>
    int ConnectedPlayerNums()
    {
        int connectedPlayerNums = 0;

        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (standbyCheck[i].CurrentState.IsConnected)
            {
                connectedPlayerNums++;
            }
        }

        return connectedPlayerNums;
    }

    void AICharacterSpawn()
    {
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (!standbyCheck[i].CurrentState.IsConnected)
            {
                standbyCheck[i].IsAI = true;
                standbyCheck[i].SpawnAICharacter();
            }
        }
        _isAISpawned = true;
    }

    /// <summary>
    /// GameSceneに移転
    /// </summary>
    void ToGameScene()
    {
        //全員確定したら取り消せないようにする
        for (int i = 0; i < ConnectedPlayerNums(); i++)
        {
            standbyCheck[i].IsCanPressBtn = false;
        }

        mainText.text = "ゲーム\nスタートだ！";
        mainText.color = Color.yellow;
    }
}
