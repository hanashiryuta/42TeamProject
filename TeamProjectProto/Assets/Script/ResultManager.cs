//
//作成日時：4月25日
//リザルト画面クラス
//作成者：平岡誠司
//
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using System.Linq;

public class ResultManager : MonoBehaviour
{
    PlayerRank playerRank;//Playerの名前取得用
    [SerializeField]
    Button oneMoreBtn, toTitleBtn;//各ボタンの情報
    Button nowSelectedBtn;//今選択しているボタン

    [SerializeField]
    float textMovingTime = 2f;//順位テキストの移動スピード

    List<Text> _playerRankTextsList = new List<Text>();//順位表示テキストOBJリスト
    public List<Text> PlayerRankTextsList
    {
        get { return _playerRankTextsList; }
    }
    List<int> _playerRankList = new List<int>(); // プレイヤー順位リスト
    public List<int> PlayerRankList
    {
        get { return _playerRankList; }
    }

    List<Vector3> finishPosition = new List<Vector3>();//最終位置
    List<GameObject> playerScoreTexts = new List<GameObject>();//プレイヤースコア表示テキスト

    bool _isAnim = true;//アニメ中か

    ConnectedPlayerStatus connectedPlayerStatus;//接続したプレイヤー
    SpawnUIPlayer spawnUIPlayer;//UIプレイヤースポーン

    //fade
    FadeController fadeController;
    bool isFadeOuted = false;

    //load
    GameLoad gameLoad;
    bool isSceneChange = false;

    //Input
    PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;
    float moveX = 0;

    //SE
    SEController se;

    //RankSpawn
    ResultPositionSpawnController resultPositionSpawnCon;

    // Use this for initialization
    void Awake()
    {
        //接続プレイヤー数を取得
        if (connectedPlayerStatus == null)
        {
            // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }

        playerRank = GameObject.Find("PlayerRankController").GetComponent<PlayerRank>();

        //ランク生成初期位置を決定しランクOBJを生成
        resultPositionSpawnCon = GetComponent<ResultPositionSpawnController>();
        resultPositionSpawnCon.SetRanksDefaltPosition(connectedPlayerStatus.ConnectedPlayer.Count);
        resultPositionSpawnCon.SetRanksFinishPosition(connectedPlayerStatus.ConnectedPlayer.Count);
        // アニメ用位置を渡す
        //defaltPosition = resultPositionSpawnCon.DefaultPositionsList;
        finishPosition = resultPositionSpawnCon.FinishPositionsList;
        //テキストコンポーネントを渡す
        for (int i = 0; i < resultPositionSpawnCon.RankOBJList.Count; i++)
        {
            _playerRankTextsList.Add(resultPositionSpawnCon.RankOBJList[i].GetComponent<Text>());
        }

        //順位付け
        _playerRankList = SetPlayersRank();

        //プレイヤーランクテキスト
        SetPlayerRankText();

        //スポーンUIプレイヤー
        SetSpawnUIPlayer();

        //fade
        fadeController = GameObject.Find("FadePanel").GetComponent<FadeController>();
        //load
        gameLoad = transform.GetComponent<GameLoad>();
        //現在接続しているプレイヤーの中で番号が一番小さいやつを選択プレイヤーにする
        SetControllablePlayer();
        //SE
        se = transform.GetComponent<SEController>();
    }

    // Update is called once per frame
    void Update()
    {
        SetControllablePlayer();

        //fadein
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }
        else
        {
            //fadeout
            if (isSceneChange)
            {
                fadeController.FadeOut();
            }
        }

        //順位アニメ中か
        if (_isAnim)
        {
            StartCoroutine(ShowRankCoroutine());
        }
        else
        {
            //XInput
            currentState = GamePad.GetState(playerIndex);
            moveX = currentState.ThumbSticks.Left.X;
            ResultXInput();
            Debug.Log("moveX =" + moveX);
        }

        //NextSceneLoad
        if (fadeController.IsFadeOutFinish && !isFadeOuted)
        {
            gameLoad.LoadingStartWithOBJ();
            isFadeOuted = true;
        }

        previousState = currentState;
    }

    void ResultXInput()
    {
        //right
        if (moveX >= 0.8f && nowSelectedBtn != toTitleBtn)
        {
            toTitleBtn.Select();
            se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
            nowSelectedBtn = toTitleBtn;
        }

        //left
        if (moveX <= -0.8f && nowSelectedBtn != oneMoreBtn)
        {
            oneMoreBtn.Select();
            se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
            nowSelectedBtn = oneMoreBtn;
        }

        //A
        if (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed)
        {
            se.PlaySystemSE((int)SEController.SystemSE.OK);
            BtnPushed(nowSelectedBtn);
        }
    }

    /// <summary>
    /// 選択しているボタンのonClickイベントを呼ぶ
    /// </summary>
    /// <param name="btn"></param>
    void BtnPushed(Button btn)
    {
        btn.onClick.Invoke();
    }

