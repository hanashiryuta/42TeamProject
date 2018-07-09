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
    public PlayerIndex playerIndex;//プレイヤーインデックス

    GameObject playerInstance;//プレイヤー実体

    // Update is called once per frame
    void Update ()
    {
        if (playerInstance == null)
        {
            /***
             * プレイヤーのコントローラーを固定することで
             * Unity側のコントローラー番号とプレイヤーインデックスが合わない不具合を防ぐ
             ***/

            playerInstance = GameObject.Find("Player" + ((int)playerIndex + 1));//指定されたインデックスでプレイヤーを格納
            playerInstance.GetComponent<PlayerMove>().playerIndex = playerIndex;//格納されたプレイヤーのコントローラーインデックスを指定

        }
    }
}
