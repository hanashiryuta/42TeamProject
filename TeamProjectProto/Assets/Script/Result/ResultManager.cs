//
//作成日時：4月25日
//リザルトマネージャー
//作成者：何承恩
//
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class ResultManager : SceneController
{
    //プレイヤー順位関連
    PlayerRank playerRank;//Playerの名前取得用
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
    ConnectedPlayerStatus connectedPlayerStatus;//接続したプレイヤー
    //Spawn
    ResultPositionSpawnController resultPositionSpawnCon;
    SpawnUIPlayer spawnUIPlayer;//UIプレイヤースポーン

    //Btns
    [SerializeField]
    Button oneMoreBtn, toTitleBtn;//各ボタンの情報
    Button nowSelectedBtn;//今選択しているボタン

    //Anim
    [SerializeField]
    float textMovingTime;//順位テキストの移動スピード
    List<Vector2> finishPosition = new List<Vector2>();//最終位置
    List<GameObject> playerScoreTexts = new List<GameObject>();//プレイヤースコア表示テキスト

    //SceneState
    ResultSceneState sceneState = ResultSceneState.RankAnim;

    // Use this for initialization
    void Awake()
    {
        //接続プレイヤー数を取得
        if (connectedPlayerStatus == null)
        {
            // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }
        //プレイヤーランク
        playerRank = GameObject.Find("PlayerRankController").GetComponent<PlayerRank>();

        SetRankUIsPosition();//ランクUI
        _playerRankList = SetPlayersRank();//順位付け
        SetPlayerRankText();//プレイヤーランクテキスト
        SetSpawnUIPlayer();//スポーンUIプレイヤー
    }

     /// <summary>
    /// シーンの状態に沿ってメソッド実行
    /// </summary>
    public override void CheckSceneState()
    {
        //XInput
        currentState = GamePad.GetState(playerIndex);

        switch (sceneState)
        {
            case ResultSceneState.FadeIn://フェードイン中
                if (fadeController.IsFadeInFinish)
                    sceneState = ResultSceneState.RankAnim;
                break;

            case ResultSceneState.RankAnim://アニメ中
                StartCoroutine(ShowRankCoroutine());
                break;

            case ResultSceneState.None://基準状態
                moveX = currentState.ThumbSticks.Left.X;
                ResultXInput();
                break;

            case ResultSceneState.ToNextScene://次のシーンに移行
                //NextSceneLoad
                if (fadeController.IsFadeOutFinish)
                    gameLoad.LoadingStartWithOBJ();
                break;
        }

        previousState = currentState;
    }

    /// <summary>
    /// リザルト入力
    /// </summary>
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
        gameLoad.NextScene = GameLoad.Scenes.StageSelect;

        isSceneChange = true;
        sceneState = ResultSceneState.ToNextScene;
    }

    /// <summary>
    /// 「タイトルへ」を選んだらタイトルシーンへ
    /// </summary>
    public void ToTitleBtn()
    {
        gameLoad.NextScene = GameLoad.Scenes.Title;

        //接続プレイヤーステータス受け取りオブジェを削除
        connectedPlayerStatus.Created = false;
        Destroy(connectedPlayerStatus.transform.gameObject);

        isSceneChange = true;
        sceneState = ResultSceneState.ToNextScene;
    }

    /// <summary>
    /// ランク演出アニメーション
    /// </summary>
    /// <returns></returns>
    IEnumerator ShowRankCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        //ランキング表示アニメ
        for (int i = 0; i < _playerRankTextsList.Count; i++)
        {
            _playerRankTextsList[i].rectTransform.DOAnchorPos(finishPosition[i], textMovingTime);

            //最下位のアニメーションの終わる時間に合わせて
            if (i == _playerRankTextsList.Count - 1)
            {
                yield return new WaitForSeconds(textMovingTime - 0.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        //アニメ終わったらoneMore選択
        oneMoreBtn.Select();
        nowSelectedBtn = oneMoreBtn;
        //SceneState移行
        if(sceneState == ResultSceneState.RankAnim)
            sceneState = ResultSceneState.None;
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

    /// <summary>
    /// ランクUI初期設定
    /// </summary>
    void SetRankUIsPosition()
    {
        //ランク生成初期位置を決定しランクOBJを生成
        resultPositionSpawnCon = GetComponent<ResultPositionSpawnController>();
        resultPositionSpawnCon.SetRanksDefaltPosition(connectedPlayerStatus.ConnectedPlayer.Count);
        resultPositionSpawnCon.SetRanksFinishPosition(connectedPlayerStatus.ConnectedPlayer.Count);
        // アニメ用終点位置を渡す
        finishPosition = resultPositionSpawnCon.FinishPositionsList;
        //テキストコンポーネントを渡す
        for (int i = 0; i < resultPositionSpawnCon.RankOBJList.Count; i++)
        {
            _playerRankTextsList.Add(resultPositionSpawnCon.RankOBJList[i].GetComponent<Text>());
        }
    }
}
