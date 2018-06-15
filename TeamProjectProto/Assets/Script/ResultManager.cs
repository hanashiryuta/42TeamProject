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

public class ResultManager : MonoBehaviour
{
    GameObject playerRank;//Playerの名前取得用
    Button oneMore, endGame;//各ボタンの情報
    float count;//選んでいるボタンの取得用
    [SerializeField]
    float movingTime = 2f;
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

    // Use this for initialization
    void Awake ()
    {
		playerRank = GameObject.Find ("PlayerRankController");

        oneMore = GameObject.Find("OneMore").GetComponent<Button>();
        endGame = GameObject.Find("EndGame").GetComponent<Button>();

        for(int i = 0; i < playerRankTexts.Length; i++)
        {
            playerRankTexts[i].transform.position = DefaltPosition[i].transform.position;
        }

        count = -1;

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
            playerRankTexts[i].text = (i + 1) + "位:" + playerRank.GetComponent<PlayerRank>().ResultRank[i];
        }

        //スポーンUIプレイヤー
        spawnUIPlayer = transform.GetComponent<SpawnUIPlayer>();
        spawnUIPlayer.ConnectedPLStatus = connectedPlayerStatus;
        spawnUIPlayer.PList = playerRank.GetComponent<PlayerRank>().ResultRank;

        //fade
        fadeController = GameObject.Find("FadePanel").GetComponent<FadeController>();

        //load
        gameLoad = transform.GetComponent<GameLoad>();
    }

    // Update is called once per frame
    void Update()
    {
        //fadein
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }

        if (_isAnim)
        {
            StartCoroutine(ShowRankCoroutine());
        }
        else
        {
            count += Input.GetAxisRaw("Horizontal1");

            if (count > 0.1f)
            {
                count = 1;//0.1を超えたら1にする
            }
            else if (count < -0.1f)
            {
                count = -1;//-0.1を超えたら-1にする
            }

            if (count == -1)
            {
                oneMore.Select();//-1の時、ゲームに戻るを選択状態にする
            }
            else if (count == 1)
            {
                endGame.Select();//1の時、ゲーム終了を選択状態にする
            }
        }

        //fadeout
        if (isSceneChange)
        {
            fadeController.FadeOut();

            if (fadeController.IsFadeOutFinish && !isFadeOuted)
            {
                gameLoad.LoadingStartWithOBJ();
                isFadeOuted = true;
            }
        }

    }

    /// <summary>
    /// 「もう1度遊ぶ」を選んだらキャラ生成シーンに戻す処理
    /// </summary>
    public void OneMoreBtn()
    {
        gameLoad.NextScene = GameLoad.Scene.CharacterSelect;
        isSceneChange = true;
    }

    /// <summary>
    /// 「タイトルへ」を選んだらウィンドウを閉じる処理（.exe形式のみ）
    /// </summary>
    public void ToTitleBtn()
    {
        gameLoad.NextScene = GameLoad.Scene.Tilte;
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

        for(int i = 0; i < playerRankTexts.Length; i++)
        {
            playerRankTexts[i].transform.DOMove(FinishPosition[i].transform.position, movingTime);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(1f);
        _isAnim = false;
    }



    void ToCharaSelectScene()
    {
        SceneManager.LoadScene("CharacterSelect");
    }

    void ToGameTitle()
    {
        SceneManager.LoadScene("Title");
    }
}
