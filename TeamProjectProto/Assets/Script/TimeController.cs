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

/// <summary>
/// 時間状態
/// </summary>
public enum TimeState
{
    NORMALTIME,//普通の時間状態
    LOSSTIME,//ロスタイム状態
}

public class TimeController : MonoBehaviour {

    public float gameTime = 60;//ゲーム時間
    float maxGameTime;//最大ゲーム時間
    Text timeText;//時間表示テキスト
    public GameObject timeTextObject;//時間表示テキストオブジェクト
    [HideInInspector]
    public bool isEnd = false;//ゲーム終了判定

    GameObject PausePanel;//ポーズ画面がactiveかどうかを参照する

    // 追加日：180530　追加者：何
    [SerializeField]
    GameObject startCntDown; //スタートカウントダウン
    [SerializeField]
    GameObject finishCall;

    [HideInInspector]
    public bool isPause;//時間を止めるかどうか

    [HideInInspector]
    public TimeState timeState = TimeState.NORMALTIME;//時間状態
    [HideInInspector]
    public GameObject balloonMaster;//バルーン管理クラス

    public List<GameObject> cutInList;
    public List<GameObject> textList;

    [HideInInspector]
    public List<float> reelRate;//時間対応リール割合

    // Use this for initialization
    void Start()
    {
        maxGameTime = gameTime;//最大ゲーム時間設定
        reelRate = new List<float>();
        timeText = timeTextObject.GetComponent<Text>();//テキスト取得
    }

    // Update is called once per frame
    void Update() {
        //PausePanelがnullならばPausePanelを探し出して入れる
        if (PausePanel == null) {
            PausePanel = GameObject.Find("PausePanel");
        }
        
        //状態によって時間処理変える
        switch (timeState)
        {
            case TimeState.NORMALTIME:
                NormalTime();
                break;
            case TimeState.LOSSTIME:
                LossTime();
                break;
        }       
	}

    /// <summary>
    /// 普通の時間処理
    /// </summary>
    void NormalTime()
    {
        //時間表示
        timeText.text = HalfWidth2FullWidth.Set2FullWidth(gameTime.ToString("0.0"));
        //時間フラグがfalseなら//startCountDown中タイム進まない
        if (!isPause &&
            !startCntDown.GetComponent<StartCountDown>().IsCntDown &&
            !finishCall.GetComponent<FinishCall>().IsCalling)
        {
            gameTime -= Time.deltaTime;
            if (gameTime <= 0)
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

    /// <summary>
    /// 時間に応じたリール割合セットメソッド
    /// </summary>
    /// <returns></returns>
    public List<float> ReelRateSet()
    {
        //ゲーム時間でリール割合を決める
        int rate = (int)(ReelSpin.reelCount * (gameTime / maxGameTime)) - 1;
        reelRate = new List<float>
        {
            //チョキン　　　　　　　テモチ
            ReelSpin.reelCount-rate,rate
        };
        return reelRate;
    }

    /// <summary>
    /// ロスタイム処理
    /// </summary>
    void LossTime()
    {
        if(!balloonMaster.GetComponent<BalloonMaster>().isRoulette)
            LossTimeCutIn();
        timeText.text = "最後！";
        //最後のバルーンが爆発したら
        if (balloonMaster.GetComponent<BalloonMaster>().IsBlast)
        {
            isEnd = true;//終了
        }
    }

    /// <summary>
    /// ロスタイム突入
    /// </summary>
    /// <param name="second"></param>
    /// <param name="balloonMaster"></param>
    public void LossTimeStart(float second,BalloonMaster balloonMaster)
    {
        //残り時間がバルーンの爆破時間より短かったら
        if (gameTime <= second)
        {
            gameTime =second;
            balloonMaster.balloonRespawnTime = balloonMaster.originBalloonRespawnTime;
            this.balloonMaster = balloonMaster.gameObject;
            timeState = TimeState.LOSSTIME;
        }
    }

    /// <summary>
    /// ロスタイム突入時のカットイン
    /// </summary>
    void LossTimeCutIn()
    {
        float fillSpeed = 0.05f;//画像fillスピード
        Vector3 textSpeed = new Vector3(-7, 0, 0);//テキストスピード

        for (int i = 0; i < cutInList.Count; i++)
        {
            if (textList[i].GetComponent<RectTransform>().localPosition.x >= -550)
            {
                //画像fill増加処理
                cutInList[i].GetComponent<Image>().fillAmount += fillSpeed;
                //テキスト移動処理
                textList[i].GetComponent<RectTransform>().localPosition += textSpeed;
            }
            else
            {
                //画像fill減少処理
                cutInList[i].GetComponent<Image>().fillOrigin = 0;
                cutInList[i].GetComponent<Image>().fillAmount -= fillSpeed;
            }
        }
    }
}
