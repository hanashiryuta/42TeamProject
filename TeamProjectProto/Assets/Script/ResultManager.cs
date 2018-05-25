//
//作成日時：4月25日
//リザルト画面クラス
//作成者：平岡誠司
//
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
    Text player1, player2, player3, player4;//順位表示のテキスト

	// Use this for initialization
	void Start () {
		playerRank = GameObject.Find ("PlayerRankController");

        backtoGame = GameObject.Find("/Canvas/BacktoGame").GetComponent<Button>();
        endGame = GameObject.Find("/Canvas/EndGame").GetComponent<Button>();

        player1 = GameObject.Find("/Canvas/PlayerRank1").GetComponent<Text>();
        player2 = GameObject.Find("/Canvas/PlayerRank2").GetComponent<Text>();
        player3 = GameObject.Find("/Canvas/PlayerRank3").GetComponent<Text>();
        player4 = GameObject.Find("/Canvas/PlayerRank4").GetComponent<Text>();

        count = -1;
    }

    // Update is called once per frame
    void Update()
    
		//上から順位順に名前表示
		player1.text = "1位:"+playerRank.GetComponent<PlayerRank> ().ResultRank[0];
		player2.text = "2位:"+playerRank.GetComponent<PlayerRank> ().ResultRank[1];
		player3.text = "3位:"+playerRank.GetComponent<PlayerRank> ().ResultRank[2];
		player4.text = "4位:"+playerRank.GetComponent<PlayerRank> ().ResultRank[3];

        //上から順位順に名前表示
        //player1.text = "1位:" + playerRank.GetComponent<PlayerRank>().GetFirstPlayer();
        //player2.text = "2位:" + playerRank.GetComponent<PlayerRank>().GetSecondPlayer();
        //player3.text = "3位:" + playerRank.GetComponent<PlayerRank>().GetThirdPlayer();
        //player4.text = "4位:" + playerRank.GetComponent<PlayerRank>().GetFourthPlayer();

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
            endGame.Select ();//1の時、ゲーム終了を選択状態にする
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
}
