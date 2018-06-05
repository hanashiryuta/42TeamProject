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
    public bool isSceneChange = false;
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
        if (isSceneChange)
        {
            //SceneManager.LoadScene("main");

            fadeCon.ChangeAlpha();
            Debug.Log("changeAlpha");

            if (fadeCon.IsFadeFinish && !isFaded)
            {
                gameload.LoadingStart();
                isFaded = true;
            }
        }
    }
}
