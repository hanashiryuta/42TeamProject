//
//作成日時：4月16日
//プレイヤー全般の動き
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{

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
    Text blastCountText;//内容物所持数テキスト
    Text totalBlastCountText;//内容物所持数累計テキスト
    [HideInInspector]
    public float totalBlastCount = 0;//内容物所持数累計

    [HideInInspector]
    public string horizontal = "Horizontal1";//Inputの左スティック横方向取得名前
    [HideInInspector]
    public string vertical = "Vertical1";//Inputの左スティック縦方向取得名前
    [HideInInspector]
    public string jump = "Jump1";//Inputのジャンプボタン取得名前

    public bool isStan = false;//動けるかどうか
    float stanTime = 2.0f;//動けるようになるまでの時間

    public bool isBalloonShrink = true;//爆発物が縮むかどうか

    bool isGround = true;//地面にいるかどうか
    bool isHipDrop = false;//ヒップドロップしているかどうか
    float jumpCount = 0;//ジャンプ回数
    public GameObject[] hipDropCircle;//衝撃波範囲→0515何変更　色別の4種
    public GameObject originItem;//アイテム
    public GameObject originHighItem;//ハイアイテム

    List<string> itemList;//取得アイテム管理リスト

	public AudioClip soundSE1;//ジャンプ時の音
	public AudioClip soundSE2;//アイテム取得時の音
	public AudioClip soundSE3;//ポスト投函時の音

    // Use this for initialization
    void Start()
    {
        //初期化処理
        isJump = false;
        blastCount = 0;
        isGround = true;
        isHipDrop = false;
        jumpCount = 0;

        blastCountText = GameObject.Find(transform.name + "ItemCount").GetComponent<Text>();//内容物所持数テキスト取得
        totalBlastCountText = GameObject.Find(transform.name + "TotalCount").GetComponent<Text>(); 
        itemList = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
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

        HitField();//あたり判定

        //動けるとき
        if (!isStan)
        {
            Vector3 movePosition = new Vector3(positionX, positionY, positionZ);

            //各移動軸の正規化処理
            //if ((positionX != 0 && positionZ != 0))
            //{
            //    movePosition *= 0.71f;
            //}
            //if (((positionX != 0 && positionY != 0) || (positionY != 0 && positionZ != 0)) && !isGround)
            //{
            //    movePosition *= 0.71f;
            //}


            transform.position += movePosition;//位置更新
        }
        //動けないとき
        else
        {
            stanTime -= Time.deltaTime;
            if (stanTime < 0)
            {
                isStan = false;
                stanTime = 2.0f;
            }
        }
        //床以下にならないようにする
        if (transform.position.y < 1)
        {
            transform.position = new Vector3(transform.position.x, 1, transform.position.z);
            isJump = false;
            isGround = true;
        }

        blastCountText.text = blastCount.ToString();//内容物取得数表示処理 
        totalBlastCountText.text = "Total:"+totalBlastCount.ToString();
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void Jump()
    {
        //床に着くまでジャンプした回数
        if (Input.GetButtonDown(jump))
        {
            jumpCount++;
        }
        //床でジャンプしたなら普通のジャンプできる
        if (jumpCount == 1 && !isJump && isGround)
        {
            //風船を持っているかどうかでジャンプ力が変わる
            positionY = balloon != null ? balloonJumpPower : originJumpPower; ;
            isJump = true;
            isGround = false;
			GetComponent<AudioSource> ().PlayOneShot (soundSE1);
        }
        //空中でジャンプしたならヒップドロップ
        if (jumpCount == 2 && isJump && !isHipDrop)
        {
            isHipDrop = true;
            positionY = balloon != null ? balloonJumpPower : originJumpPower;
        }

        //ヒップドロップ中は移動しないようにする
        if (isHipDrop)
        {
            positionX = 0;
            positionZ = 0;
        }

        if (!isHipDrop)
            positionY -= gravPower;//重力
        else
            positionY -= gravPower * 2.5f;//ヒップドロップ中は重力2.5倍

        //移動先で当たっているもの
        Collider[] colArray = Physics.OverlapBox(transform.position + new Vector3(0, positionY, 0), new Vector3(transform.localScale.x / 2 - 0.05f, transform.localScale.y / 2, transform.localScale.z / 2 - 0.05f), Quaternion.identity);

        bool isField = false;
        foreach (var cx in colArray)
        {
            //当たっているものが床か特殊壁だったら
            if ((cx.tag == "Field" && !cx.transform.name.Contains(transform.name))||(cx.tag == "Player"&&cx != gameObject.GetComponent<BoxCollider>()))
            {
                //ジャンプ終える
                isJump = false;
                isField = true;
                isGround = true;
                if (isHipDrop)
                {
                    //衝撃波生成
                    InstantiateHipDrop();
                    isHipDrop = false;
                }
                positionY = 0;
                jumpCount = 0;
            }
        }
        if (isGround && !isField)
        {
            isGround = false;
        }
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
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(positionX, 0, 0), new Vector3(transform.localScale.x / 2, transform.localScale.y / 2 - 0.05f, transform.localScale.z / 2), Quaternion.identity))
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
    }

    void OnCollisionEnter(Collision col)
    {
        //プレイヤーに当たったら
        if (col.gameObject.tag == "Player")
        {
            if (balloon != null)//爆発物があれば
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
            if (col.gameObject.GetComponent<ItemController>().isGet)
            {
                itemList.Add(col.name);//リスト追加
                Destroy(col.gameObject);//内容物破棄
                blastCount += col.GetComponent<ItemController>().point; //内容物所持数を増やす  
				GetComponent<AudioSource> ().PlayOneShot (soundSE2);
                //totalBlastCount += col.GetComponent<ItemController>().point;//内容物所持数累計を増やす
            }
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
            totalBlastCount += blastCount;

            if (balloon != null && isBalloonShrink)
            {
                col.GetComponent<PostController>().blastCount -= blastCount;
                blastCount = 0;//内容物所持数を0にする
                return;
            }

            col.GetComponent<PostController>().blastCount += blastCount;//中心物体に内容物総数を渡す
            col.GetComponent<PostController>().respawnCount += blastCount;
            col.GetComponent<PostController>().player = gameObject;
			blastCount = 0;//内容物所持数を0にする
			GetComponent<AudioSource> ().PlayOneShot (soundSE3);
        }
        //衝撃波に当たったら
        if (col.gameObject.tag == "HipDropCircle")
        {
            if (!col.transform.name.Contains(transform.name))
            {
                isStan = true;
                ItemBlast(1);
            }
        }
    }

    /// <summary>
    /// アイテム排出処理
    /// </summary>
    /// <param name="count">割合</param>
    public void ItemBlast(float count)
    {
        if (itemList.Count > 0)
        {
            int j = (int)(itemList.Count * (count / 10));//指定した割合で排出
            for (int i = 0; i < j; i++)
            {
                GameObject item = originItem;
                int itemNum = Random.Range(0, itemList.Count);

                switch (itemList[itemNum])//取得したアイテムからランダムで選出
                {
                    case "PointItem(Clone)"://普通のアイテム
                        blastCount--;
                        break;
                    case "HighPointItem(Clone)"://高ポイントアイテム
                        blastCount -= 2;
                        item = originHighItem;
                        break;
                }
                itemList.RemoveAt(itemNum);//リストから削除
                GameObject spawnItem = Instantiate(item, transform.position, Quaternion.identity);//生成
                spawnItem.GetComponent<ItemController>().SetMovePosition();
                spawnItem.GetComponent<ItemController>().isGet = false;
            }
        }
    }

    /// <summary>
    /// 180515　何
    /// 衝撃波生成
    /// </summary>
    private void InstantiateHipDrop()
    {
        //プレイヤー別に衝撃波の色が違う
        switch (transform.name)
        {
            case "Player1":
                GameObject hipDrop1 = Instantiate(hipDropCircle[0], transform.position + new Vector3(0, positionY, 0), Quaternion.identity);
                hipDrop1.name = hipDrop1.name + transform.name;
                break;
            case "Player2":
                GameObject hipDrop2 = Instantiate(hipDropCircle[1], transform.position + new Vector3(0, positionY, 0), Quaternion.identity);
                hipDrop2.name = hipDrop2.name + transform.name;
                break;
            case "Player3":
                GameObject hipDrop3 = Instantiate(hipDropCircle[2], transform.position + new Vector3(0, positionY, 0), Quaternion.identity);
                hipDrop3.name = hipDrop3.name + transform.name;
                break;
            case "Player4":
                GameObject hipDrop4 = Instantiate(hipDropCircle[3], transform.position + new Vector3(0, positionY, 0), Quaternion.identity);
                hipDrop4.name = hipDrop4.name + transform.name;
                break;
        }
    }
}

