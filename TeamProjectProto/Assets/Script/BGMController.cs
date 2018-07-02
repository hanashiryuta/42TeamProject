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
    [SerializeField]
    AudioClip rouletteBGM;//ルーレット用BGM（仕様がちょっと違う）
    public AudioSource bgmAudio;// AudioSource
    float defaultVolume = 0.5f;//デフォルト音量

    AudioClip nowClip;
    AudioClip nextClip;
    float nowClipVolume = 1.0f;
    float nextClipVolume = 0.0f;

    public static bool created = false;
    bool isFading = false;

    string preScene;//前のシーン
    RouletteController rouletteCon;//ルーレットコントローラー
    bool isRoulette = false;//ルーレット中か？

    TutorialController tutorialController;//チュートリアルコントローラー

    /// <summary>
    /// BGM順番
    /// </summary>
    enum BGM
    {
        Title,
        Main,
        Result,
        Tutorial
    }

    void Awake()
    {
        //シーン切替を検知
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;

        //1つしか存在しない
        if (!created)
        {
            SetNowAndNextClip((int)BGM.Title);
            bgmAudio.clip = nowClip;
            bgmAudio.Play();

            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

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
                if (!isRoulette)
                {
                    rouletteCon = GameObject.Find("Roulette(Clone)").GetComponent<RouletteController>();
                }
                MainBGMsVolumeChange();
            }
            else
            {
                rouletteCon = null;
                isRoulette = false;
            }
        }

        //FadeOut();

        //if(nowClipVolume <= 0.1f)
        //{
        //    if (!nextClipPlaying)
        //    {
        //        audio.clip = nextClip;
        //        audio.Play();
        //        nextClipPlaying = true;
        //    }
        //}

        //FadeIn();
	}

    /// <summary>
    /// mainBGMの音量調整
    /// </summary>
    void MainBGMsVolumeChange()
    {
        if(rouletteCon.rouletteState == RouletteState.ENTRY)
        {
            bgmAudio.volume = 0.2f;
        }

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
        if (newScene.name == "Title")//タイトル
        {
            SetNowAndNextClip((int)BGM.Title);
            bgmAudio.clip = nowClip;
            bgmAudio.Play();
        }
        else if (newScene.name == "CharacterSelect")//キャラセレクト
        {
        }
        else if (newScene.name == "StageSelect" && //ステージセレクト
                preScene == "Result")//もう一回で来たら
        {
            SetNowAndNextClip((int)BGM.Title);
            bgmAudio.clip = nowClip;
            bgmAudio.Play();

        }
        else if (newScene.name == "main")//ゲームメインシーン
        {
            if (!tutorialController.isTutorial)
            {
                SetNowAndNextClip((int)BGM.Main);
                bgmAudio.clip = nowClip;

                StartCountDown scd = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
                bgmAudio.PlayDelayed(scd.waitTime + 4f);//カウントダウンが終わったらプレイ
            }
            else
            {
                SetNowAndNextClip((int)BGM.Tutorial);
                bgmAudio.clip = nowClip;

                bgmAudio.Play();
            }
        }
        else if (newScene.name == "Result")//リザルト
        {
            SetNowAndNextClip((int)BGM.Result);
            bgmAudio.clip = nowClip;
            bgmAudio.Play();
        }
    }

    /// <summary>
    /// 現在と次のBGMをセット
    /// </summary>
    /// <param name="listIndex"></param>
    void SetNowAndNextClip(int listIndex)
    {
        //リスト最後だったら
        if(listIndex == bgmList.Count - 1)
        {
            nowClip = bgmList[listIndex];
            nextClip = bgmList[0];
        }
        else
        {
            nowClip = bgmList[listIndex];
            nextClip = bgmList[listIndex + 1];
        }
    }

    IEnumerator Fade()
    {
        if (isFading) { yield break; }
        isFading = true;

        var fadeOutCoroutine = StartCoroutine(FadeOut());
        var fadeInCoroutine = StartCoroutine(FadeIn());

        yield return fadeOutCoroutine;
        yield return fadeInCoroutine;

        isFading = false;
    }

    IEnumerator FadeIn()
    {
        if (nextClipVolume < 1) 
        {
            nextClipVolume += 0.1f * Time.deltaTime;
            bgmAudio.volume = nextClipVolume;
        }
        else
        {
            yield break;
        }
    }

    IEnumerator FadeOut()
    {
        if (nowClipVolume > 0.1f) 
        {
            nowClipVolume -= 0.1f * Time.deltaTime;
            bgmAudio.volume = nowClipVolume;
        }
        else
        {
            yield break;
        }
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
