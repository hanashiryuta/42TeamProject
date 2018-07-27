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
    float moveY = 0;//y軸移動方向
    float moveZ = 0;//z軸移動方向
    float moveTime = 0.5f;//吹き飛び時間
    Rigidbody rigid;//リジッドボディ
    //bool onConveyor = false;
    //Vector3 direction;

    public GameObject origin_Item_Death_Particle;//アイテム取得時パーティクル生成元
    GameObject item_Death_Particle;//アイテム取得時パーティクル

    // Use this for initialization
    void Start () {
        rigid = GetComponent<Rigidbody>();//リジッドボディ取得
	}
	
	// Update is called once per frame
	void Update () {
        if (!isGet)
        {
            moveTime -= Time.deltaTime;
            if (moveTime <= 0)
            {

                rigid.velocity = new Vector3(0, 0, 0);
                isGet = true;
            }
        //}
        ////x軸あたり判定
        //foreach (var cx in Physics.OverlapBox(transform.position, new Vector3(transform.localScale.x / 2, transform.localScale.y - 0.01f, transform.localScale.z / 2 - 0.01f), Quaternion.identity))
        //{
        //    //当たっているものが床だったら
        //    if (cx.tag == "Field")
        //    {
        //        //移動しない
        //        rigid.velocity = new Vector3(0,rigid.velocity.y,rigid.velocity.z);
        //        break;
        //    }
        //}
        ////y軸あたり判定
        //foreach (var cx in Physics.OverlapBox(transform.position, new Vector3(transform.localScale.x / 2 - 0.01f, transform.localScale.y, transform.localScale.z / 2 - 0.01f), Quaternion.identity))
        //{
        //    //当たっているものが床だったら
        //    if (cx.tag == "Field")
        //    {
        //        //移動しない
        //        rigid.velocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);
        //        break;
        //    }
        //}
        ////Z軸あたり判定
        //foreach (var cx in Physics.OverlapBox(transform.position, new Vector3(transform.localScale.x / 2 - 0.01f, transform.localScale.y - 0.01f, transform.localScale.z / 2), Quaternion.identity))
        //{
        //    //当たっているものが床だったら
        //    if (cx.tag == "Field")
        //    {
        //        //移動しない
        //        rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, 0);
        //        break;
        //    }
        }

        //if (onConveyor)
        //{
        //    rigid.velocity += direction;
        //}
    }

    /// <summary>
    /// 移動先設定
    /// </summary>
    public void SetMovePosition()
    {
        rigid = GetComponent<Rigidbody>();//リジッドボディ取得
        //360度からランダム
        //float rand = Random.Range(0, 361);
        //moveX = Mathf.Cos(rand);
        //moveZ = Mathf.Sin(rand);
        moveX = (float)Random.Range(-10, 11) / 10;//xランダム設定
        moveZ = (float)Random.Range(-10, 11) / 10;//yランダム設定
        moveY = (float)Random.Range(0, 11) / 10;//Zランダム設定

        rigid.AddForce(500*new Vector3(moveX, moveY, moveZ).normalized);//移動処理
    }

    /// <summary>
    /// アイテム取得時パーティクル生成メソッド
    /// </summary>
    public void Item_Death_Particle()
    {
        if(item_Death_Particle == null)
        {
            item_Death_Particle = Instantiate(origin_Item_Death_Particle, transform.position, Quaternion.identity);
        }
    }

    //void OnCollisionStay(Collision col)
    //{
    //    //Rayを飛ばしてベルトコンベアに当たっていたらベルトコンベアで動くようにする
    //    if (Physics.Linecast(transform.position + Vector3.up, transform.position + Vector3.down, LayerMask.GetMask("BeltConveyor")))
    //    {
    //        var beltConveyor = col.gameObject.GetComponent<BeltConveyor>();
    //        if (beltConveyor != null)
    //        {
    //            direction = beltConveyor.Conveyor();
    //            onConveyor = true;
    //        }
    //        else
    //        {
    //            onConveyor = false;
    //        }
    //    }
    //    else
    //    {
    //        onConveyor = false;
    //    }
    //}
}
