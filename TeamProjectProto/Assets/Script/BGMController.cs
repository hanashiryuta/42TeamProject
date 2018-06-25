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
    AudioSource audio;// AudioSource

    AudioClip nowClip;
    AudioClip nextClip;
    float nowClipVolume = 1.0f;
    float nextClipVolume = 0.0f;
    bool nextClipPlaying = false;

    static bool created = false;
    bool isFading = false;

    string preScene;//前のシーン

    /// <summary>
    /// BGM順番
    /// </summary>
    enum BGM
    {
        Title,
        Main,
        Result
    }

    void Awake()
    {
        //シーン切替を検知
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;

        //1つしか存在しない
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }


        audio = transform.GetComponent<AudioSource>();

        SetNowAndNextClip((int)BGM.Title);
        audio.clip = nowClip;
        audio.Play();
    }
	
	// Update is called once per frame
	void Update ()
    {
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

    void ChangeBGM(AudioClip nextBGM)
    {
        audio.clip = nextBGM;
    }

    void SetSceneBGM(Scene newScene, string preScene)
    {
        if (newScene.name == "Title")
        {
            SetNowAndNextClip((int)BGM.Title);
            audio.clip = nowClip;
            audio.Play();
        }
        else if (newScene.name == "CharacterSelect")
        {
        }
        else if (newScene.name == "StageSelect" && preScene == "Result")//もう一回で来たら
        {
            SetNowAndNextClip((int)BGM.Title);
            audio.clip = nowClip;
            audio.Play();

        }
        else if (newScene.name == "main")
        {
            SetNowAndNextClip((int)BGM.Main);
            audio.clip = nowClip;
            audio.Play();

        }
        else if (newScene.name == "Result")
        {
            SetNowAndNextClip((int)BGM.Result);
            audio.clip = nowClip;
            audio.Play();
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
            audio.volume = nextClipVolume;
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
            audio.volume = nowClipVolume;
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
        Debug.Log(scene.name + "Unloaded");
        //音フェード
        //StartCoroutine(Fade());
        preScene = scene.name;
    }

    /// <summary>
    /// シーンが呼ばれた時呼ばれるメソッド
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + "Loaded");
        //BGM設定
        Debug.Log(preScene + "が前のシーン");

        SetSceneBGM(scene, preScene);
    }
}
