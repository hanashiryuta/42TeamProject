/*
 * 作成日時：180529
 * コントローラー番号に合わせてプレイヤー指定
 * 作成者：何承恩
 */
using UnityEngine;
using XInputDotNetPure; // Required in C#

public class XInputConfig : MonoBehaviour
{
    [HideInInspector]
    public PlayerIndex playerIndex;

    GameObject playerInstance;

    // Use this for initialization
    void Start ()
    {
        // No need to initialize anything for the plugin
    }

    // Update is called once per frame
    void Update ()
    {
        if (playerInstance == null)
        {
            playerInstance = GameObject.Find("Player" + ((int)playerIndex + 1));
            playerInstance.GetComponent<PlayerMove>().playerIndex = playerIndex;
        }
    }




}
