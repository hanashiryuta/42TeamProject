/*
 * 作成日時：180611
 * 接続したプレイヤーのステータスを保管するクラス
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectedPlayerStatus : MonoBehaviour
{
    [SerializeField]
    Dictionary<string, int> _connectedPlayerDic = new Dictionary< string, int>();//繋がているプレイヤー名ディクショナリ
    public Dictionary<string, int> ConnectedPlayer
    {
        get { return _connectedPlayerDic; }
        set { _connectedPlayerDic = value; }
    }

    string _stageName;//ステージの名前
    public string StageName
    {
        get { return _stageName; }
        set { _stageName = value; }
    }

    static bool isCreated = false;//生成したか？
    public bool Created
    {
        get { return isCreated; }
        set { isCreated = value; }
    }

    List<bool> _isAIList = new List<bool>();//AIか？
    public List<bool> IsAIList
    {
        get { return _isAIList; }
        set { _isAIList = value; }
    }

    void Awake()
    {
        //シーン切替を検知
        SceneManager.sceneLoaded += SceneLoaded;

        //1つしか存在しない
        if (!isCreated)
        {
            //SelectシーンからMain.Resultシーン
            DontDestroyOnLoad(this.gameObject);
            isCreated = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// シーンが呼ばれた時呼ばれるメソッド
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + "Loaded");
        //キャラセレクトシーンをロードすると再格納するため
        if(scene.name == "CharacterSelect")
        {
            //現在接続プレイヤーディクショナリをクリア
            _connectedPlayerDic.Clear();
        }
    }
}
