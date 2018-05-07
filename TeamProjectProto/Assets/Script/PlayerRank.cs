//
//作成日時：4月29日
//プレイヤーの順位付けクラス
//作成者：平岡誠司
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRank : MonoBehaviour {
    [HideInInspector]
	public List<string> playerRankList;//順位付け用のリスト
	static bool created=false;

	/// <summary>
	/// 1つだけを生成
	/// </summary>
	void Awake(){
		if (!created) {
			DontDestroyOnLoad (this.gameObject);
			created = true;
		} else {
			Destroy (this.gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		playerRankList = new List<string> ();
		Reset ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Result画面の時、左側から1位、2位、3位、4位
		if (SceneManager.GetActiveScene ().name == "Result") {
//			for (int i = 0; i < playerRankList.Count; i++) {
//				playerRankList [i].SetActive (true);
//				playerRankList [i].transform.position =
//					new Vector3 (4.5f + i * -3.0f, 0, 0);
//				}

			foreach (var rank in playerRankList) {
				Debug.Log (rank);
			}
		}
	}

	/// <summary>
	/// Playerの名前をリストに追加する処理
	/// </summary>
	/// <param name="player">爆発した時に風船を持っていたPlayer</param>
	public void SetPlayer(GameObject player){
		playerRankList.Add (player.name);
		//Destroy (player);
	}

	/// <summary>
	/// リストの要素を全て削除する処理
	/// </summary>
	public void Reset(){
//		if (transform.childCount >= 0) {
//			foreach (Transform child in gameObject.transform) {
//				Destroy (child.gameObject);
//			}

//			for (int i = 0; i < gameObject.transform.childCount; i++) {
//				Destroy (transform.GetChild (i));
//
//				playerRankList.RemoveAt (i);

//			}

			playerRankList.Clear ();
		}

	/// <summary>
	/// 1位のPlayerの名前を返す
	/// </summary>
	/// <returns>The first player.</returns>
	public string GetFirstPlayer(){
		return playerRankList [3];
	}

	/// <summary>
	/// 2位のPlayerの名前を返す
	/// </summary>
	/// <returns>The second player.</returns>
	public string GetSecondPlayer(){
		return playerRankList [2];
	}

	/// <summary>
	/// 3位のPlayerの名前を返す
	/// </summary>
	/// <returns>The third player.</returns>
	public string GetThirdPlayer(){
		return playerRankList [1];
	}

	/// <summary>
	/// 最下位のPlayerの名前を返す
	/// </summary>
	/// <returns>The fourth player.</returns>
	public string GetFourthPlayer(){
		return playerRankList [0];
	}
	}
