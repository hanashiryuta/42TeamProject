using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoad : MonoBehaviour
{
    public GameObject loadObj;
    public Slider slider;

    public GameLoad()
    {

    }

    public void LoadingStart()
    {
        //　ロード画面UIをアクティブにする
        loadObj.SetActive(true);

        StartCoroutine(LoadData());
    }

    private IEnumerator LoadData()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("CharacterSelect");
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
    
}
