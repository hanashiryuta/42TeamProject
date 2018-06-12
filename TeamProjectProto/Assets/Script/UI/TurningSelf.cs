/*
 * 作成日：180612
 * 自転
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningSelf : MonoBehaviour
{
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(0, 0.5f, 0);
	}
}
