using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject Balloon = GameObject.FindGameObjectWithTag("Balloon");
        GameObject Player = Balloon.GetComponent<BalloonController>().player;
        Vector3 PlayerPosition = Player.transform.Find("Armature/Bone/Bone.001/Bone.002/Bone.003/Bone.004/Bone.004_end/Crown").transform.position;
        LineRenderer Line = GameObject.Find("BalloonLineRenderer(Clone)").GetComponent<LineRenderer>();
        //PlayerPosition.y += 3.0f;
         
        Line.SetPosition(0, Balloon.transform.position);
        //Line.SetPosition(1, new Vector3(Player.transform.position.x, Player.transform.position.y + 1.0f, Player.transform.position.z));
        Line.SetPosition(1, PlayerPosition);
    }
}
