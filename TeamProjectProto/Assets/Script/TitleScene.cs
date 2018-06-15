using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    GameLoad gameload;

    [SerializeField]
    GameObject fadePanel;
    FadeController fadeController;
    public bool isSceneChange = false;
    bool isFadeOuted = false;
    
    // Use this for initialization
    void Start()
    {
        gameload = this.GetComponent<GameLoad>();

        fadeController = fadePanel.GetComponent<FadeController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeController.IsFadeInFinish == false)
        {
            fadeController.FadeIn();
        }
        else
        {
            if (isSceneChange)
            {
                fadeController.FadeOut();
            }
        }

        if (fadeController.IsFadeOutFinish && !isFadeOuted)
        {
            gameload.LoadingStartWithOBJ();
            isFadeOuted = true;
        }
    }
}
