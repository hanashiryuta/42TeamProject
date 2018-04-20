///
///作成日時：4月20日
///強制交換アイテム生成クラス
///作成者：葉梨竜太
///
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExChangeRespawn : MonoBehaviour {

    public GameObject originExChangeItem;
    GameObject exChangeItem = null;
    GameObject balloon;
    public float beforeBP;

	// Use this for initialization
	void Start () {
        exChangeItem = null;
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
        if (balloon.GetComponent<BalloonController>().blastCount>=balloon.GetComponent<BalloonController>().blastLimit-beforeBP&&exChangeItem == null)
        {
            exChangeItem = Instantiate(originExChangeItem,transform.position,Quaternion.identity,transform);
        }
        else if (balloon.GetComponent<BalloonController>().blastCount < balloon.GetComponent<BalloonController>().blastLimit - beforeBP && exChangeItem != null)
        {
            Destroy(exChangeItem);
        }
	}
}
