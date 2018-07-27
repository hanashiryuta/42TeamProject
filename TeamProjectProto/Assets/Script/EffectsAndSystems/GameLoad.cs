/*
 * 作成日：180604
 * ゲーム非同期ロード
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLoad : MonoBehaviour
{
    GameObject loadObj;//ロード表示UIOBJ
    Slider slider;//ロード表示スライダー

    bool _isLoadStart = false;// ロード始めたか？

    [SerializeField]
    Scenes _nextScene;//次のシーン

    /// <summary>
    /// 次のシーン
    /// </summary>
    public Scenes NextScene
    {
        get { return _nextScene; }
        set { _nextScene = value; }
    }

    /// <summary>
    /// シーン(ビルド順に合わせて)
    /// </summary>
    public enum Scenes
    {
        Title,          // タイトル
        CharacterSelect,// キャラセレクト
        StageSelect,    // ステージセレクト
        Main,           // メインゲーム
        Result          // リザルト
    }

    void Start()
    {
        loadObj = GameObject.Find("LoadingObj").gameObject;
        slider = loadObj.GetComponentInChildren<Slider>();
    }

    /// <summary>
    /// ロードOBJ付きの非同期ロード
    /// </summary>
    public void LoadingStartWithOBJ()
    {
        if (!_isLoadStart)
        {
            //ロード画面UIを表示する
            loadObj.GetComponent<CanvasGroup>().alpha = 1;
            //ロードコルーチン開始
            StartCoroutine(LoadData());
            _isLoadStart = true;
        }
    }

    /// <summary>
    /// ロードOBJ付きの非同期ロード(遅延付き)
    /// </summary>
    /// <param name="delayTime"></param>
    public void LoadingStartWithOBJ(float delayTime)
    {
        Invoke("LoadingStartWithOBJ", delayTime);
    }

    /// <summary>
    /// 普通にロード
    /// </summary>
    public void LoadingStartWithoutOBJ()
    {
        if (!_isLoadStart)
        {
            SceneManager.LoadScene((int)_nextScene);
            _isLoadStart = true;
        }
    }

    /// <summary>
    /// 普通にロード(遅延付き)
    /// </summary>
    /// <param name="delayTime"></param>
    public void LoadingStartWithoutOBJ(float delayTime)
    {
        Invoke("LoadingStartWithoutOBJ", delayTime);
    }

    /// <summary>
    /// 非同期ロード
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadData()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync((int)_nextScene);
        async.allowSceneActivation = false;// シーン遷移をしない

        //　読み込みが終わるまで進捗状況をスライダーの値に反映させる
        while (async.progress < 0.9f)
        {
            slider.value = async.progress;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Scene Loaded");

        // ロード完了時進捗スライダーの値をMAXに
        slider.value = 1;

        yield return new WaitForSeconds(0.5f);

        async.allowSceneActivation = true;// シーン遷移許可
    }
}
