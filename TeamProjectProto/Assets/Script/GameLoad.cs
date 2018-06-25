/*
 * 作成日：180604
 * ゲーム非同期ロード
 * 作成者：阿部→何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoad : MonoBehaviour
{
    public GameObject loadObj;
    public Slider slider;

    [SerializeField]
    Scene _nextScene;
    public Scene NextScene
    {
        get { return _nextScene; }
        set { _nextScene = value; }
    }

    public enum Scene
    {
        Tilte,
        CharacterSelect,
        StageSelect,
        Main,
        Result
    }

    public GameLoad()
    {

    }

    /// <summary>
    /// ロードオブジ付きの非同期ロード
    /// </summary>
    public void LoadingStartWithOBJ()
    {
        //　ロード画面UIをアクティブにする
        loadObj.SetActive(true);

        StartCoroutine(LoadData());
    }

    /// <summary>
    /// 非同期ロード
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadData()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync((int)_nextScene);
        async.allowSceneActivation = false;    // シーン遷移をしない

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (async.progress < 0.9f)
        {
            Debug.Log(async.progress);
            slider.value = async.progress;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Scene Loaded");

        slider.value = 1;

        yield return new WaitForSeconds(1);

        async.allowSceneActivation = true;    // シーン遷移許可
    }


    public void LoadingStartWithoutOBJ()
    {
        SceneManager.LoadSceneAsync((int)_nextScene);
    }

}
