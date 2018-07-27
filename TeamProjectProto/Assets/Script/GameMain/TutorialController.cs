//
//6月28日
//チュートリアル管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

/// <summary>
/// チュートリアルの開始状態
/// </summary>
public enum TutorialStart
{
    Not,//開始していない
    Start,//開始
    End//終了
}


public class TutorialController : MonoBehaviour {

    public bool isTutorial;//チュートリアルをするかどうか
    [HideInInspector]
    public BGMController bgmController;//BGMコントローラ

    [HideInInspector]
    public TutorialStart tutorialStart = TutorialStart.Not;//チュートリアル開始状態

    static bool tutorialCreate = false;//DontDestroy用フラグ

    public GameObject originTutorial;//チュートリアルオブジェクト
    GameObject tutorial;

    // Use this for initialization
    void Awake () {
        //シーン間継続処理
        if (!tutorialCreate)
        {
            DontDestroyOnLoad(this.gameObject);
            tutorialCreate = true;
        }
        else
        {
            //すでにいたらしない
            Destroy(this.gameObject);
        }

        //シーン読み込みコールバック
        SceneManager.sceneLoaded += SceneLoaded;
        //シーン終了コールバック
        SceneManager.sceneUnloaded += SceneUnloaded;
        //チュートリアル表示可能
        isTutorial = true;        
    }
	
	// Update is called once per frame
	void Update () {
        //メインシーンのみチュートリアル処理
      if (SceneManager.GetActiveScene().name == "main")
            Tutorial();
     
    }

    /// <summary>
    /// チュートリアル処理
    /// </summary>
    void Tutorial()
    {
        //チュートリアル開始状態により処理変更
        switch (tutorialStart)
        {
            case TutorialStart.Not://開始状態ではない
                //チュートリアル可能なら
                if (isTutorial)
                    tutorialStart = TutorialStart.Start;//状態遷移
                break;
            case TutorialStart.Start://開始
                //チュートリアルオブジェクトがないなら
                if(tutorial == null)
                {
                    //キャンバス下に生成
                    tutorial = Instantiate(originTutorial, GameObject.FindGameObjectWithTag("Canvas").transform);
                    //フェードなどの下に移動
                    tutorial.transform.SetSiblingIndex(GameObject.FindGameObjectWithTag("Canvas").transform.childCount-2);
                    //自身指定
                    tutorial.GetComponent<Tutorial>().tutorialController = this;
                }
                break;
            case TutorialStart.End://終了
                //チュートリアルオブジェクト破棄
                Destroy(tutorial);
                tutorialStart = TutorialStart.Not;//状態遷移
                break;
        }
    }
    /// <summary>
    /// シーン終了時コールバック
    /// </summary>
    /// <param name="scene"></param>
    private void SceneUnloaded(Scene scene)
    {
        if(scene.name == "main")
        {
            //メインシーン終了時に状態遷移
            tutorialStart = TutorialStart.End;
        }
    }
    /// <summary>
    /// シーン読み込み時コールバック
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            //タイトルを経由すればチュートリアル表示できる
            isTutorial = true;
        }
    }
}
