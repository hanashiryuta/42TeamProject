//
//5月25日
//タイトルシーン用セレクトスクリプト
//作成者：安部崇寛
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleSceneSelectScript : MonoBehaviour {

    public Button exit;
    public Button gameStart;

	// Use this for initialization
	void Start () {
        gameStart.Select();
	}
	
    public void GameStart() {
        GetComponent<TitleScene>().isSceneChange = true;
    }

    public void GameExit() {
        Application.Quit();
    }

    public void GameCredit()
    {

    }
}
