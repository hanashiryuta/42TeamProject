/*
 * 作成日時：180621
 * BGMコントローラー
 * 作成者：何承恩
 */
 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMController : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> bgmList = new List<AudioClip>();//BGM格納リスト
    public AudioSource bgmAudio;// AudioSource
    float defaultVolume = 0.5f;//デフォルト音量

    public static bool isCreated = false;//生成されたか？
    [HideInInspector]
    public string preScene;//前のシーン

    RouletteController rouletteCon;//ルーレットコントローラー
    bool isGetRoulette = false;//ルーレットを取得したか？

    TutorialController tutorialController;//チュートリアルコントローラー

    /// <summary>
    /// BGM順番
    /// </summary>
    enum BGM
    {
        Title,  //タイトルシーン
        Main,   //メインゲームシーン
        Result, //リザルトシーン
        Tutorial//チュートリアル
    }

    void Awake()
    {
        //シーン切替を検知
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;

        //1つしか存在しない
        if (!isCreated)
        {
            SetBGM((int)BGM.Title);
            bgmAudio.Play();

            DontDestroyOnLoad(this.gameObject);
            isCreated = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

        //チュートリアルコントローラー
        tutorialController = GameObject.Find("TutorialController").GetComponent<TutorialController>();
        tutorialController.bgmController = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
        // mainシーンで
        if(SceneManager.GetActiveScene().name == "main")
        {
            //ルーレットがあったら
            if (GameObject.Find("Roulette(Clone)") != null)
            {
                if (!isGetRoulette)
                {
                    rouletteCon = GameObject.Find("Roulette(Clone)").GetComponent<RouletteController>();
                    isGetRoulette = true;
                }
                BGMVolumeChange();
            }
            else
            {
                rouletteCon = null;
                isGetRoulette = false;
            }
        }
	}

    /// <summary>
    /// BGMの音量調整(ルーレット中)
    /// </summary>
    void BGMVolumeChange()
    {
        //ルーレット入場時
        if(rouletteCon.rouletteState == RouletteState.ENTRY)
        {
            bgmAudio.volume = 0.2f;
        }
        //ルーレット退場時
        if (rouletteCon.rouletteState == RouletteState.EXIT)
        {
            bgmAudio.volume = defaultVolume;
        }
    }

    /// <summary>
    /// 今のシーンのBGMをセット
    /// </summary>
    /// <param name="newScene">今のシーン</param>
    /// <param name="preScene">前のシーン</param>
    public void SetSceneBGM(Scene newScene, string preScene)
    {
        switch (newScene.name)
        {
            case "Title": //タイトル
                SetBGM((int)BGM.Title);
                bgmAudio.Play();
                break;

            case "CharacterSelect": //キャラセレクト
                break;

            case "StageSelect": //ステージセレクト
                if(preScene == "Result")//リザルトシーンの「もう一回」ボタンで来たら
                {
                    SetBGM((int)BGM.Title);
                    bgmAudio.Play();
                }
                break;

            case "main"://ゲームメインシーン
                if (!tutorialController.isTutorial)//チュートリアルがなかった場合
                {
                    SetBGM((int)BGM.Main);

                    StartCountDown scd = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
                    bgmAudio.PlayDelayed(scd.waitTime + 4f);//カウントダウンが終わったらプレイ
                }
                else
                {
                    SetBGM((int)BGM.Tutorial);
                    bgmAudio.Play();
                }
                break;

            case "Result"://リザルト
                SetBGM((int)BGM.Result);
                bgmAudio.Play();
                break;
        }
    }

    /// <summary>
    /// BGMをセット
    /// </summary>
    /// <param name="listIndex"></param>
    void SetBGM(int listIndex)
    {
        bgmAudio.clip = bgmList[listIndex];
    }

    /// <summary>
    /// シーンが廃棄された時呼ばれるメソッド
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void SceneUnloaded(Scene scene)
    {
        Debug.Log(scene.name + " Unloaded");
        //アンロードされたシーンの名前を取得
        if(preScene != null)
            preScene = scene.name;
    }

    /// <summary>
    /// シーンが呼ばれた時呼ばれるメソッド
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " Loaded");
        //BGM設定
        SetSceneBGM(scene, preScene);
    }
}
