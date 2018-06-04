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
    Button backtoGame, endGame;//各ボタンの情報
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

	// Use this for initialization
	void Start ()
    {
		playerRank = GameObject.Find ("PlayerRankController");

        backtoGame = GameObject.Find("BacktoGame").GetComponent<Button>();
        endGame = GameObject.Find("EndGame").GetComponent<Button>();

        for(int i = 0; i < playerRankTexts.Length; i++)
        {
            playerRankTexts[i].transform.position = DefaltPosition[i].transform.position;
        }

        count = -1;

        //上から順位順に名前表示
        for (int i = 0; i < playerRankTexts.Length; i++)
        {
            playerRankTexts[i].text = (i + 1) + "位:" + playerRank.GetComponent<PlayerRank>().ResultRank[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
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
                backtoGame.Select();//-1の時、ゲームに戻るを選択状態にする
            }
            else if (count == 1)
            {
                endGame.Select();//1の時、ゲーム終了を選択状態にする
            }
        }
    }

    /// <summary>
    /// 「もう1度遊ぶ」を選んだらゲームシーンに戻す処理
    /// </summary>
    public void BackGame()
    {
        SceneManager.LoadScene("main");
    }

    /// <summary>
    /// 「ゲーム終了」を選んだらウィンドウを閉じる処理（.exe形式のみ）
    /// </summary>
    public void EndGame()
    {
        //Application.Quit ();
        SceneManager.LoadScene("Title");
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




}
