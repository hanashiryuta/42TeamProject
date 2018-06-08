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
    GameObject balloon;
    public float beforeBP;
    bool isRespawn = true;

	// Use this for initialization
	void Start () {
        isRespawn = true;
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
        if (balloon.GetComponent<BalloonOrigin>().blastCount == 0)
            isRespawn = true;

        if (balloon.GetComponent<BalloonOrigin>().blastCount>=balloon.GetComponent<BalloonOrigin>().blastLimit-beforeBP&&isRespawn)
        {
            Instantiate(originExChangeItem,transform.position,Quaternion.identity,transform);
            isRespawn = false;
        }
	}
}
