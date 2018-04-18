//
//作成日時：4月18日
//シーン管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour {

    GameObject balloon;//爆発物

	// Use this for initialization
	void Start () {
        balloon = GameObject.FindGameObjectWithTag("Balloon");//爆発物取得
    }
	
	// Update is called once per frame
	void Update () {

        //爆発物がなければ探す
        if(balloon == null)
        {
            balloon = GameObject.FindGameObjectWithTag("Balloon");
            return;
        }


        //プレイヤーの人数が1人になったらシーンをリロードする
        if (balloon.GetComponent<BalloonController>().isEnd)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
		
	}
}
