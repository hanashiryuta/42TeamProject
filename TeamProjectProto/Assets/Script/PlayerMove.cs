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

    List<GameObject> onObject;//今当たっているオブジェクト
    

    // Use this for initialization
    void Start () {
        //初期化処理
        isJump = false;
        blastCount = 0;
        onObject = new List<GameObject>();

        itemCountText = GameObject.Find(transform.name + "ItemCount").GetComponent<Text>();//内容物所持数テキスト取得
	}
	
	// Update is called once per frame
	void Update () { 
        //プレイヤーの移動処理
        positionX =   Input.GetAxisRaw(horizontal) * moveSpeed;
        positionZ =   Input.GetAxisRaw(vertical) * moveSpeed;

        Jump();//ジャンプ  
         
        HitField();//あたり判定
      
        transform.position = new Vector3(transform.position.x+positionX, transform.position.y+positionY, transform.position.z + positionZ);//位置更新
        if (transform.position.y < 1)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);//床以下にならないようにする
            jumpPower = 0;
            isJump = false;
        }

        itemCountText.text = blastCount.ToString();//内容物取得数表示処理 
	}

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void Jump()
    { 
        //ジャンプ処理
        if (Input.GetButtonDown(jump) && !isJump)
        {
            jumpPower = balloon != null ? balloonJumpPower : originJumpPower;//爆発物の有無によりジャンプの力が変更
            isJump = true;
        }

        if (isJump)
        {
            positionY = jumpPower;//現在のPositionに力を足していく
            jumpPower -= gravPower;//力を減らしていく            
        }
        
    }
    
    /// <summary>
    /// Rayを使ったあたり判定
    /// </summary>
    void HitField()
    {

        //x軸方向のあたり判定
        if (Input.GetAxisRaw(horizontal) != 0)
        {
            float offset = 0.49f;
            Vector3[] offsetList = new Vector3[]
            {
                new Vector3(0,-offset,-offset),
                new Vector3(0,-offset,offset),
                new Vector3(0,offset,-offset),
                new Vector3(0,offset,offset),
            };

            //x軸マイナス方向
            if (Input.GetAxisRaw(horizontal) < 0)
            {
                //レイを中心から四角形状に4本出す
                for (int i = 0; i < 4; i++)
                {
                    Ray ray = new Ray(transform.position + offsetList[i], new Vector3(-1, 0, 0));
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 0.51f))
                    {
                        //レイが当たれば移動しない
                        if (hit.transform.tag == "Field")
                            positionX = 0;
                    }
                }
            }
            //x軸プラス方向
            if (Input.GetAxisRaw(horizontal) > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Ray ray = new Ray(transform.position + offsetList[i], new Vector3(1, 0, 0));
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 0.51f))
                    {
                        //レイが当たれば移動しない
                        if (hit.transform.tag == "Field")
                            positionX = 0;
                    }
                }
            }
        }

        //z軸方向あたり判定
        if (Input.GetAxisRaw(vertical) != 0)
        {
            float offset = 0.49f;
            Vector3[] offsetList = new Vector3[]
            {
                new Vector3(-offset,-offset,0),
                new Vector3(offset,-offset,0),
                new Vector3(-offset,offset,0),
                new Vector3(offset,offset,0),
            };

            //z軸マイナス方向
            if (Input.GetAxisRaw(vertical) < 0)
            {
                //レイを中心から四角形状に4本出す
                for (int i = 0; i < 4; i++)
                {
                    Ray ray = new Ray(transform.position + offsetList[i], new Vector3(0, 0, -1));
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 0.51f))
                    {
                        //レイが当たれば移動しない
                        if (hit.transform.tag == "Field")
                            positionZ = 0;
                    }
                }
            }
            //z軸プラス方向
            if (Input.GetAxisRaw(vertical) > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    Ray ray = new Ray(transform.position + offsetList[i], new Vector3(0, 0, 1));
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 0.51f))
                    {
                        //レイが当たれば移動しない
                        if (hit.transform.tag == "Field")
                            positionZ = 0;
                    }
                }
            }            
        }

        //y方向あたり判定
        if (positionY <= 0)
        {
            bool isOnField = false;//下にフィールドがあるか
            //レイをずらすオフセット
            float offset = 0.49f;
            Vector3[] offsetList = new Vector3[]
            {
                new Vector3(-offset,0,-offset),
                new Vector3(-offset,0,offset),
                new Vector3(offset,0,-offset),
                new Vector3(offset,0,offset),
            };

            //レイを中心から四角形状に4本出す
            for (int i = 0; i < 4; i++)
            {
                Ray ray = new Ray(transform.position + offsetList[i], new Vector3(0, -1, 0));
                RaycastHit hit;

                //どれか一つでも当たっていたら床に立っている
                if (Physics.Raycast(ray, out hit, 0.51f))
                {
                    if (hit.transform.tag == "Field")
                    {
                        isJump = false;
                        positionY = 0;
                        isOnField = true;
                    }
                }
            }
            //床に立っていなければ
            if (!isOnField)
                isJump = true;//ジャンプ判定
        }
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
            col.GetComponent<PostController>().blastCount += blastCount;//中心物体に内容物総数を渡す
            blastCount = 0;//内容物所持数を0にする
        }
    }
}
