using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour {

    public List<GameObject> PlayerEntry;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (PlayerEntry[0].GetComponent<PlayerEntry>().OnClick && PlayerEntry[1].GetComponent<PlayerEntry>().OnClick && PlayerEntry[2].GetComponent<PlayerEntry>().OnClick && PlayerEntry[3].GetComponent<PlayerEntry>().OnClick) {
            if (Input.GetButtonDown("Action")) {
                SceneManager.LoadScene("main");
            }
        }
	}
}
