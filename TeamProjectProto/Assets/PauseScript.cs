using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{

    [SerializeField]

    private GameObject pause;
    public GameObject instancePauseUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("p") || Input.GetButtonDown("Action1"))
        {
            if (instancePauseUI == null)
            {
                instancePauseUI = GameObject.Instantiate(pause) as GameObject;
                Time.timeScale = 0f;
            }
            else
            {
                if (instancePauseUI.active)
                {
                    Destroy(instancePauseUI);
                    Time.timeScale = 1f;
                }
            }
        }

        if (Time.timeScale == 1 && instancePauseUI != null)
        {
            Destroy(instancePauseUI);
        }
    }
}
