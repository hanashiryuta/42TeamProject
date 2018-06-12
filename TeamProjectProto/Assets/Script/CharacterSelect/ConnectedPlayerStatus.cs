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
    Dictionary<string, int> _connectedPlayer = new Dictionary< string, int>();
    public Dictionary<string, int> ConnectedPlayer
    {
        get { return _connectedPlayer; }
        set { _connectedPlayer = value; }
    }

    string _stageName;
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
        if (!created)
        {
            //SelectシーンからMainシーン
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
}
