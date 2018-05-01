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

public class ResultManager : MonoBehaviour {
	Button backtoGame, endGame;//各ボタンの情報
	float count;//選んでいるボタンの取得用

	// Use this for initialization
	void Start () {
		backtoGame = GameObject.Find ("/Canvas/BacktoGame").GetComponent<Button> ();
		endGame = GameObject.Find ("/Canvas/EndGame").GetComponent<Button> ();

		count = -1;
	}
	
	// Update is called once per frame
	void Update () {
		count += Input.GetAxisRaw ("Horizontal1");

		if (count > 0.1f) {
			count = 1;//0.1を超えたら1にする
		} else if (count < -0.1f) {
			count = -1;//-0.1を超えたら-1にする
		}

		if (count == -1) {
			backtoGame.Select ();//-1の時、ゲームに戻るを選択状態にする
		} else if (count == 1) {
			endGame.Select ();//1の時、ゲーム終了を選択状態にする
		}
	}

	/// <summary>
	/// 「もう1度遊ぶ」を選んだらゲームシーンに戻す処理
	/// </summary>
	public void BackGame(){
		SceneManager.LoadScene ("main");
	}

	/// <summary>
	/// 「ゲーム終了」を選んだらウィンドウを閉じる処理（.exe形式のみ）
	/// </summary>
	public void EndGame(){
        //Application.Quit ();
        SceneManager.LoadScene("Title");
	}
}
