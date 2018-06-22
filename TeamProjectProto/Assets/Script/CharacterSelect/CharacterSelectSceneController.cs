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

public class CharacterSelectSceneController : MonoBehaviour
{
    //プレイヤー準備関連
    [SerializeField]
    CheckPlayerStandby[] standbyCheck;//プレイヤースタンドバイチェック
    PlayerIndex controllablePlayerIndex;
    [SerializeField]
    Text mainText;//中央の表示テキスト

    bool _isAISpawned = false;

    float _delayTime = 1.5F;

    //シーン移転関連
    GameLoad gameload;
    [SerializeField]
    GameObject fadePanel;
    FadeController fadeController;
    bool isFadedOut = false;
    bool isFadedIn = false;

    //接続したプレイヤー
    [SerializeField]
    GameObject connectedPlayerStatusObj;
    ConnectedPlayerStatus connectedPlayerStatus;

    bool isStandbied = false;

    // Use this for initialization
    void Start()
    {
        gameload = this.GetComponent<GameLoad>();
        fadeController = fadePanel.GetComponent<FadeController>();
        fadeController.FadeIn();
    }

    // Update is called once per frame
    void Update()
    {
        SetControllablePlayer();

        //フェードイン中か
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }
        else
        {
            if (!isFadedIn)
            {
                for (int i = 0; i < ConnectedPlayerCount(); i++)
                {
                    standbyCheck[i].IsCanPressBtn = true;
                }
                isFadedIn = true;
            }
        }

        //全員スタンドバイしたか
        if (Is_ControllablePlayer_Pressed_Start()/*IsPlayerStandby()*/)
        {
            //AI無し
            //if (!_isAISpawned)
            //{
            //    Invoke("AICharacterSpawn", _delayTime);
            //    _isAISpawned = true;
            //}
            ToStageSelectScene();
        }

        //Back押されたら
        if (Is_ControllablePlayer_Pressed_Back())
        {
            Debug.Log("Back");
            ToTitleScene();
        }

    }

    /// <summary>
    /// 全員準備完了か
    /// </summary>
    /// <returns></returns>
    bool IsPlayerStandby()
    {
        bool isAllReady = false;//準備完了か
        int readyPlayers = 0;//準備完了プレイヤー数
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (standbyCheck[i].IsSpawn == false)
            {
                isAllReady = false;//準備完了してない
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
            isAllReady = true;//準備完了
        }

        //プレイヤーが一人の時
        if (readyPlayers == ConnectedPlayerCount() &&
            ConnectedPlayerCount() == 1)
        {
            mainText.text = "一人は遊べない！";
        }
        else
        {
            mainText.text = "Ａボタンを押して\n入場";
        }

        return isAllReady;
    }

    /// <summary>
    /// ゲーム進行操作可能なプレイヤーがSTARTボタン押したか
    /// </summary>
    /// <returns></returns>
    bool Is_ControllablePlayer_Pressed_Start()
    {
        bool isAllReady = false;//準備完了か
        int readyPlayers = 0;//準備完了プレイヤー数
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (standbyCheck[i].IsSpawn == false)
            {
                isAllReady = false;//準備完了してない
                //break; 前のプレイヤーが空なら後ろのチェック行かないのでbreakしちゃだめ
            }
            else
            {
                readyPlayers++;
            }
        }

        //ゲーム進行操作プレイヤーが生成したか
        if (standbyCheck[(int)controllablePlayerIndex].IsSpawn)
        {
            //ゲーム進行操作プレイヤーがSTART押したら
            if (standbyCheck[(int)controllablePlayerIndex].IsStartPressed &&
                readyPlayers != 1)
            {
                isAllReady = true;//準備完了
            }
        }

        //プレイヤーが一人の時
        if (readyPlayers == 1)
        {
            mainText.text = "一人は遊べない！";
        }
        else
        {
            mainText.text = "準備完了したら\n" +
                            "Ｐｌａｙｅｒ" + HalfWidth2FullWidth.Set2FullWidth(((int)controllablePlayerIndex) + 1) +
                            "が\nＳＴＡＲＴ押してね！";
        }

        return isAllReady;
    }

    /// <summary>
    /// ゲーム進行操作可能なプレイヤーがSTARTボタン押したか
    /// </summary>
    /// <returns></returns>
    bool Is_ControllablePlayer_Pressed_Back()
    {
        return standbyCheck[(int)controllablePlayerIndex].IsBackPressed;
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
    /// StageSelectSceneに移転
    /// </summary>
    void ToStageSelectScene()
    {
        isStandbied = true;

        //全員確定したら取り消せないようにする
        for (int i = 0; i < ConnectedPlayerCount(); i++)
        {
            standbyCheck[i].IsCanPressBtn = false;
        }

        SavePlayerStatus();

        mainText.text = "準備完了！";
        mainText.color = Color.yellow;

        gameload.NextScene = GameLoad.Scene.StageSelect;
        Invoke("SceneLoad", _delayTime + 1f);
    }

    /// <summary>
    /// シーンロード
    /// </summary>
    void SceneLoad()
    {
        fadeController.FadeOut();
        if (fadeController.IsFadeOutFinish && !isFadedOut)
        {
            gameload.LoadingStartWithOBJ();
            isFadedOut = true;
        }
    }

    /// <summary>
    /// TitleSceneに移転
    /// </summary>
    void ToTitleScene()
    {
        //移転時ボタン押せないように
        for (int i = 0; i < ConnectedPlayerCount(); i++)
        {
            standbyCheck[i].IsCanPressBtn = false;
        }

        gameload.NextScene = GameLoad.Scene.Tilte;
        Invoke("SceneLoad", 0f);
    }


    /// <summary>
    /// 参戦しているプレイヤーを記録
    /// </summary>
    void SavePlayerStatus()
    {
        connectedPlayerStatus = Instantiate(connectedPlayerStatusObj).GetComponent<ConnectedPlayerStatus>();
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (standbyCheck[i].IsSpawn)
            {
                connectedPlayerStatus.ConnectedPlayer.Add("Player" + (i + 1), i);
            }
        }          
    }

    /// <summary>
    /// 操作可能なプレイヤーを選択
    /// /// </summary>
    void SetControllablePlayer()
    {
        if (GamePad.GetState(PlayerIndex.One).IsConnected)
        {
            controllablePlayerIndex = PlayerIndex.One;
        }
        else if (GamePad.GetState(PlayerIndex.Two).IsConnected)
        {
            controllablePlayerIndex = PlayerIndex.Two;
        }
        else if (GamePad.GetState(PlayerIndex.Three).IsConnected)
        {
            controllablePlayerIndex = PlayerIndex.Three;
        }
        else if (GamePad.GetState(PlayerIndex.Four).IsConnected)
        {
            controllablePlayerIndex = PlayerIndex.Four;
        }

        for(int i = 0; i < standbyCheck.Length; i++)
        {
            standbyCheck[i].ControllablePlayerIndex = controllablePlayerIndex;
        }
    }
}
