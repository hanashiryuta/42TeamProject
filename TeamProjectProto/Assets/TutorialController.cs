//
//6月28日
//チュートリアル管理クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;
public enum state
{
    Not,
    Start,
    End
}


public class TutorialController : MonoBehaviour {

    public bool isTutorial;
    [HideInInspector]
    public BGMController bgmController;

    [HideInInspector]
    public state state = state.Not;

    static bool tutorialCreate = false;

    public GameObject originTutorial;
    GameObject tutorial;

    // Use this for initialization
    void Awake () {
        if (!tutorialCreate)
        {
            DontDestroyOnLoad(this.gameObject);
            tutorialCreate = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

        
        SceneManager.sceneLoaded += SceneLoaded;
        SceneManager.sceneUnloaded += SceneUnloaded;
        isTutorial = true;        
    }
	
	// Update is called once per frame
	void Update () {

      if (SceneManager.GetActiveScene().name == "main")
            Tutorial();
     
    }

    void Tutorial()
    {
        switch (state)
        {
            case state.Not:
                if (isTutorial&&SceneManager.GetActiveScene().name == "main")
                    state = state.Start;
                break;
            case state.Start:
                if(tutorial == null)
                {
                    tutorial = Instantiate(originTutorial, GameObject.FindGameObjectWithTag("Canvas").transform);
                    tutorial.transform.SetSiblingIndex(GameObject.FindGameObjectWithTag("Canvas").transform.childCount-2);
                    tutorial.GetComponent<Tutorial>().tutorialController = this;
                }
                break;
            case state.End:
                Destroy(tutorial);
                state = state.Not;
                break;
        }
    }
    private void SceneUnloaded(Scene scene)
    {
        if(scene.name == "main")
        {
            state = state.End;
        }
    }
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Title")
        {
            isTutorial = true;
        }
    }
}
