//
//作成日時：5月7日
//時間管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TimeController : MonoBehaviour {

    public float gameTime = 60;//ゲーム時間
    Text timeText;//時間表示テキスト
    public GameObject timeTextObject;//時間表示テキストオブジェクト
    [HideInInspector]
    public bool isEnd = false;//ゲーム終了判定

    GameObject PausePanel;//ポーズ画面がactiveかどうかを参照する
<<<<<<< HEAD

    // 追加日：180530　追加者：何
    [SerializeField]
    GameObject startCntDown; //スタートカウントダウン
    [SerializeField]
    GameObject finishCall;
=======
>>>>>>> parent of bb982be... Merge branch 'PresenProtoType' into Tokisaki_Branch_Light
    
    // Use this for initialization
    void Start () {
        timeText = timeTextObject.GetComponent<Text>();//テキスト取得
    }
	
	// Update is called once per frame
	void Update () {
        //時間表示
        //timeText.text = "time "+gameTime.ToString("0.0");
        timeText.text = gameTime.ToString("0.0"); 
        //5月19日
        //ポーズ画面の表示に合わせてgameTimeの処理を止める
        //追記者：安部崇寛
        //
        //PausePanelがnullならばPausePanelを探し出して入れる
        if (PausePanel == null) { 
            PausePanel = GameObject.Find("PausePanel");
        }

<<<<<<< HEAD
        //ポーズ画面のactive状態でタイムを進めるか判定   //startCountDown中タイム進まない
        if ((PausePanel == null ||!PausePanel.active) &&
            !startCntDown.GetComponent<StartCountDown>().IsCntDown &&
            !finishCall.GetComponent<FinishCall>().IsCalling)
=======
        if (PausePanel == null ||!PausePanel.active)//ポーズ画面のactive状態でタイムを進めるか判定
>>>>>>> parent of bb982be... Merge branch 'PresenProtoType' into Tokisaki_Branch_Light
        {
            gameTime -= Time.deltaTime;
            if(gameTime <= 0)
            {
                //時間来たらゲーム終了判定
                gameTime = 0;
                isEnd = true;
            }
        }

        //10秒以下になったら赤色
        if (gameTime <= 10)
        {
            timeText.color = Color.red;
        }
	}
}
