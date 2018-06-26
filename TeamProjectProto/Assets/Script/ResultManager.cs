//
//作成日時：4月25日
//リザルト画面クラス
//作成者：平岡誠司
//
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class ResultManager : MonoBehaviour
{
    GameObject playerRank;//Playerの名前取得用
    [SerializeField]
    Button oneMoreBtn, toTitleBtn;//各ボタンの情報
    Button nowSelectedBtn;

    [SerializeField]
    float textMovingTime = 2f;//順位テキストの移動スピード
    [SerializeField]
    Text[] playerRankTexts;//順位表示のテキスト
    [SerializeField]
    GameObject[] DefaltPosition;//初期位置
    [SerializeField]
    GameObject[] FinishPosition;//最終位置

    bool _isAnim = true;//アニメ中か

    ConnectedPlayerStatus connectedPlayerStatus;//接続したプレイヤー
    SpawnUIPlayer spawnUIPlayer;//UIプレイヤースポーン

    //fade
    FadeController fadeController;
    bool isFadeOuted = false;

    bool isOneMore = false;
    bool isTitle = false;

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

    // Use this for initialization
    void Awake()
    {
        playerRank = GameObject.Find("PlayerRankController");

        for (int i = 0; i < playerRankTexts.Length; i++)
        {
            playerRankTexts[i].transform.position = DefaltPosition[i].transform.position;
        }

        if (connectedPlayerStatus == null)
        {
            // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }

        for (int i = 0; i < playerRankTexts.Length; i++)
        {
            playerRankTexts[i].text = "";
        }
        //上から順位順に名前表示
        //接続しているプレイヤー数だけ表示する
        for (int i = 0; i < connectedPlayerStatus.ConnectedPlayer.Count; i++)
        {
            playerRankTexts[i].text = HalfWidth2FullWidth.Set2FullWidth((i + 1).ToString()) + " 位:";// + playerRank.GetComponent<PlayerRank>().ResultRank[i];
        }

        //スポーンUIプレイヤー
        spawnUIPlayer = transform.GetComponent<SpawnUIPlayer>();
        spawnUIPlayer.ConnectedPLStatus = connectedPlayerStatus;
        spawnUIPlayer.PList = playerRank.GetComponent<PlayerRank>().ResultRank;
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

        for (int i = 0; i < playerRankTexts.Length; i++)
        {
            playerRankTexts[i].transform.DOMove(FinishPosition[i].transform.position, textMovingTime);
            yield return new WaitForSeconds(0.5f);
        }
        //yield return new WaitForSeconds(1f);
        //アニメ終わったらoneMore選択
        oneMoreBtn.Select();
        nowSelectedBtn = oneMoreBtn;

        _isAnim = false;
    }

    /// <summary>
    /// 操作可能なプレイヤーを選択
    /// /// </summary>
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
}
