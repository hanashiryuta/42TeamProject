///
///作成日時：4月20日
///強制交換アイテム生成クラス
///作成者：葉梨竜太
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExChangeRespawn : MonoBehaviour {

    public GameObject exChangeItem;
    GameObject balloon;
    public float beforeBP;
    bool isSpawn = true;

	// Use this for initialization
	void Start () {
        isSpawn = true;
        balloon = GameObject.FindGameObjectWithTag("Balloon");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (balloon == null)
        {
            balloon = GameObject.FindGameObjectWithTag("Balloon");
            return;
        }
        if (isSpawn&&balloon.GetComponent<BalloonController>().blastCount>=balloon.GetComponent<BalloonController>().blastLimit-beforeBP)
        {
            Instantiate(exChangeItem,transform.position,Quaternion.identity,transform);
            isSpawn = false;
        }
	}
}
