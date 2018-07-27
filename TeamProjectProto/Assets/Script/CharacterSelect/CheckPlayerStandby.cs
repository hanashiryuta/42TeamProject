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
    GamePadState _currentState;//今のGamePad状態
    GamePadState _previousState;//前のGamePad状態

    [SerializeField]
    GameObject playerPrefabs;//キャラプレハブ
    [SerializeField]
    Texture tex;//テクスチャ
    [SerializeField]
    Text _playrLabel;//プレイヤー名前ラベル
    [SerializeField]
    Text _btnText;//ボタンテキスト（Aボタンを押して入場）

    [SerializeField]
    GameObject orgin_playerBG;//Player背景
    GameObject playerBG;
    GameObject player;
    bool _isSpawn = false;//キャラ生成したか
    bool _isCanPressBtn = false;//ボタン入力受け付けているか
    bool _isAI = false;//AIキャラか

    bool _isStartPressed = false;//Start押されたか
    bool _isB_BackPressed = false;//B(Back)押されたか

    //SE
    SEController se;

    //キャンセルか
    bool isCancel = false;

    public PlayerIndex ControllablePlayerIndex
    {
        set { controllablePlayerIndex = value; }
    }
    public GamePadState CurrentState
    {
        get { return _currentState; }
    }
    public Text PlayerLabel
    {
        get { return _playrLabel; }
        set { _playrLabel = value; }
    }
    public bool IsStartPressed
    {
        get { return _isStartPressed; }
        set { _isStartPressed = value; }
    }
    public bool IsB_BackPressed
    {
        get { return _isB_BackPressed; }
        set { _isB_BackPressed = value; }
    }
    public bool IsSpawn
    {
        get { return _isSpawn; }
        set { _isSpawn = value; }
    }
    public bool IsCanPressBtn
    {
        get { return _isCanPressBtn; }
        set { _isCanPressBtn = value; }
    }
    public bool IsAI
    {
        get { return _isAI; }
        set { _isAI = value; }
    }


    // Use this for initialization
    void Start ()
    {
        playerBG = GameObject.Instantiate(orgin_playerBG, this.transform);//背景生成
        se = transform.GetComponent<SEController>();
    }

    // Update is called once per frame
    void Update ()
    {
        _currentState = GamePad.GetState(playerIndex);

        //ボタン押せる状態で
        if (_isCanPressBtn)
        {
            //プレイヤー生成チェック
            CheckSpawn();

            //キャンセル復帰をずらす
            CancelInit_Delay(0.2f);

            //自分がゲーム進行操作プレイヤーだったら
            ControllablePlayerInput();
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
            if (IsBtnPressed("A"))//Aボタン押したら
            {
                SpawnPlayerCharacter();//プレイヤー生成
                _isSpawn = true;
            }
        }
        else//生成していた時
        {
            if (IsBtnPressed("B"))//Bボタン押したら
            {
                RemovePlayerCharacter();//プレイヤー廃棄
                isCancel = true;//生成キャンセル
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
        player = GameObject.Instantiate(playerPrefabs, transform.position, Quaternion.Euler(0, 180, 0));//生成
        player.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = tex;//テクスチャ変更
        se.PlaySystemSE((int)SEController.SystemSE.CharacterSpawn);//SE
　      _playrLabel.enabled = true;//名前表示
        _btnText.enabled = false;//未参加テキスト非表示
    }

    /// <summary>
    /// プレイヤーキャラ削除
    /// 準備状態キャンセル
    /// </summary>
    void RemovePlayerCharacter()
    {
        GameObject.Destroy(player);//削除
        player = null;//nullに
        se.PlaySystemSE((int)SEController.SystemSE.Cancel);//SE
        _playrLabel.enabled = false;//名前非表示
        _btnText.enabled = true;//未参加テキスト表示
    }

    /// <summary>
    /// AIキャラ生成
    /// 準備状態に入る
    /// </summary>
    public void SpawnAICharacter()
    {
        player = GameObject.Instantiate(playerPrefabs, transform.position, Quaternion.Euler(0, 180, 0));
        player.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = tex;//テクスチャ変更
        _playrLabel.enabled = true;//名前表示
        _btnText.enabled = false;//未参加テキスト非表示
    }
  
    /// <summary>
    /// 指定のボタンが押されたか？
    /// </summary>
    /// <param name="btn"></param>
    /// <returns></returns>
    bool IsBtnPressed(string btn)
    {
        switch (btn)
        {
            case "A":
                if (_previousState.Buttons.A == ButtonState.Released &&
                    _currentState.Buttons.A == ButtonState.Pressed)
                {
                    return true;
                }
                return false;

            case "B":
                if (_previousState.Buttons.B == ButtonState.Released &&
                    _currentState.Buttons.B == ButtonState.Pressed)
                {
                    return true;
                }
                return false;

            case "Start":
                if (_previousState.Buttons.Start == ButtonState.Released &&
                    _currentState.Buttons.Start == ButtonState.Pressed)
                {
                    return true;
                }
                return false;

            default:
                return false;
        }
    }

    /// <summary>
    /// ゲーム進行操作プレイヤー操作
    /// </summary>
    void ControllablePlayerInput()
    {
        //自分がゲーム進行操作プレイヤーだったら
        if (playerIndex == controllablePlayerIndex)
        {
            //生成している時
            if (_isSpawn)
            {
                _isStartPressed = IsBtnPressed("Start");//STARTボタン押したか
            }

            if (!_isSpawn && !isCancel)//生成してない時且つ取り消しじゃない
            {
                _isB_BackPressed = IsBtnPressed("B");//Bボタン押したか(前のシーンに戻る)
            }
        }
    }

    /// <summary>
    /// キャラ生成をキャンセルしたら設定した秒数後にisCancelをfalseに
    /// (もう一回B押したら前のシーンに戻る状態に)
    /// </summary>
    void CancelInit_Delay(float time)
    {
        if (isCancel)
        {
            Invoke("CancelInit", time);
        }
    }

    /// <summary>
    /// isCancelを初期化
    /// </summary>
    void CancelInit()
    {
        isCancel = false;
    }
}
