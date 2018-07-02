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
    public GameObject loadObj;//ロード表示UIOBJ
    public Slider slider;//ロード表示スライダー

    bool _isLoadStart = false;// ロード始めたか？

    [SerializeField]
    Scenes _nextScene;
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
        Tilte,          // タイトル
        CharacterSelect,// キャラセレクト
        StageSelect,    // ステージセレクト
        Main,           // メインゲーム
        Result          // リザルト
    }

    /// <summary>
    /// ロードオブジ付きの非同期ロード
    /// </summary>
    public void LoadingStartWithOBJ()
    {
        if (!_isLoadStart)
        {
            //ロード画面UIをアクティブにする
            loadObj.SetActive(true);
            //ロードコルーチン開始
            StartCoroutine(LoadData());
            _isLoadStart = true;
        }
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
            //Debug.Log(async.progress);
            slider.value = async.progress;
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("Scene Loaded");

        // ロード完了時進捗スライダーの値をMAXに
        slider.value = 1;

        yield return new WaitForSeconds(0.5f);

        async.allowSceneActivation = true;    // シーン遷移許可
    }
}
