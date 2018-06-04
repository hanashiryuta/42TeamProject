/*
 * 作成日時：180604
 * プレイヤースタンドバイチェック
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; // Required in C#

public class CheckPlayerStandby : MonoBehaviour
{
    [SerializeField]
    PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;
    [SerializeField]
    GameObject playerPrefabs;

    public bool isSpawn = false;

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        currentState = GamePad.GetState(playerIndex);

        if(!isSpawn)
        {
            if (PlayerStandBy())
            {
                GameObject.Instantiate(playerPrefabs, transform.position, Quaternion.Euler(0, 180, 0));
                isSpawn = true;
            }
        }

        previousState = currentState;
    }

    /// <summary>
    /// Aボタンを押したかをチェック
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <returns></returns>
    bool PlayerStandBy()
    {
        if (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed)
        {
            return true;
        }
        return false;
    }
}
