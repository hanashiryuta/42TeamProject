﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Air : MonoBehaviour {
    public float power = 10;//移動速度
    GameObject balloon;
    public AudioClip soundSE1;//生成時の効果音
    public float airValue;

    // Use this for initialization
    void Start () {
        balloon = GameObject.FindGameObjectWithTag("Balloon");
        GetComponent<AudioSource>().PlayOneShot(soundSE1);
	}
	
	// Update is called once per frame
	void Update () {
        if (balloon == null)
        {
            balloon = GameObject.FindGameObjectWithTag("Balloon");
            Destroy(gameObject);
            return;
        }

        //風船に向かって飛んでいく
        //Vector3 direction = balloon.transform.position - transform.position;
        //GetComponent<Rigidbody>().AddForce(direction.normalized * power);

        Vector3 pos = (balloon.transform.position - transform.position).normalized; //balloonとの距離を求める
        transform.position += pos * Time.deltaTime * 30; //距離分足していく

       
        if (balloon.GetComponent<BalloonOrigin>().isDestroy)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter(Collider col)
    {
        //風船に当たったら膨らませる
        if (col.gameObject.tag == "Balloon")
        {
            balloon.GetComponent<BalloonOrigin>().BalloonBlast(airValue);
            Destroy(gameObject);
        }
    }
}
