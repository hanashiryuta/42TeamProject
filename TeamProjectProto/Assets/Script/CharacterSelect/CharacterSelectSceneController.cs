/*
 * 作成日時：180605
 *  キャラ選択シーンコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure; // Required in C#

public class CharacterSelectSceneController : SceneController
{
    //プレイヤー準備関連
    [SerializeField]
    CheckPlayerStandby[] standbyCheck;//プレイヤースタンドバイチェック
    PlayerIndex controllablePlayerIndex;//操作可能プレイヤーのインデックス
    [SerializeField]
    Text mainText;//中央の表示テキスト
    [SerializeField]
    Text player1Text, startText;

    //AI
    bool _isAISpawned = false;

    //接続したプレイヤー関連
    [SerializeField]
    GameObject connectedPlayerStatusObj;//接続プレイヤーステータスOBJ
    ConnectedPlayerStatus connectedPlayerStatus;//接続プレイヤーステータス

    //SceneState
    CharacterSceneState sceneState = CharacterSceneState.FadeIn;

    public override void Start()
    {
        base.Start();

        mainText.enabled = false;
        player1Text.enabled = false;
        startText.enabled = false;
    }

    /// <summary>
    /// シーンの状態に沿ってメソッド実行
    /// </summary>
    public override void CheckSceneState()
    {
        //Get XInput
        currentState = GamePad.GetState(playerIndex);

        switch (sceneState)
        {
            case CharacterSceneState.FadeIn://フェードイン中
                if (fadeController.IsFadeInFinish)
                {
                    //ボタン判定を起動
                    for (int i = 0; i < ConnectedPlayerCount(); i++)
                        standbyCheck[i].IsCanPressBtn = true;

                    sceneState = CharacterSceneState.None;
                }
                break;

            case CharacterSceneState.None://基準状態
                if (Is_ControllablePlayer_Pressed_Start())//Start押されたら
                {
                    //SE鳴る
                    se.PlaySystemSE((int)SEController.SystemSE.OK);

                    //NPC生成
                    if (!_isAISpawned)
                    {
                        AICharacterSpawn();
                    }

                    sceneState = CharacterSceneState.ToNextScene;
                }
                if (Is_ControllablePlayer_Pressed_B_Back())//B(Back)押されたら
                {
                    //SE鳴る
                    se.PlaySystemSE((int)SEController.SystemSE.Cancel);
                    sceneState = CharacterSceneState.ToPreScene;
                }
                break;

            case CharacterSceneState.ToPreScene://前のシーンに移行
                ToTitleScene();
                //フェードアウト終わったら
                if (fadeController.IsFadeOutFinish)
                    //NextSceneLoad
                    gameLoad.LoadingStartWithOBJ();
                break;

            case CharacterSceneState.ToNextScene://次のシーンに移行
                ToStageSelectScene();
                //フェードアウト終わったら
                if (fadeController.IsFadeOutFinish)
                    //NextSceneLoad
                    gameLoad.LoadingStartWithOBJ();
                break;
        }

        previousState = currentState;
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
            if (standbyCheck[(int)controllablePlayerIndex].IsStartPressed)
            {
                gameLoad.NextScene = GameLoad.Scenes.StageSelect;//ステージシーンに
                isAllReady = true;//準備完了
            }
        }

        //プレイヤーが一人の時
        if (readyPlayers >= 1)
        {
            mainText.text = "　　の\n" +
                            "　　　　　　　で\n" +
                            "ゲームスタート！";
            mainText.enabled = true;
            player1Text.text = HalfWidth2FullWidth.Set2FullWidth("P" + ((int)controllablePlayerIndex + 1));
            player1Text.enabled = true;
            startText.enabled = true;
        }

        return isAllReady;
    }

    /// <summary>
    /// ゲーム進行操作可能なプレイヤーがB(Back)ボタン押したか
    /// </summary>
    /// <returns></returns>
    bool Is_ControllablePlayer_Pressed_B_Back()
    {
        if (standbyCheck[(int)controllablePlayerIndex].IsB_BackPressed)
        {
            gameLoad.NextScene = GameLoad.Scenes.Title;
        }
        return standbyCheck[(int)controllablePlayerIndex].IsB_BackPressed;
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

    /// <summary>
    /// NPC生成
    /// </summary>
    void AICharacterSpawn()
    {
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            //PLがない位置はNPCで埋める
            if (!standbyCheck[i].IsSpawn)
            {
                standbyCheck[i].IsAI = true;
                standbyCheck[i].SpawnAICharacter();
                standbyCheck[i].PlayerLabel.text = "ＮＰＣ";
                standbyCheck[i].IsSpawn = true;
            }
        }
        _isAISpawned = true;
    }

    /// <summary>
    /// StageSelectSceneに移転
    /// </summary>
    void ToStageSelectScene()
    {
        //全員確定したら取り消せないようにする
        for (int i = 0; i < ConnectedPlayerCount(); i++)
        {
            standbyCheck[i].IsCanPressBtn = false;
        }

        SavePlayerStatus();

        mainText.text = "準備完了！";
        mainText.color = Color.yellow;
        player1Text.enabled = false;
        startText.enabled = false;

        gameLoad.NextScene = GameLoad.Scenes.StageSelect;
        fadeOutDelayTime = 1f;
        isSceneChange = true;
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

        gameLoad.NextScene = GameLoad.Scenes.Title;
        isSceneChange = true;
    }

    /// <summary>
    /// 参戦しているプレイヤーを記録
    /// </summary>
    void SavePlayerStatus()
    {
        if(GameObject.FindGameObjectWithTag("PlayerStatus") != null)
        {
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }
        else
        {
            connectedPlayerStatus = Instantiate(connectedPlayerStatusObj).GetComponent<ConnectedPlayerStatus>();
        }
        //再格納のためClear
        connectedPlayerStatus.ConnectedPlayer.Clear();
        connectedPlayerStatus.IsAIList.Clear();
        //プレイヤーInfoを格納
        for (int i = 0; i < standbyCheck.Length; i++)
        {
            if (standbyCheck[i].IsSpawn)
            {
                //AIか
                if (standbyCheck[i].IsAI)
                {
                    connectedPlayerStatus.IsAIList.Add(true);
                }
                else
                {
                    connectedPlayerStatus.IsAIList.Add(false);
                }
                //プレイヤー名とインデックスを格納
                connectedPlayerStatus.ConnectedPlayer.Add("Player" + (i + 1), i);
            }
        }
    }
}
