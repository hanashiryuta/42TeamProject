/*
 * 作成日時：180604
 * プレイヤースタンドバイチェック
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#
using UnityEngine.UI;

public class CheckPlayerStandby : MonoBehaviour
{
    public PlayerIndex playerIndex;// ゲームパッド用インデックス
    PlayerIndex controllablePlayerIndex;//ゲーム進行操作プレイヤーインデックス
    public PlayerIndex ControllablePlayerIndex
    {
        set { controllablePlayerIndex = value; }
    }
    GamePadState _previousState;
    public GamePadState PreviousState
    {
        get { return _currentState; }
    }

    GamePadState _currentState;
    public GamePadState CurrentState
    {
        get { return _currentState; }
    }
    [SerializeField]
    GameObject playerPrefabs;//キャラプレハブ
    [SerializeField]
    Texture tex;//テクスチャ
    [SerializeField]
    Text _playrLabel;//プレイヤーラベル
    public Text PlayerLabel
    {
        get { return _playrLabel; }
        set { _playrLabel = value; }
    }
    [SerializeField]
    Text _btnText;//ボタンテキスト（Aボタンを押して入場）
    [SerializeField]
    GameObject orgin_playerBG;//Player背景
    GameObject playerBG;

    GameObject player;
    bool _isSpawn = false;//キャラ生成したか
    public bool IsSpawn
    {
        get { return _isSpawn; }
    }
    bool _isCanPressBtn = false;//ボタン入力受け付けているか
    public bool IsCanPressBtn
    {
        get { return _isCanPressBtn; }
        set { _isCanPressBtn = value; }
    }
    bool _isAI = false;//AIキャラか
    public bool IsAI
    {
        get { return _isAI; }
        set { _isAI = value; }
    }

    bool _isStartPressed = false;//Start押されたか
    public bool IsStartPressed
    {
        get { return _isStartPressed; }
        set { _isStartPressed = value; }
    }

    bool _isBackPressed = false;//Back押されたか
    public bool IsBackPressed
    {
        get { return _isBackPressed; }
        set { _isBackPressed = value; }
    }


    // Use this for initialization
    void Start ()
    {
        playerBG = GameObject.Instantiate(orgin_playerBG, this.transform);
    }

    // Update is called once per frame
    void Update ()
    {
        _currentState = GamePad.GetState(playerIndex);

        if (_isCanPressBtn)
        {
            CheckSpawn();

        }

        _previousState = _currentState;
    }

    /// <summary>
    /// プレイヤー生成チェック
    /// </summary>
    void CheckSpawn()
    {
        if (!_isSpawn)//生成してない時
        {
            if (Is_Abtn_Pressed())//Aボタン押したら
            {
                SpawnPlayerCharacter();
                _isSpawn = true;
            }
        }
        else//生成していた時
        {
            if (Is_Bbtn_Pressed())//Bボタン押したら
            {
                RemovePlayerCharacter();
                _isSpawn = false;
            }
        }

        //自分がゲーム進行操作プレイヤーだったら
        if (playerIndex == controllablePlayerIndex)
        {
            //生成している時
            if (_isSpawn)
            {
                _isStartPressed = Is_StartBtn_Pressed(); //STARTボタン押したか
            }

            _isBackPressed = Is_BackBtn_Pressed(); //Backボタン押したか
        }

    }

    /// <summary>
    /// プレイヤーキャラ生成
    /// 準備状態に入る
    /// </summary>
    void SpawnPlayerCharacter()
    {
        player = GameObject.Instantiate(playerPrefabs, transform.position, Quaternion.Euler(0, 180, 0));
        player.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = tex;//テクスチャ変更
        //playerBG = GameObject.Instantiate(orgin_playerBG, this.transform);

        _playrLabel.enabled = true;
        _btnText.enabled = false;
    }

    /// <summary>
    /// 準備状態キャンセル
    /// </summary>
    void RemovePlayerCharacter()
    {
        GameObject.Destroy(player);
        player = null;
        //GameObject.Destroy(playerBG);
        //playerBG = null;
        _playrLabel.enabled = false;
        _btnText.enabled = true;
    }

    /// <summary>
    /// AIキャラ生成
    /// 準備状態に入る
    /// </summary>
    public void SpawnAICharacter()
    {
        player = GameObject.Instantiate(playerPrefabs, transform.position, Quaternion.Euler(0, 180, 0));
        player.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = tex;//テクスチャ変更
        _playrLabel.enabled = true;

        _isSpawn = true;
    }

    /// <summary>
    /// Aボタンを押したかをチェック
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    bool Is_Abtn_Pressed()
    {
        if (_previousState.Buttons.A == ButtonState.Released &&
            _currentState.Buttons.A == ButtonState.Pressed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Bボタンを押したかをチェック
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    bool Is_Bbtn_Pressed()
    {
        if (_previousState.Buttons.B == ButtonState.Released &&
            _currentState.Buttons.B == ButtonState.Pressed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// STARTボタンを押したかをチェック
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    bool Is_StartBtn_Pressed()
    {
        if (_previousState.Buttons.Start == ButtonState.Released &&
            _currentState.Buttons.Start == ButtonState.Pressed)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// BACKボタンを押したかをチェック
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    bool Is_BackBtn_Pressed()
    {
        if (_previousState.Buttons.Back == ButtonState.Released &&
            _currentState.Buttons.Back == ButtonState.Pressed)
        {
            return true;
        }
        return false;
    }
}
