//
//作成日時：4月16日
//プレイヤー全般の動き
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour {

    float positionX = 0;//プレイヤーのｘ方向移動距離
    float positionZ = 0;//プレイヤーのｚ方向移動距離
    float positionY = 0;//プレイヤーのｙ座標
    public float moveSpeed = 0.5f;//プレイヤーの移動速度    
    public float balloonMoveSpeed = 0.7f;//爆発物を持っている時の移動速度

    bool isJump = false;//ジャンプしているか
    public float originJumpPower = 0.2f;//基本ジャンプの上方向の力
    public float gravPower = 0.1f;//重力の力
    float jumpPower = 0;//今のジャンプの上方向の力
    
    public float balloonJumpPower = 0.3f;//爆発物所持時のジャンプの上方向の力
    [HideInInspector]
    public GameObject balloon;//爆発物

    float blastCount = 0;//内容物所持数
    Text itemCountText;//内容物所持数テキスト
    [HideInInspector]
    public float totalBlastCount = 0;//内容物所持数累計

    [HideInInspector]
    public string horizontal = "Horizontal1";//Inputの左スティック横方向取得名前
    [HideInInspector]
    public string vertical = "Vertical1";//Inputの左スティック縦方向取得名前
    [HideInInspector]
    public string jump = "Jump1";//Inputのジャンプボタン取得名前

    public bool isStan = false;//動けるかどうか
    float stanTime = 1.0f;//動けるようになるまでの時間

    public bool isBalloonShrink = true;//爆発物が縮むかどうか

    bool isGround = true;

    // Use this for initialization
    void Start () {
        //初期化処理
        isJump = false;
        blastCount = 0;
        isGround = true;

        itemCountText = GameObject.Find(transform.name + "ItemCount").GetComponent<Text>();//内容物所持数テキスト取得
	}
	
	// Update is called once per frame
	void Update () {
        //風船を持っていないとき
        if (balloon == null)
        {
            //プレイヤーの移動処理
            positionX = Input.GetAxisRaw(horizontal) * moveSpeed;
            positionZ = Input.GetAxisRaw(vertical) * moveSpeed;
        }
        //風船を持っている時
        else
        { 
            //プレイヤーの移動処理
            positionX = Input.GetAxisRaw(horizontal) * balloonMoveSpeed;
            positionZ = Input.GetAxisRaw(vertical) * balloonMoveSpeed;
        }

        Jump();//ジャンプ  
        
        if(Input.GetKeyDown(KeyCode.A)&&isJump)
        {
            Debug.Log("ヒップドロップ！");
        }
         
        HitField();//あたり判定

        //動けるとき
        if (!isStan)
        {
            Vector3 movePosition = new Vector3(positionX, positionY, positionZ);
            if ((positionX != 0 && positionZ != 0))
            {
                movePosition *= 0.71f;
            }
            if (((positionX != 0 && positionY != 0) || (positionY != 0 && positionZ != 0)) && !isGround)
            {
                movePosition *= 0.71f;
            }


            transform.position += movePosition;//位置更新
        }
        //動けないとき
        else
        {
            stanTime -= Time.deltaTime;
            if(stanTime < 0)
            {
                isStan = false;
                stanTime = 1.0f;
            }
        }
        //床以下にならないようにする
        if (transform.position.y < 1)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            isJump = false;
            isGround = true;
        }

        itemCountText.text = blastCount.ToString();//内容物取得数表示処理 
	}

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void Jump()
    {
        //床にいるならジャンプできる
        if (Input.GetButtonDown(jump)&&!isJump&&isGround)
        {
            //風船を持っているかどうかでジャンプ力が変わる
            positionY = balloon != null ? balloonJumpPower : originJumpPower; ;
            isJump = true;
            isGround = false;
        }
        
        positionY -= gravPower;//重力

        //移動先で当たっているもの
         Collider[] colArray = Physics.OverlapBox(transform.position + new Vector3(0, positionY, 0), new Vector3(transform.localScale.x/2-0.05f,transform.localScale.y/2,transform.localScale.z/2- 0.05f), Quaternion.identity);

        bool isField = false;
        foreach (var cx in colArray)
        {
            //当たっているものが床か特殊壁だったら
            if (cx.tag == "Field" && !cx.transform.name.Contains(transform.name))
            {
                //ジャンプ終える
                positionY = 0;
                isJump = false;
                isField = true;
                isGround = true;
            }
        }
        if(isGround&&!isField)
        {
            isGround = false;
        }

        ////ジャンプ処理
        //if (Input.GetButtonDown(jump) && !isJump)
        //{
        //    jumpPower = balloon != null ? balloonJumpPower : originJumpPower;//爆発物の有無によりジャンプの力が変更
        //    isJump = true;
        //}

        //if (isJump)
        //{
        //    positionY = jumpPower;//現在のPositionに力を足していく
        //    jumpPower -= gravPower;//力を減らしていく            
        //}
        //if (positionY <= 0)
        //{
        //bool isOnField = false;//下にフィールドがあるか
        //foreach (var cx in colArray)
        //{
        //    if (cx.tag == "Field" && !cx.transform.name.Contains(transform.name))
        //    {
        //        isJump = false;
        //        positionY = 0;
        //        jumpPower = 0;
        //        isOnField = true;
        //    }
        //}
        ////床に立っていなければ
        //if (!isOnField)
        //{
        //    isJump = true;//ジャンプ判定
        //}
        //}
    }

    /// <summary>
    /// Rayを使ったあたり判定
    /// </summary>
    void HitField()
    {
        //x方向あたり判定
        if (Input.GetAxisRaw(horizontal) != 0)
        {
            //移動先で当たっているもの
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(positionX, 0, 0), new Vector3(transform.localScale.x / 2, transform.localScale.y / 2 - 0.05f, transform.localScale.z / 2 ), Quaternion.identity))
            {
                //当たっているものが床か特殊壁だったら
                if (cx.tag == "Field" && !cx.transform.name.Contains(transform.name))
                {
                    //移動しない
                    positionX = 0;
                    break;
                }
            }
        }

        //z方向あたり判定
        if (Input.GetAxisRaw(vertical) != 0)
        {
            //移動先で当たっているもの
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(0, 0, positionZ), new Vector3(transform.localScale.x / 2, transform.localScale.y / 2 - 0.05f, transform.localScale.z / 2), Quaternion.identity))
            {
                //当たっているものが床か特殊壁だったら
                if (cx.tag == "Field" && !cx.transform.name.Contains(transform.name))
                {
                    //移動しない
                    positionZ = 0;
                    break;
                }
            }
        }
       
        ////x軸方向のあたり判定
        //if (Input.GetAxisRaw(horizontal) != 0)
        //{
        //    float offset = 0.49f;
        //    Vector3[] offsetList = new Vector3[]
        //    {
        //        new Vector3(0,-offset,-offset),
        //        new Vector3(0,-offset,offset),
        //        new Vector3(0,offset,-offset),
        //        new Vector3(0,offset,offset),
        //    };

        //    //x軸マイナス方向
        //    if (Input.GetAxisRaw(horizontal) < 0)
        //    {
        //        //レイを中心から四角形状に4本出す
        //        for (int i = 0; i < 4; i++)
        //        {
        //            Ray ray = new Ray(transform.position+new Vector3(positionX,0,0) + offsetList[i], new Vector3(-1, 0, 0));
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit, 0.51f))
        //            //if (Physics.BoxCast(transform.position, Vector3.one, new Vector3(-1, 0, 0), out hit, Quaternion.identity, 0.5f))
        //            //if (Physics.OverlapBox(transform.position + new Vector3(positionX, 0, 0), Vector3.one, Quaternion.identity).Length > 0)
        //            {
        //                //レイが当たれば移動しない
        //                if (hit.transform.tag == "Field" && !hit.transform.name.Contains(transform.name))
        //                {
        //                    positionX = 0;
        //                }
        //            }
        //        }
        //    }
        //    //x軸プラス方向
        //    if (Input.GetAxisRaw(horizontal) > 0)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            Ray ray = new Ray(transform.position + new Vector3(positionX, 0, 0) + offsetList[i], new Vector3(1, 0, 0));
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit, 0.51f))
        //            {
        //                //レイが当たれば移動しない
        //                if (hit.transform.tag == "Field" && !hit.transform.name.Contains(transform.name))
        //                    positionX = 0;
        //            }
        //        }
        //    }
        //}

        ////z軸方向あたり判定
        //if (Input.GetAxisRaw(vertical) != 0)
        //{
        //    float offset = 0.49f;
        //    Vector3[] offsetList = new Vector3[]
        //    {
        //        new Vector3(-offset,-offset,0),
        //        new Vector3(offset,-offset,0),
        //        new Vector3(-offset,offset,0),
        //        new Vector3(offset,offset,0),
        //    };

        //    //z軸マイナス方向
        //    if (Input.GetAxisRaw(vertical) < 0)
        //    {
        //        //レイを中心から四角形状に4本出す
        //        for (int i = 0; i < 4; i++)
        //        {
        //            Ray ray = new Ray(transform.position + new Vector3(0, 0, positionZ) + offsetList[i], new Vector3(0, 0, -1));
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit, 0.51f))
        //            {
        //                //レイが当たれば移動しない
        //                if (hit.transform.tag == "Field" && !hit.transform.name.Contains(transform.name))
        //                    positionZ = 0;
        //            }
        //        }
        //    }
        //    //z軸プラス方向
        //    if (Input.GetAxisRaw(vertical) > 0)
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            Ray ray = new Ray(transform.position + new Vector3(0, 0, positionZ) + offsetList[i], new Vector3(0, 0, 1));
        //            RaycastHit hit;

        //            if (Physics.Raycast(ray, out hit, 0.51f))
        //            {
        //                //レイが当たれば移動しない
        //                if (hit.transform.tag == "Field" && !hit.transform.name.Contains(transform.name))
        //                    positionZ = 0;
        //            }
        //        }
        //    }            
        //}

        ////y方向あたり判定
        //if (positionY <= 0)
        //{
        //    bool isOnField = false;//下にフィールドがあるか
        //    //レイをずらすオフセット
        //    float offset = 0.49f;
        //    Vector3[] offsetList = new Vector3[]
        //    {
        //        new Vector3(-offset,0,-offset),
        //        new Vector3(-offset,0,offset),
        //        new Vector3(offset,0,-offset),
        //        new Vector3(offset,0,offset),
        //    };

        //    //レイを中心から四角形状に4本出す
        //    for (int i = 0; i < 4; i++)
        //    {
        //        Ray ray = new Ray(transform.position + new Vector3(0, positionY, 0) + offsetList[i], new Vector3(0, -1, 0));
        //        RaycastHit hit;

        //        //どれか一つでも当たっていたら床に立っている
        //        if (Physics.Raycast(ray, out hit, 0.51f))
        //        {
        //            if (hit.transform.tag == "Field" && !hit.transform.name.Contains(transform.name))
        //            {
        //                isJump = false;
        //                positionY = 0;
        //                jumpPower = 0;
        //                isOnField = true;
        //            }
        //        }
        //    }
        //    //床に立っていなければ
        //    if (!isOnField)
        //        isJump = true;//ジャンプ判定
        //}
    }

    void OnCollisionEnter(Collision col)
    {
        //プレイヤーに当たったら
        if(col.gameObject.tag == "Player")
        {
            if(balloon != null)//爆発物があれば
            {                
                balloon.GetComponent<BalloonController>().BalloonMove(transform.gameObject, col.gameObject);//爆発物の移動処理
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //内容物に当たったら
        if (col.gameObject.name.Contains("PointItem"))
        {
            Destroy(col.gameObject);//内容物破棄
            blastCount += col.GetComponent<ItemController>().point; //内容物所持数を増やす  
            totalBlastCount += col.GetComponent<ItemController>().point;//内容物所持数累計を増やす  
        }
        //強制交換アイテムに当たったら
        if (col.gameObject.name.Contains("ExChangeItem"))
        {
            if (balloon != null)
                balloon = null;
            Destroy(col.gameObject);//強制交換アイテム破棄
            GameObject[] pList = GameObject.FindGameObjectsWithTag("Player");//プレイヤー配列を作成
            GameObject b = GameObject.FindGameObjectWithTag("Balloon");
            b.GetComponent<BalloonController>().BalloonExChange(pList, gameObject);
        }
        //中心物体に当たったら
        if (col.gameObject.tag == "Post")
        {
            if (balloon != null&&isBalloonShrink)
            {
                col.GetComponent<PostController>().blastCount -= blastCount;
                blastCount = 0;//内容物所持数を0にする
                return;
            }
            
            col.GetComponent<PostController>().blastCount += blastCount;//中心物体に内容物総数を渡す
            col.GetComponent<PostController>().respawnCount += blastCount;
            col.GetComponent<PostController>().player = gameObject;
            blastCount = 0;//内容物所持数を0にする
        }
    }
}
