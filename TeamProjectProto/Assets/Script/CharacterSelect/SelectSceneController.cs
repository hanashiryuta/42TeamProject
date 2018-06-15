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

    //接続したプレイヤー
    [SerializeField]
    GameObject connectedPlayerStatusObj;
    ConnectedPlayerStatus connectedPlayerStatus;

    // Use this for initialization
    void Start()
    {
        gameload = this.GetComponent<GameLoad>();
        fadeController = fadePanel.GetComponent<FadeController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsPlayerStandby())
        {
            //AI無し
            //if (!_isAISpawned)
            //{
            //    Invoke("AICharacterSpawn", _delayTime);
            //    _isAISpawned = true;
            //}
            ToGameScene();
        }
    }

    /// <summary>
    /// 全員準備完了か
    /// </summary>
    /// <returns></returns>
    bool IsPlayerStandby()
    {
        bool allReady = false;//準備完了
        int readyPlayers = 0;//準備完了プレイヤー数
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (standbyCheck[i].IsSpawn == false)
            {
                allReady = false;//準備完了してない
                //break; 前のプレイヤーが空なら後ろのチェック行かないのでbreakしちゃだめ
            }
            else
            {
                readyPlayers++;
            }
        }

        //準備完了したプレイヤーと 接続しているプレイヤー人数と同じ
        //且つプレイヤーが一人ではない
        if (readyPlayers == ConnectedPlayerCount() &&
            ConnectedPlayerCount() != 1)
        {
            allReady = true;//準備完了
        }

        //プレイヤーが一人の時
        if (readyPlayers == ConnectedPlayerCount() &&
            ConnectedPlayerCount() == 1)
        {
            mainText.text = "一人は遊べない！";
        }
        else
        {
            mainText.text = "Aボタンを押して\n入場";
        }

        return allReady;
    }

    /// <summary>
    /// 接続しているプレイヤーの人数をカウント
    /// </summary>
    /// <returns>接続しているプレイヤー数</returns>
    int ConnectedPlayerCount()
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
        for (int i = 0; i < ConnectedPlayerCount(); i++)
        {
            standbyCheck[i].IsCanPressBtn = false;
        }

        SavePlayerStatus();

        mainText.text = "ゲーム\nスタートだ！";
        mainText.color = Color.yellow;

        Invoke("SceneLoad", _delayTime + 1f);
    }


    void SceneLoad()
    {
        fadeController.ChangeAlpha();
        if (fadeController.IsFadeFinish && !isFaded)
        {
            gameload.LoadingStart();
            isFaded = true;
        }
    }

    /// <summary>
    /// 参戦しているプレイヤーを記録
    /// </summary>
    void SavePlayerStatus()
    {
        connectedPlayerStatus = Instantiate(connectedPlayerStatusObj).GetComponent<ConnectedPlayerStatus>();
        Debug.Log(connectedPlayerStatus);
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (standbyCheck[i].IsSpawn)
            {
                connectedPlayerStatus.ConnectedPlayer.Add("Player" + (i + 1), i);
            }
        }          
    }
}
