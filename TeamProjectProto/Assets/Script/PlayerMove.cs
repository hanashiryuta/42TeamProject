//
//作成日時：4月16日
//プレイヤー全般の動き
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class PlayerMove : MonoBehaviour
{
    public float AxisX = 0;//プレイヤーのｘ移動方向
    public float AxisZ = 0;//プレイヤーのｚ移動方向
    float moveSpeed = 0;//移動速度
    public float originMoveSpeed = 0.5f;//プレイヤーの移動速度    
    public float balloonMoveSpeed = 0.7f;//爆発物を持っている時の移動速度

    bool isJump = false;//ジャンプしているか
    public float originJumpPower = 0.2f;//基本ジャンプの上方向の力
    float gravPower = 0.1f;//重力の力
    float jumpPower = 0;//今のジャンプの上方向の力

    public float balloonJumpPower = 0.3f;//爆発物所持時のジャンプの上方向の力
    [HideInInspector]
    public GameObject balloon;//爆発物

    [HideInInspector]
    public float blastCount = 0;//内容物所持数
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
    float stanTime = 3.0f;//動けるようになるまでの時間

    public bool isBalloonShrink = true;//爆発物が縮むかどうか

    bool isGround = true;//地面にいるかどうか
    bool isHipDrop = false;//ヒップドロップしているかどうか
    float jumpCount = 0;//ジャンプ回数
    public GameObject[] hipDropCircle;//衝撃波範囲→0515何変更　色別の4種
    public GameObject originItem;//アイテム
    public GameObject originHighItem;//ハイアイテム

    [HideInInspector]
    public List<string> itemList;//取得アイテム管理リスト
    List<string> totalItemList;//累計取得アイテム管理リスト

	public AudioClip soundSE1;//ジャンプ時の音
	public AudioClip soundSE2;//アイテム取得時の音
	public AudioClip soundSE3;//ポスト投函時の音

    Rigidbody rigid;//リジットボディ
    float hipDropTime = 0.3f;//ヒップドロップ空中待機時間
    Vector3 hipDropPosition = Vector3.zero;//ヒップドロップ空中待機場所
    
    Vector3 rotationPosition;//プレイヤー回転用ポジション

    //180516 何
    private Animator playerAnim;
    //180530 何
    StartCountDown startCntDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script
    PlayerJumpHit playerJumpHit;

    public bool isMoveInertia = false;

    //衝撃波ヒット時コントローラー振動用リスト
    List<PlayerIndex> XDInput = new List<PlayerIndex>() { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
    public float setStopTime; //Unity側でのコントローラー振動停止までの時間設定用
    private float stopTime; //振動してから止まるまでのタイムラグ
    private bool isStop = false; //振動を止めるかどうか


    // Use this for initialization
    void Start()
    {
        //初期化処理
        isJump = false;
        blastCount = 0;
        isGround = true;
        isHipDrop = false;
        jumpCount = 0;
        rigid = GetComponent<Rigidbody>();
        rotationPosition = transform.position;

        blastCountText = GameObject.Find(transform.name + "ItemCount").GetComponent<Text>();//内容物所持数テキスト取得
        totalBlastCountText = GameObject.Find(transform.name + "TotalCount").GetComponent<Text>(); 
        itemList = new List<string>();
        totalItemList = new List<string>();
        //180516 アニメーター
        playerAnim = transform.GetComponent<Animator>();
        //180530 スタートカウントダウン
        startCntDown = GameObject.Find("StartCountDown").GetComponent<StartCountDown>();
        //180601 終了合図
        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();
        playerJumpHit = GetComponentInChildren<PlayerJumpHit>();

        stopTime = setStopTime * 60; //振動してから止まるまでのタイムラグ
        isStop = false; //振動を止めるかどうか
        
    }

    void Update()
    {
        blastCountText.text = blastCount.ToString();//内容物取得数表示処理 
        totalBlastCountText.text = "Total:" + totalBlastCount.ToString();
        
        PlayerAnim(playerAnim);
        
        if (isStop)
        {
            if (stopTime < 0)
            {
                GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 0.0f);
                stopTime = setStopTime * 60;
                isStop = false;
            }
            else
            {
                stopTime -= 1.0f;
            }
        }

        if(isStan)
        {
            return;
        }

        //移動&ジャンプ入力処理
        HandleXInput();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(jumpCount + "ジャンプカウント");
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //動けないなら
        if (isStan)
        {
            //最初に移動量をゼロに
            if (stanTime >= 3.0f)
            {
                rigid.velocity = Vector3.zero;
                GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                isStop = true;
            }

            rigid.velocity = new Vector3(0, rigid.velocity.y, 0);

            //時間で回復
            stanTime -= Time.deltaTime;
            if (stanTime < 0)
            {
                isStan = false;
                stanTime = 3.0f;
            }
            return;
        }
        //ジャンプ処理
        Jump();

        //スタートカウントダウンOR終了合図中動かせない
        if (!startCntDown.IsCntDown && !finishCall.IsCalling)
        {
            //移動処理
            Move();
        }

        //終了合図中動けない
        if (finishCall.IsCalling)
        {
            rigid.velocity = Vector3.zero;
        }

        Vector3 diff = transform.position + new Vector3(moveJoy.x, 0, moveJoy.y) - transform.position;

        diff.y = 0;

        if (diff.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(diff);
        }       
    }

    /// <summary>
    /// 移動入力処理
    /// </summary>
    void MoveInput()
    {
        //方向指定
        AxisX = Input.GetAxis(horizontal);
        AxisZ = Input.GetAxis(vertical);

        //風船を持っていないとき
        if (balloon == null)
            moveSpeed = originMoveSpeed;
        //風船を持っている時
        else
            moveSpeed = balloonMoveSpeed;
    }

    /// <summary>
    /// ジャンプ入力処理
    /// </summary>
    void JumpInput()
    {
        //ジャンプボタンを押したら
        if (Input.GetButtonDown(jump))
        {
            //ジャンプパワー設定
            jumpPower = balloon != null ? balloonJumpPower * 700 : originJumpPower * 700;

            //地面にいたら
            if (jumpCount == 0)
            {
                rigid.AddForce(new Vector3(0, jumpPower, 0));
                GetComponent<AudioSource>().PlayOneShot(soundSE1);
            }

            //空中にいたら
            else if (jumpCount == 1)
            {
                hipDropTime = 0.3f;
                rigid.velocity = Vector3.zero;
                hipDropPosition = transform.position;
            }

            //ジャンプカウント増加
            jumpCount++;

            //上限設定
            if (jumpCount > 2)
                jumpCount = 2;
        }
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    public void Move()
    {
        //移動vector生成
        Vector3 moveVector = Vector3.zero;

        //あたり判定
        // HitField();

        //移動量設定
        moveVector.x = moveSpeed * moveJoy.x;
        moveVector.z = moveSpeed * moveJoy.y;

            
        //y方向無しの現在のvelocity保存
        Vector3 rigidVelocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);

        if (!isMoveInertia)
        {
            //移動量追加
            rigid.AddForce(100 * (moveVector - rigidVelocity));

            //移動量2.5倍
            rigidVelocity = new Vector3(rigidVelocity.x * 2.5f, rigid.velocity.y, rigidVelocity.z * 2.5f);

            //設定
            rigid.velocity = rigidVelocity;
        }
        else
        {
            //移動（慣性あり）
            rigid.AddForce(moveVector * 15 - rigidVelocity * 2);

            float a = 7;

            if (rigid.velocity.x > a)
            {
                rigid.velocity = new Vector3(a, rigid.velocity.y, rigid.velocity.z);
            }
            if (rigid.velocity.z > a)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, a);
            }
            if (rigid.velocity.x < -a)
            {
                rigid.velocity = new Vector3(-a, rigid.velocity.y, rigid.velocity.z);
            }
            if (rigid.velocity.z < -a)
            {
                rigid.velocity = new Vector3(rigid.velocity.x, rigid.velocity.y, -a);
            }
        }
    }

    /// <summary>
    /// ギズモ描画
    /// </summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(transform.localScale.x*2, transform.localScale.x*2*2, transform.localScale.x*2));
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void Jump()
    {
        //地面にいるとき
        if(jumpCount == 0)
        {
            //重力設定
            gravPower = 9.8f;
        }

        //空中にいるとき
        if(jumpCount == 1)
        {
            //ジャンプ中
            isJump = true;
            //重力設定
            gravPower = 9.8f;
        }

        //ヒップドロップ中
        if(jumpCount == 2)
        {
            //一定時間空中で停止
            hipDropTime -= Time.deltaTime;
            if (hipDropTime > 0)
            {
                //移動量ゼロ
                rigid.velocity = Vector3.zero;
                //位置保存
                transform.position = hipDropPosition;
            }
            //左右移動ゼロ化
            moveJoy.x = 0;
            moveJoy.y = 0;

            //重力設定
            gravPower = 9.8f * 2;
            //ヒップドロップ中
            isHipDrop = true;
        }

        //あたり判定用配列
        /*
        Collider[] colArray = Physics.OverlapBox(
            transform.position + new Vector3(0, -0.01f, 0) + new Vector3(0, 1, 0),
            new Vector3(transform.localScale.x * 2 / 2 - 0.01f,
            transform.localScale.y * 2,
            transform.localScale.z * 2 / 2 - 0.01f),
            transform.localRotation);*/

        //RaycastHit[] colArray = rigid.SweepTestAll(-transform.up, 10.0f);

        //重力追加
        rigid.AddForce(new Vector3(0, -gravPower*5, 0));

        //地面いるか判定
        bool isField = false;

        //foreach (var cx in colArray)
        {
            //Debug.Log(cx.transform.name);
            //当たっているものが床かプレイヤーだったら
            //if ((cx.transform.tag == "Field")||(cx.transform.tag == "Player"&&cx.transform.gameObject != gameObject))
            //あたり判定を別のオブジェクトに任せた
            if(playerJumpHit.isJumpHit)
            {
                //位置を少し浮かす
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
                //地面にいる
                isField = true;
                //ジャンプ終える
                isJump = false;
                //ヒップドロップ中だったら
                if (isHipDrop&&hipDropTime <= 0)
                {
                    //衝撃波生成
                    InstantiateHipDrop();
                }
                isHipDrop = false;
                //地面にいる状態に変更
                if (jumpCount > 0)
                {
                    jumpCount = 0;
                }
            }
        }

        //地面にいる状態　かつ　地面にいない判定だったら（壁から落ちる）
        if (!isField&&jumpCount == 0)
        {
            //ジャンプ状態に変更
            jumpCount = 1;
        }

        ////床以下にならないようにする
        //if (transform.position.y < 1f)
        //{
        //    transform.position = new Vector3(transform.position.x,1f, transform.position.z);
        //    isJump = false;
        //}
    }

    /// <summary>
    /// Rayを使ったあたり判定
    /// </summary>
    void HitField()
    {
        //x方向あたり判定
        if (Input.GetAxis(horizontal) > 0)
        {
            //移動先で当たっているもの
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(0.01f, 0.01f, 0) + new Vector3(0,1,0), new Vector3(transform.localScale.x * 2 / 2, transform.localScale.y * 2, transform.localScale.z * 2 / 2), transform.localRotation))
            {
                //当たっているものが床か特殊壁だったら
                if (cx.tag == "Field")
                {
                    //移動しない
                    //AxisX = 0;
                    moveJoy.x = 0;
                    break;
                }
            }
        }

        //z方向あたり判定
        if (Input.GetAxis(vertical) > 0)
        {
            //移動先で当たっているもの
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(0, 0.01f, 0.01f) + new Vector3(0, 1, 0), new Vector3(transform.localScale.x * 2 / 2, transform.localScale.y * 2, transform.localScale.z * 2 / 2), transform.localRotation))
            {
                //当たっているものが床か特殊壁だったら
                if (cx.tag == "Field")
                {
                    //移動しない
                    //AxisZ = 0;
                    moveJoy.y = 0;
                    break;
                }
            }
        }

        //x方向あたり判定
        if (Input.GetAxis(horizontal) < 0)
        {
            //移動先で当たっているもの
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(-0.01f, 0.01f, 0) + new Vector3(0, 1, 0), new Vector3(transform.localScale.x * 2 / 2, transform.localScale.y * 2, transform.localScale.z * 2 / 2), transform.localRotation))
            {
                //当たっているものが床か特殊壁だったら
                if (cx.tag == "Field")
                {
                    //移動しない
                    //AxisX = 0;
                    moveJoy.x = 0;
                    break;
                }
            }
        }

        //z方向あたり判定
        if (Input.GetAxis(vertical) < 0)
        {
            //移動先で当たっているもの
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(0, 0.01f, -0.01f) + new Vector3(0, 1, 0), new Vector3(transform.localScale.x * 2 / 2, transform.localScale.y * 2, transform.localScale.z * 2 / 2), transform.localRotation))
            {
                //当たっているものが床か特殊壁だったら
                if (cx.tag == "Field")
                {
                    //移動しない
                    //AxisZ = 0;
                    moveJoy.y = 0;
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
                GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                isStop = true;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
    //    //内容物に当たったら
    //    if (col.gameObject.name.Contains("PointItem"))
    //    {
    //        if (col.gameObject.GetComponent<ItemController>().isGet&&balloon == null)
    //        {
    //            itemList.Add(col.name);//リスト追加
    //            Destroy(col.gameObject);//内容物破棄
    //            blastCount += col.GetComponent<ItemController>().point; //内容物所持数を増やす  
				//GetComponent<AudioSource> ().PlayOneShot (soundSE2);
    //            //totalBlastCount += col.GetComponent<ItemController>().point;//内容物所持数累計を増やす
    //        }
    //    }
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
        if (col.gameObject.tag == "Post"&&balloon == null)
        {
            totalBlastCount += blastCount;

            if (balloon != null && isBalloonShrink)
            {
                col.GetComponent<PostController>().blastCount -= blastCount;
                itemList.Clear();
                blastCount = 0;//内容物所持数を0にする
                return;
            }

            col.GetComponent<PostController>().blastCount += blastCount;//中心物体に内容物総数を渡す
            col.GetComponent<PostController>().respawnCount += blastCount;
            col.GetComponent<PostController>().player = gameObject;
            foreach(var cx in itemList)
            {
                totalItemList.Add(cx);
            }
            itemList.Clear();
            blastCount = 0;//内容物所持数を0にする
			GetComponent<AudioSource> ().PlayOneShot (soundSE3);
        }

        //衝撃波に当たったら
        if (col.gameObject.tag == "HipDropCircle")
        {
            //衝撃波ヒット時振動
            if (!col.transform.name.Contains(transform.name)&&!isStan)
            {
                //GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                //isStop = true;
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
                GameObject spawnItem = Instantiate(item, transform.position+new Vector3(0,item.transform.localScale.y+3,0), Quaternion.Euler(90,0,0));//生成
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
                GameObject hipDrop1 = Instantiate(hipDropCircle[0], transform.position+new Vector3(0,0.2f,0), Quaternion.identity);
                hipDrop1.name = hipDrop1.name + transform.name;
                break;
            case "Player2":
                GameObject hipDrop2 = Instantiate(hipDropCircle[1], transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
                hipDrop2.name = hipDrop2.name + transform.name;
                break;
            case "Player3":
                GameObject hipDrop3 = Instantiate(hipDropCircle[2], transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
                hipDrop3.name = hipDrop3.name + transform.name;
                break;
            case "Player4":
                GameObject hipDrop4 = Instantiate(hipDropCircle[3], transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
                hipDrop4.name = hipDrop4.name + transform.name;
                break;
        }
    }

    /// <summary>
    /// 追加日：180516 追加者：何
    /// プレイヤーアニメーション
    /// </summary>
    /// <param name="anim"></param>
    private void PlayerAnim(Animator anim)
    {
        anim.SetBool("isJump", isJump);//`ジャンプ
        anim.SetBool("isHipDrop", isHipDrop);//ヒップドロップ
        anim.SetFloat("velocity", Mathf.Abs(rigid.velocity.x) <= 0.001f && Mathf.Abs(rigid.velocity.z) <= 0.001f ? 0 : 1);//移動
        anim.SetBool("isStan", isStan);//スタン
    }

    [HideInInspector]
    public PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;

    Vector2 moveJoy;

    /// <summary>
    /// 追加日：180529 追加者：何
    /// XBoxコントローラー入力
    /// </summary>
    void HandleXInput()
    {
        currentState = GamePad.GetState(playerIndex);

        if (!currentState.IsConnected)
        {
            return;
        }

        moveJoy.x = currentState.ThumbSticks.Left.X;
        moveJoy.y = currentState.ThumbSticks.Left.Y;

        //風船を持っていないとき
        if (balloon == null)
            moveSpeed = originMoveSpeed;
        //風船を持っている時
        else
            moveSpeed = balloonMoveSpeed;


        JumpXInput();

        previousState = currentState;
    }

    /// <summary>
    /// 追加日：180529 追加者：何
    /// Jumpボタン入力
    /// </summary>
    /// <returns>Aボタン一回押されたか</returns>
    void JumpXInput()
    {
        //ジャンプボタンを押したら
        if (previousState.Buttons.A == ButtonState.Released && currentState.Buttons.A == ButtonState.Pressed)
        {
            //ジャンプパワー設定
            jumpPower = balloon != null ? balloonJumpPower * 700 : originJumpPower * 700;

            //地面にいたら
            if (jumpCount == 0)
            {
                rigid.AddForce(new Vector3(0, jumpPower, 0));
                GetComponent<AudioSource>().PlayOneShot(soundSE1);
            }
            //空中にいたら
            else if (jumpCount == 1)
            {
                hipDropTime = 0.3f;
                rigid.velocity = Vector3.zero;
                hipDropPosition = transform.position;
            }

            //ジャンプカウント増加
            jumpCount++;

            //上限設定
            if (jumpCount > 2)
                jumpCount = 2;
        }
    }
}
