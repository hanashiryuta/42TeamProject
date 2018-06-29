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
    Dictionary<string, int> _connectedPlayer = new Dictionary< string, int>();//繋がているプレイヤー
    public Dictionary<string, int> ConnectedPlayer
    {
        get { return _connectedPlayer; }
        set { _connectedPlayer = value; }
    }

    string _stageName;//ステージの名前
    public string StageName
    {
        get { return _stageName; }
        set { _stageName = value; }
    }

    static bool created = false;
    public bool Created
    {
        get { return created; }
        set { created = value; }
    }

    void Awake()
    {
        //シーン切替を検知
        SceneManager.sceneLoaded += SceneLoaded;

        //1つしか存在しない
        if (!created)
        {
            //SelectシーンからMain.Resultシーン
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
    }

    /// <summary>
    /// シーンが呼ばれた時呼ばれるメソッド
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + "Loaded");
        //キャラセレクトシーンをロードするとプ接続プレイヤーを削除
        if(scene.name == "CharacterSelect")
            _connectedPlayer.Clear();
    }
}
