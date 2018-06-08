using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntry : MonoBehaviour {

    public string OkButtonName;
    public string Cancel;
    public bool OnClick;

	// Use this for initialization
	void Start () {
        OnClick = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown(OkButtonName)) {
            OnClick = true;
        }

        if (OnClick)
        {
            if (Input.GetButtonDown(Cancel)) {
                OnClick = false;
            }
        }
	}
}
