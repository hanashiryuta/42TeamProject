using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostMarker : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //Xスケール、Zスケールを小さくして0以下になったらDestroyする
        gameObject.transform.localScale -= new Vector3(0.025f, 0, 0.025f);
        if(gameObject.transform.localScale.x<=0 && gameObject.transform.localScale.z <= 0)
        {
            Destroy(gameObject);
        }
	}
}
