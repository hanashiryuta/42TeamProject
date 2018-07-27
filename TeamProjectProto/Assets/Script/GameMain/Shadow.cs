//
//作成日時：5月18日
//影の位置調整
//作成者：平岡誠司
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MonoBehaviour {
    GameObject player;

	// Use this for initialization
	void Start () {
        player = transform.parent.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        //プレイヤーの下方向にRayを飛ばして、その先に当たったオブジェクトに影を置く
        Ray ray = new Ray(player.transform.position, Vector3.down);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction*10, Color.red);

        if(Physics.Raycast(ray,out hit, 10f))
        {
            this.transform.position = hit.point;
        }
	}
}
