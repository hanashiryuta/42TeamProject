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

    static bool created = false;

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
        if (SceneManager.GetActiveScene().name == "main")
        {
        }
    }
}
