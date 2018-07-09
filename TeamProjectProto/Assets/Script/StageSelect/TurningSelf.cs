/*
 * 作成日：180612
 * OBJ自転（ステージセレクト）
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurningSelf : MonoBehaviour
{
    public Vector3 rotate;//回転角度
	
	// Update is called once per frame
	void Update ()
    {
        //回転
        transform.Rotate(rotate.x, rotate.y, rotate.z);
	}
}
