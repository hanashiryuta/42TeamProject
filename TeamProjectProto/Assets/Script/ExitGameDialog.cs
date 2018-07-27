using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Collections;
using XInputDotNetPure;

public class ExitGameDialog : MonoBehaviour {

    static bool created = false;

    void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void OnApplicationQuit()
    {
        for(int i = 0; i < 4; i++)
        {
            GamePad.SetVibration((PlayerIndex)i, 0.0f, 0.0f);
        }
    }

}
