using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air : MonoBehaviour {
    public float power = 10;//移動速度
    GameObject balloon;

    // Use this for initialization
    void Start () {
        balloon = GameObject.FindGameObjectWithTag("Balloon");
	}
	
	// Update is called once per frame
	void Update () {
        //風船に向かって飛んでいく
        Vector3 direction = balloon.transform.position - transform.position;
        //GetComponent<Rigidbody>().AddForce(direction.normalized * power);

        //直線で飛んでいく
        GetComponent<Rigidbody>().MovePosition(transform.position + direction.normalized * 0.5f);

        if (balloon.GetComponent<BalloonController>().isDestroy)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter(Collider col)
    {
        //風船に当たったら膨らませる
        if (col.gameObject.tag == "Balloon")
        {
            balloon.GetComponent<BalloonController>().BalloonBlast();
            Destroy(gameObject);
        }
    }
}