    /// <summary>
    /// 「もう1度遊ぶ」を選んだらステージ生成シーンへ
    /// </summary>
    public void OneMoreBtn()
    {
        gameLoad.NextScene = GameLoad.Scene.StageSelect;
        isSceneChange = true;
    }

    /// <summary>
    /// 「タイトルへ」を選んだらタイトルシーンへ
    /// </summary>
    public void ToTitleBtn()
    {
        gameLoad.NextScene = GameLoad.Scene.Tilte;

        //接続プレイヤーステータス受け取りオブジェを削除
        connectedPlayerStatus.Created = false;
        Destroy(connectedPlayerStatus.transform.gameObject);

        isSceneChange = true;
    }

    /// <summary>
    /// 追加日：180601 追加者：何
    /// テキストアニメーション
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowRankCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        //DoMoveの原点は左下なので中心に移動
        Vector2 centerPos = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        for (int i = 0; i < _playerRankTextsList.Count; i++)
        {
            _playerRankTextsList[i].rectTransform
                .DOAnchorPos(new Vector2(finishPosition[i].x, finishPosition[i].y), textMovingTime);
            yield return new WaitForSeconds(0.5f);
        }
        //アニメ終わったらoneMore選択
        oneMoreBtn.Select();
        nowSelectedBtn = oneMoreBtn;

        _isAnim = false;
    }

    /// <summary>
    /// 操作可能なプレイヤーを選択
    /// </summary>
    void SetControllablePlayer()
    {
        if (GamePad.GetState(PlayerIndex.One).IsConnected)
        {
            playerIndex = PlayerIndex.One;
        }
        else if (GamePad.GetState(PlayerIndex.Two).IsConnected)
        {
            playerIndex = PlayerIndex.Two;
        }
        else if (GamePad.GetState(PlayerIndex.Three).IsConnected)
        {
            playerIndex = PlayerIndex.Three;
        }
        else if (GamePad.GetState(PlayerIndex.Four).IsConnected)
        {
            playerIndex = PlayerIndex.Four;
        }
    }

    /// <summary>
    /// プレイヤーのスコアで順位付け/
    /// テキストOBJ生成し格納/
    /// スコアテキストを格納
    /// </summary>
    List<int> SetPlayersRank()
    {
        List<int> eachRanksCount = new List<int>();//順位ごとの人数

        int rank = 0;//順位
        int count = 0;//順位カウント
        float temp = 0;//スコア一時格納
        //順位付け
        for (int i = 0; i < playerRank.PlayerRankScore.Count; i++)
        {
            if (i == 0)
            {
                temp = playerRank.PlayerRankScore[0];
                rank = count = 1;
            }
            else
            {
                if (temp == playerRank.PlayerRankScore[i])
                {
                    count++;
                }
                else
                {
                    temp = playerRank.PlayerRankScore[i];
                    rank += count;
                    count = 1;
                }
            }

            eachRanksCount.Add(rank);
        }

        //スコアテキストを格納
        foreach(var textOBJ in _playerRankTextsList)
        {
            Debug.Log(textOBJ.transform.GetChild(1).gameObject.name);
            playerScoreTexts.Add(textOBJ.transform.GetChild(1).gameObject);
        }

        return eachRanksCount;
    }

    /// <summary>
    /// スポーンUIプレイヤー初期設定
    /// </summary>
    void SetSpawnUIPlayer()
    {
        spawnUIPlayer = transform.GetComponent<SpawnUIPlayer>();
        spawnUIPlayer.ConnectedPLStatus = connectedPlayerStatus;//UIプレイヤー接続ステータス
        spawnUIPlayer.PList = playerRank.ResultRank;//UIプレイヤー順位順
        spawnUIPlayer.RankList = PlayerRankList;//UIプレイヤーごとの順位
        for (int i = 0; i < PlayerRankTextsList.Count; i++)//UIプレイヤー生成場所
        {
            spawnUIPlayer.PositionOBJ.Add(PlayerRankTextsList[i].transform.GetChild(0).gameObject);
        }
    }

    /// <summary>
    /// プレイヤーランクテキスト初期設定
    /// </summary>
    void SetPlayerRankText()
    {
        //テキスト初期化
        for (int i = 0; i < _playerRankTextsList.Count; i++)
        {
            //ランクテキスト初期化
            _playerRankTextsList[i].text = "";
            //スコアテキスト初期化
            playerScoreTexts[i].GetComponent<Text>().text = "";
            playerScoreTexts[i].transform.GetChild(0).GetComponent<Text>().text = "";
        }

        //接続しているプレイヤー数だけ表示する
        for (int i = 0; i < connectedPlayerStatus.ConnectedPlayer.Count; i++)
        {
            //上から順位表示
            _playerRankTextsList[i].text = HalfWidth2FullWidth.Set2FullWidth(_playerRankList[i]) + " 位:";
            //スコア表示
            playerScoreTexts[i].GetComponent<Text>().text = HalfWidth2FullWidth.Set2FullWidth(playerRank.PlayerRankScore[i]);
            playerScoreTexts[i].transform.GetChild(0).GetComponent<Text>().text = "チョキン";
        }
    }
}
