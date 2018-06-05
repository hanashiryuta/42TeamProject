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
    //プレイヤー準備関連
    [SerializeField]
    CheckPlayerStandby[] standbyCheck;//プレイヤースタンドバイチェック
    [SerializeField]
    Text mainText;
    bool _isAISpawned = false;

    float _delayTime = 1.5F;

    //シーン移転関連
    GameLoad gameload;
    [SerializeField]
    GameObject fadePanel;
    FadeController fadeController;
    bool isFaded = false;

    // Use this for initialization
    void Start ()
    {
        gameload = this.GetComponent<GameLoad>();
        fadeController = fadePanel.GetComponent<FadeController>();
    }

    // Update is called once per frame
    void Update ()
    {
        if (IsPlayerStandby())
        {
            if (!_isAISpawned)
            {
                Invoke("AICharacterSpawn", _delayTime);
                _isAISpawned = true;
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
                standbyCheck[i].PlayerLabel.text = "COM";
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

        Invoke("GameSceneLoad", _delayTime + 1f);
    }


    void GameSceneLoad()
    {
        fadeController.ChangeAlpha();
        Debug.Log(fadeController.IsFadeFinish);
        if (fadeController.IsFadeFinish && !isFaded)
        {
            gameload.LoadingStart();
            Debug.Log(isFaded);
            isFaded = true;
        }
    }
}
