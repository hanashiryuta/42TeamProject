/*
 * 作成日時：180604
 * プレイヤースタンドバイチェック
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

public class CheckPlayerStandby : MonoBehaviour
{
    [SerializeField]
    PlayerIndex playerIndex;// ゲームパッド用インデックス
    GamePadState _previousState;
    GamePadState _currentState;
    public GamePadState CurrentState
    {
        get { return _currentState; }
    }
    [SerializeField]
    GameObject playerPrefabs;//キャラプレハブ
    [SerializeField]
    Texture tex;//テクスチャ

    GameObject player;
    bool _isSpawn = false;//キャラ生成したか
    public bool IsSpawn
    {
        get { return _isSpawn; }
    }
    bool _isCanPressBtn = true;//ボタン入力受け付けているか
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

    // Use this for initialization
    void Start ()
    {		
	}
	
	// Update is called once per frame
	void Update ()
    {
        _currentState = GamePad.GetState(playerIndex);

        if (_isCanPressBtn)
        {
            CheckStandBy();
        }

        _previousState = _currentState;
    }

    void CheckStandBy()
    {
        if (!_isSpawn)//生成してない時
        {
            if (Is_Abtn_Pressed())//Aボタン押してら
            {
                SpawnPlayerCharacter();
                _isSpawn = true;
            }
        }
        else//生成していた時
        {
            if (Is_Bbtn_Pressed())//Bボタン押してら
            {
                RemovePlayerCharacter();
                _isSpawn = false;
            }
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
    }

    /// <summary>
    /// 準備状態キャンセル
    /// </summary>
    void RemovePlayerCharacter()
    {
        GameObject.Destroy(player);
        player = null;
    }

    /// <summary>
    /// AIキャラ生成
    /// 準備状態に入る
    /// </summary>
    public void SpawnAICharacter()
    {
        player = GameObject.Instantiate(playerPrefabs, transform.position, Quaternion.Euler(0, 180, 0));
        player.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = tex;//テクスチャ変更
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
}
