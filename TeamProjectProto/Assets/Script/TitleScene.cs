using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    GameLoad gameload;

    [SerializeField]
    GameObject fadePanel;
    FadeController fadeCon;
    bool isSceneChange = false;
    bool isFaded = false;
    
    // Use this for initialization
    void Start()
    {
        gameload = this.GetComponent<GameLoad>();

        fadeCon = fadePanel.GetComponent<FadeController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown("s"))
        {
            isSceneChange = true;
            Debug.Log("SceneChange");
        }

        if (isSceneChange)
        {
            //SceneManager.LoadScene("main");

            fadeCon.ChangeAlpha();
            Debug.Log("changeAlpha");

            if (fadeCon.image.color.a >= 254 && !isFaded)
            {
                gameload.LoadingStart();
                isFaded = true;
            }
        }


    }
}
