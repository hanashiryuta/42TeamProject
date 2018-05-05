//
//作成日時：4月18日
//内容物の処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour {

    public float point = 1;//内容物の数
    [HideInInspector]
    public bool isGet = true;//獲得できるかどうか
    float positionX = 0;//x軸移動量
    float positionY = 0;//y軸移動量
    float positionZ = 0;//z軸移動量
    bool isGround = true;//地面にいるか
    float moveX = 0;//x軸移動方向
    float moveZ = 0;//z軸移動方向
    float moveTime = 0.5f;//吹き飛び時間

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!isGet)
        {
            positionX = moveX / 25;
            positionZ = moveZ / 25;

            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(positionX, 0, 0), new Vector3(transform.localScale.x / 2, transform.localScale.y / 2 - 0.05f, transform.localScale.z / 2), Quaternion.identity))
            {
                //当たっているものが床だったら
                if (cx.tag == "Field")
                {
                    //移動しない
                    positionX = 0;
                    break;
                }
            }

            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(0, 0, positionZ), new Vector3(transform.localScale.x / 2, transform.localScale.y / 2 - 0.05f, transform.localScale.z / 2), Quaternion.identity))
            {
                //当たっているものが床だったら
                if (cx.tag == "Field")
                {
                    //移動しない
                    positionZ = 0;
                    break;
                }
            }

            transform.position += new Vector3(positionX, 0, positionZ);

            moveTime -= Time.deltaTime;
            if (moveTime <= 0)
            {
                isGet = true;
            }
        }
	}

    /// <summary>
    /// 移動先設定
    /// </summary>
    public void SetMovePosition()
    {
        //360度からランダム
        float rand = Random.Range(0, 361);
        moveX = 2 * Mathf.Cos(rand);
        moveZ = 2 * Mathf.Sin(rand);
    }
}
