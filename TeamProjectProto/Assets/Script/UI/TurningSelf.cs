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
    public Vector3 rotate;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(rotate.x, rotate.y, rotate.z);
	}
}
