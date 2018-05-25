//
//作成日時：4月29日
//プレイヤーの順位付けクラス
//作成者：平岡誠司
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerRank : MonoBehaviour
{
    // 0525 編集者：何
	GameObject[] _playerRankArray;//順位付け用のリスト(GameScene)
    public GameObject[] PlayerRankArray
    {
        get { return _playerRankArray; }
    }
    // 0525 編集者：何
    List<string> _resultRank;//順位付け用のリスト(ResultScene)
    public List<string> ResultRank
    {
        get { return _resultRank; }
        set { _resultRank = value; }
    }

    static bool created = false;

    // 0525 編集者：何
    bool _isInPlay = true; // ゲーム中か
    public bool IsInPlay
    {
        get { return _isInPlay; }
        set { _isInPlay = value; }
    }

	/// <summary>
	/// 1つだけを生成
	/// </summary>
	void Awake()
    {
		if (!created)
        {
			DontDestroyOnLoad (this.gameObject);
			created = true;
		}
        else
        {
			Destroy (this.gameObject);
		}

        _playerRankArray = GameObject.FindGameObjectsWithTag("Player");
        _resultRank = null;
        //Reset ();
    }

    // Use this for initialization
    void Start ()
    {
    }
	
	// Update is called once per frame
	void Update ()
	{
        // ゲーム中か
        if (_isInPlay)
        {
            SetPlayerRank();
            SetCrown();
        }

        //Result画面の時、左側から1位、2位、3位、4位
        if (SceneManager.GetActiveScene ().name == "Result")
        {
//			for (int i = 0; i < playerRankList.Count; i++) {
//				playerRankList [i].SetActive (true);
//				playerRankList [i].transform.position =
//					new Vector3 (4.5f + i * -3.0f, 0, 0);
//				}

			foreach (var rank in _resultRank)
            {
				Debug.Log (rank);
			}
		}
	}

    /// <summary>
    /// 作成日：180525
    /// 作成者：何承恩
    /// プレイヤーのランク付け
    /// </summary>
    void SetPlayerRank()
    {
        //ソート（大きい順に）
        for (int i = 0; i < _playerRankArray.Length - 1; i++)
        {
            for (int j = i + 1; j < _playerRankArray.Length; j++)
            {
                if (_playerRankArray[i].GetComponent<PlayerMove>().totalBlastCount < _playerRankArray[j].GetComponent<PlayerMove>().totalBlastCount)
                {
                    GameObject p = _playerRankArray[j];
                    _playerRankArray[j] = _playerRankArray[i];
                    _playerRankArray[i] = p;
                }
            }
        }
    }

    /// <summary>
    /// 作成日：180525
    /// 作成者：何承恩
    /// 一位に王冠を付ける
    /// </summary>
    void SetCrown()
    {
        foreach (var player in _playerRankArray)
        {
            // 1位の得点が0 => 全得点が0 なので
            if (_playerRankArray[0].GetComponent<PlayerMove>().totalBlastCount == 0)
            {
                //全プレイヤーの王冠は消す
                player.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/Bone.004_end/Crown").GetComponent<MeshRenderer>().enabled = false;
            }
            else
            {
                //1位の王冠を見えるようにする
                //1位タイも王冠を見えるようにする
                if (player.GetComponent<PlayerMove>().totalBlastCount >= _playerRankArray[0].GetComponent<PlayerMove>().totalBlastCount)
                {
                    player.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/Bone.004_end/Crown").GetComponent<MeshRenderer>().enabled = true;
                }
                //それ以外のプレイヤーの王冠は消す
                else
                {
                    player.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/Bone.004_end/Crown").GetComponent<MeshRenderer>().enabled = false;
                }
            }

        }
    }

    /// <summary>
    /// リストの要素を全て削除する処理
    /// </summary>
    public void Reset()
    {
//		if (transform.childCount >= 0) {
//			foreach (Transform child in gameObject.transform) {
//				Destroy (child.gameObject);
//			}

//			for (int i = 0; i < gameObject.transform.childCount; i++) {
//				Destroy (transform.GetChild (i));
//
//				playerRankList.RemoveAt (i);

//			}

			//_playerRankArray.Clear ();
	}
}
