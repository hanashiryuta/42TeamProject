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
    public float holdItemCount = 0;//内容物所持数
    Text holdItemCountText;//内容物所持数テキスト
    public Text HoldItemCountText
    {
        get { return holdItemCountText; }
        set { holdItemCountText = value; }
    }
    Text totalItemCountText;//内容物所持数累計テキスト
    [HideInInspector]
    public float totalItemCount = 0;//内容物所持数累計

    [HideInInspector]
    public string horizontal = "Horizontal1";//Inputの左スティック横方向取得名前
    [HideInInspector]
    public string vertical = "Vertical1";//Inputの左スティック縦方向取得名前
    [HideInInspector]
    public string jump = "Jump1";//Inputのジャンプボタン取得名前

    public bool isStan = false;//動けるかどうか
    public float originStanTime = 1.5f;//動けるようになるまでの時間
    [HideInInspector]
    public float stanTime;

    public bool isBalloonShrink = true;//爆発物が縮むかどうか

    bool isHipDrop = false;//ヒップドロップしているかどうか
    [HideInInspector]
    public float jumpCount = 0;//ジャンプ回数
    public GameObject[] hipDropCircle;//衝撃波範囲→0515何変更　色別の4種
    public GameObject originItem;//アイテム
    public GameObject originHighItem;//ハイアイテム

    [HideInInspector]
    public List<string> itemList;//取得アイテム管理リスト
    [HideInInspector]
    public List<string> totalItemList;//累計取得アイテム管理リスト

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

    public GameObject origin_Stan_Star_Particle;//星型パーティクル生成元
    GameObject stan_Star_Particle;//星型パーティクル

    public GameObject effect;//エフェクト

    // ダッシュ関連 0606 何
    bool _isDash = false;//ダッシュ中か
    public bool IsDash
    {
        get { return _isDash; }
    }
    public float dashSpeedScale = 2f;// ダッシュ速度倍率
    public float dashTimePerItem = 0.5f;// アイテム一個でダッシュできる時間
    float _dashLimitTime = 0f;//ダッシュ上限時間
    public float DashLimitTime
    {
        get { return _dashLimitTime; }
    }
    float _dashCountDown = 0f;//ダッシュカウントダウン
    public float DashCountDown
    {
        get { return _dashCountDown; }
        set { _dashCountDown = value; }
    }
    public GameObject origin_Dash_Particle;//ダッシュパーティクル生成元
    GameObject dash_Particle;//ダッシュパーティクル

    bool dashStart = true;//ダッシュしたかどうか

    public float shockWavePower = 100;//衝撃波で吹き飛ぶ強さ

    public bool canHipDrop = false;//ヒップドロップできるようにするかどうか
    public float attackPower = 25;//プレイヤーにダッシュで体当たりして吹き飛ばす強さ
    [HideInInspector]
    public bool hitHipDrop = false;//ヒップドロップサークルに当たったか
    [HideInInspector]
    public bool isHit = false;//ダッシュしていない他プレイヤーに当たったか
    float hitTime = 0.1f;//BoxColliderを付けなおすまでの時間
    
    float playerPos_X;//X座標の移動制限（ステージセレクトで選ばれたステージに応じて、変更）
    float playerPos_Y;//Y座標の移動制限
    float playerPos_Z;//Z座標の移動制限（ステージセレクトで選ばれたステージに応じて、変更）

    [HideInInspector]
    public bool isBlastStan;
    [HideInInspector]
    public GameObject balloonMaster;

    float dashParticleTime = 0.0f;

    public bool isConstant = false;//ダッシュ上限を固定するか
    public float originDashLimit = 3.0f;//ダッシュ可能の最大時間の設定
    [HideInInspector]
    public SEController playerSE;//SEコントローラー

    public GameObject playerCol;//プレイヤーあたり判定

    [HideInInspector]
    public float totalItemCount_For_Text = 0;//トータル表示用数

    bool onConveyor = false;
    Vector3 direction;

    // Use this for initialization
    void Start()
    {
        //初期化処理
        isJump = false;
        holdItemCount = 0;
        isHipDrop = false;
        jumpCount = 0;
        rigid = GetComponent<Rigidbody>();
        rotationPosition = transform.position;
        stanTime = 0;

        //holdItemCountText = GameObject.Find(transform.name + "ItemCount").GetComponent<Text>();//内容物所持数テキスト取得
        totalItemCountText = GameObject.Find(transform.name + "TotalCount").GetComponent<Text>();
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

        //180607 ダッシュ
        SetDashLimitTime(holdItemCount, dashTimePerItem);
        _dashCountDown = _dashLimitTime;

        //180622 SE
        playerSE = transform.GetComponent<SEController>();
    }

    void Update()
    {
        holdItemCountText.text = HalfWidth2FullWidth.Set2FullWidth(holdItemCount);//内容物取得数表示処理 
        totalItemCountText.text = HalfWidth2FullWidth.Set2FullWidth(totalItemCount_For_Text);

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

        if (isStan)
        {
            return;
        }

        //移動&ジャンプ入力処理
        HandleXInput();

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log(jumpCount + "ジャンプカウント");
        }

        //ダッシュ中でなければ
        if (!_isDash)
        {
            dashStart = true;
        }

        //タックル関係
        IsTackle();
        //Clamp();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //動けないなら
        if (isStan)
        {
            //スタン中に動かないように修正
            if (!hitHipDrop)
            {
                rigid.velocity = Vector3.zero;
            }

            //最初に移動量をゼロに
            if (stanTime <= 0.0f)
            {
                //スタン中あたり判定のレイヤーを変えてプレイヤー同士当たらないようにする
                playerCol.layer = LayerMask.NameToLayer("PlayerStanHit");
                //rigid.velocity = Vector3.zero;
                GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                isStop = true;
                //星型パーティクル生成
                if (stan_Star_Particle == null)
                    stan_Star_Particle = Instantiate(origin_Stan_Star_Particle, transform);
            }

            rigid.AddForce(new Vector3(0, -9.8f * 5, 0));

            //時間で回復
            stanTime += Time.deltaTime;
            if (stanTime > originStanTime)
            {
                //レイヤーを戻す
                playerCol.layer = LayerMask.NameToLayer("PlayerHit");
                isStan = false;
                stanTime = 0.0f;
                if(isBlastStan)
                {
                    isBlastStan = false;
                    balloonMaster.GetComponent<BalloonMaster>().isRoulette = true;
                }
                if (stan_Star_Particle != null)
                {
                    //星型パーティクル削除
                    Destroy(stan_Star_Particle);
                }
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
    
        Vector3 diff = new Vector3(moveJoy.x, 0, moveJoy.y);

        diff.y = 0;

        if (diff.magnitude > 0.01f)
        {
            transform.rotation = Quaternion.LookRotation(diff);
        }

        ////ベルトコンベアに乗っていたら動く
        //if (onConveyor)
        //{
        //    rigid.velocity += direction;
        //}

        //Rayを飛ばしてベルトコンベアに当たっていたらベルトコンベアで動くようにする
        RaycastHit hit;
        if (Physics.Linecast(transform.position + Vector3.up, transform.position + Vector3.down, out hit, LayerMask.GetMask("BeltConveyor")))
        {
            var beltConveyor = hit.transform.gameObject.GetComponent<BeltConveyor>();
            if (beltConveyor != null)
            {
                direction = beltConveyor.Conveyor();
                rigid.velocity += direction;
            }
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
                playerSE.PlayPlayerSEOnce((int)SEController.PlayerSE.Jump);
                //GetComponent<AudioSource>().PlayOneShot(soundSE1);
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
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), new Vector3(transform.localScale.x * 2, transform.localScale.x * 2 * 2, transform.localScale.x * 2));
    }

    /// <summary>
    /// ジャンプ処理
    /// </summary>
    void Jump()
    {
        //地面にいるとき
        if (jumpCount == 0)
        {
            //重力設定
            gravPower = 9.8f;
        }

        //空中にいるとき
        if (jumpCount == 1)
        {
            //ジャンプ中
            isJump = true;
            //重力設定
            gravPower = 9.8f;
        }

        if (canHipDrop)
        {
            //ヒップドロップ中
            if (jumpCount == 2)
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
        rigid.AddForce(new Vector3(0, -gravPower * 5, 0));

        //地面いるか判定
        bool isField = false;

        //foreach (var cx in colArray)
        {
            //Debug.Log(cx.transform.name);
            //当たっているものが床かプレイヤーだったら
            //if ((cx.transform.tag == "Field")||(cx.transform.tag == "Player"&&cx.transform.gameObject != gameObject))
            //あたり判定を別のオブジェクトに任せた
            if (playerJumpHit.isJumpHit)
            {
                //位置を少し浮かす
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
                //地面にいる
                isField = true;
                //ジャンプ終える
                isJump = false;
                //ヒップドロップ中だったら
                if (isHipDrop && hipDropTime <= 0)
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
        if (!isField && jumpCount == 0)
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
            foreach (var cx in Physics.OverlapBox(transform.position + new Vector3(0.01f, 0.01f, 0) + new Vector3(0, 1, 0), new Vector3(transform.localScale.x * 2 / 2, transform.localScale.y * 2, transform.localScale.z * 2 / 2), transform.localRotation))
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
            ////自身がダッシュ中であり、ジャンプしていないとき
            //if (_isDash && jumpCount == 0)
            //{
            //    //相手のプレイヤーがスタン中でない、かつ、ダッシュ中でない、又は、ジャンプしていなければ
            //    if (!col.gameObject.GetComponent<PlayerMove>().isStan &&
            //        (!col.gameObject.GetComponent<PlayerMove>().IsDash || col.gameObject.GetComponent<PlayerMove>().jumpCount == 0))
            //    {
            //        //相手のプレイヤーを弾く
            //        col.gameObject.GetComponent<PlayerMove>().isHit = true;
            //        col.gameObject.GetComponent<Rigidbody>().AddForce((col.transform.position - transform.position + new Vector3(0, 0.1f, 0)) * attackPower,
            //            ForceMode.Impulse);
            //    }
            //}
        
    }

    void OnTriggerEnter(Collider col)
    {
        //プレイヤーに当たったら
        if (col.gameObject.tag == "Player")
        {
            if (balloon != null)//爆発物があれば
            {
                balloon.GetComponent<BalloonOrigin>().BalloonMove(transform.gameObject, col.gameObject);//爆発物の移動処理
                GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                isStop = true;
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
            b.GetComponent<BalloonOrigin>().BalloonExChange(pList, gameObject);
        }

        //衝撃波に当たったら
        if (col.gameObject.tag == "HipDropCircle")
        {
            //衝撃波ヒット時振動
            if (!col.transform.name.Contains(transform.name) && !isStan)
            {
                //GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                //isStop = true;
                isStan = true;
                ItemBlast(1);

                //衝撃波からプレイヤーの方向に向かって吹き飛ぶ
                rigid.AddForce((col.transform.position - transform.position) * -shockWavePower);
                hitHipDrop = true;
            }
        }
    }

    /// <summary>
    /// ポストに当たった際の処理
    /// </summary>
    /// <param name="post"></param>
    public void HitPost(GameObject post)
    {
        //トータルをテモチ分増やす
        totalItemCount += holdItemCount;
        //SE鳴らす
        playerSE.PlayPlayerSEOnce((int)SEController.PlayerSE.SendPost);
        //ポストにテモチ数を渡す
        post.GetComponent<PostController>().blastCount += holdItemCount;
        post.GetComponent<PostController>().respawnCount += holdItemCount;
        //ポストに自信を渡す
        post.GetComponent<PostController>().player = gameObject;
        //所持アイテムをトータルリストに渡す
        foreach (var cx in itemList)
        {
            totalItemList.Add(cx);
        }
        //アイテムリストクリア
        itemList.Clear();
        //内容物所持数を0にする
        holdItemCount = 0;
    }

    /// <summary>
    /// アイテム排出処理
    /// </summary>
    /// <param name="count">割合</param>
    public void ItemBlast(float count)
    {
        //if (itemList.Count > 0)
        //{
        //    int j = (int)(itemList.Count * (count / 10));//指定した割合で排出
        //    for (int i = 0; i < j; i++)
        //    {
        //        GameObject item = originItem;
        //        int itemNum = Random.Range(0, itemList.Count);

        //        switch (itemList[itemNum])//取得したアイテムからランダムで選出
        //        {
        //            case "PointItem(Clone)"://普通のアイテム
        //                blastCount--;
        //                break;
        //            case "HighPointItem(Clone)"://高ポイントアイテム
        //                blastCount -= 2;
        //                item = originHighItem;
        //                break;
        //        }
        //        itemList.RemoveAt(itemNum);//リストから削除
        //        GameObject spawnItem = Instantiate(item, transform.position+new Vector3(0,item.transform.localScale.y+3,0), Quaternion.Euler(90,0,0));//生成
        //        spawnItem.GetComponent<ItemController>().SetMovePosition();
        //        spawnItem.GetComponent<ItemController>().isGet = false;
        //    }
        //}

        //排出ポイント割合
        int itemRatio = (int)(holdItemCount * (count / 10));
        //排出ポイント割合が0になるまで排出
        while (itemRatio > 0)
        {
            //排出アイテム設定
            GameObject item = originItem;

            //2ポイント以下なら別設定
            if (itemRatio <= 2)
            {
                GameObject spawnItem;//排出させたアイテム
                int TwoOrOne = Random.Range(0, 2);

                //1/2の確率で、排出ポイント割合が2で、2ポイントアイテムをもっていたら
                if (TwoOrOne == 0 && itemRatio == 2 && itemList.Contains("2CoinPointItem(Clone)"))
                {
                    holdItemCount -= 2;//2ポイント減
                    itemRatio -= 2;//排出ポイント割合2ポイント減
                    item = originHighItem;//2ポイントアイテム排出
                    spawnItem = Instantiate(item, transform.position + new Vector3(0, item.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                    spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                    spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                    itemList.Remove("2CoinPointItem(Clone)");//2ポイントアイテム削除
                    break;
                }
                holdItemCount--;//ポイント減
                itemRatio--;//排出ポイント割合ポイント減
                item = originItem;//ポイントアイテム排出
                spawnItem = Instantiate(item, transform.position + new Vector3(0, item.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                itemList.Remove("1CoinPointItem(Clone)");//ポイントアイテム削除
            }
            //それ以外はランダム
            else
            {
                int itemNum = Random.Range(0, itemList.Count);//ランダム設定
                switch (itemList[itemNum])//取得したアイテムからランダムで選出
                {
                    case "1CoinPointItem(Clone)"://普通のアイテム
                        holdItemCount--;//ポイント減
                        itemRatio--;//排出ポイント割合ポイント減
                        item = originItem;//ポイントアイテム排出
                        break;
                    case "2CoinPointItem(Clone)"://高ポイントアイテム
                        holdItemCount -= 2;//2ポイント減
                        itemRatio -= 2;//排出ポイント割合2ポイント減
                        item = originHighItem;//2ポイントアイテム排出
                        break;
                }
                GameObject spawnItem = Instantiate(item, transform.position + new Vector3(0, item.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                itemList.RemoveAt(itemNum);//排出アイテム削除
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
                GameObject hipDrop1 = Instantiate(hipDropCircle[0], transform.position + new Vector3(0, 0.2f, 0), Quaternion.identity);
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

        //効果音追加
        //playerSE.PlayerPlayreSEOnce((int)SEController.PlayerSE.HipDrop);
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
        {
            //ダッシュ中
            if (_isDash)
            {
                //倍率によって乗算
                moveSpeed = originMoveSpeed * dashSpeedScale;
            }
            else moveSpeed = originMoveSpeed;
        }
        //風船を持っている時
        else
        {
            //ダッシュ中
            if (_isDash)
            {
                //倍率によって乗算
                moveSpeed = balloonMoveSpeed * dashSpeedScale;
            }
            else moveSpeed = balloonMoveSpeed;
        }

        PauseXInput();
        JumpXInput();
        if (!startCntDown.IsCntDown && !finishCall.IsCalling)
            DashXInput();

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
                playerSE.PlayPlayerSEOnce((int)SEController.PlayerSE.Jump);
                //GetComponent<AudioSource>().PlayOneShot(soundSE1);
            }
            //空中にいたら
            else if (jumpCount == 1)
            {
                hipDropTime = 0.3f;
                //rigid.velocity = Vector3.zero;
                hipDropPosition = transform.position;

                if (canHipDrop)
                {
                    //効果音追加
                    //playerSE.PlayerPlayreSEOnce((int)SEController.PlayerSE.HipDropTurnInAir);
                }
            }

            //ジャンプカウント増加
            jumpCount++;

            //上限設定
            if (jumpCount > 1)
                jumpCount = 1;
        }
    }

    public float dashTiredTime = 1f; //疲れた時間

    /// <summary>
    /// 追加日：180607 追加者：何
    /// ダッシュ入力
    /// </summary>
    void DashXInput()
    {
        if (_isDash)
        {
            //ダッシュ中にパーティクルを生成
            //if (dash_Particle == null) dash_Particle = Instantiate(origin_Dash_Particle, transform);
            dashParticleTime -= Time.deltaTime;
            if (dashParticleTime <= 0)
            {
                Instantiate(origin_Dash_Particle, transform.position, Quaternion.identity);
                dashParticleTime = 0.1f;
            }
        }
        else
        {
            SetDashLimitTime(holdItemCount, dashTimePerItem);

            //ダッシュ中ではない時パーティクルを削除
            //if (dash_Particle != null) Destroy(dash_Particle);
        }

        // RBボタン押している間
        if (currentState.Buttons.RightShoulder == ButtonState.Pressed)
        {
            //カウントダウン
            if (balloon != null)
            {
                //風船を持つプレイヤーは消費量半分
                _dashCountDown -= Time.deltaTime / 2f;
            }
            else
            {
                _dashCountDown -= Time.deltaTime;
            }

            if (_dashCountDown > 0)
            {
                _isDash = true;

                //ダッシュしたら1度だけ音を鳴らす
                if (dashStart)
                {
                    //効果音追加
                    playerSE.PlayPlayerSEOnce((int)SEController.PlayerSE.Dash);
                    //GetComponent<AudioSource>().PlayOneShot(soundSE6);
                }
                dashStart = false;
            }
            else
            {
                _dashCountDown = 0;
                _isDash = false;
            }
        }
        // RBボタン押してない間
        else
        {
            _isDash = false;

            //ゲージ切れでしたら１秒待つから回復
            if (_dashCountDown <= 0 && dashTiredTime > 0)
            {
                dashTiredTime -= Time.deltaTime;
            }
            else
            {
                //半分の速度でカウントダウン回復
                _dashCountDown += Time.deltaTime/* / 2f*/;
                dashTiredTime = 1f;
            }
        }

        //上限に超えないようにする
        if (_dashCountDown >= _dashLimitTime)
        {
            _dashCountDown = _dashLimitTime;
        }
    }

    /// <summary>
    /// 追加日：180607 追加者：何
    /// ダッシュタイム上限を設定
    /// </summary>
    /// <param name="itemAmount"></param>
    /// <param name="time"></param>
    void SetDashLimitTime(float itemAmount, float time)
    {
        //上限固定をしなければアイテムの個数に応じて上限を設定する
        if (!isConstant)
            _dashLimitTime = (itemAmount + 1) * time;
        //上限を固定する
        else
        {
            if (balloon == null)
                _dashLimitTime = originDashLimit;
            else
                _dashLimitTime = originDashLimit + 2.0f;
        }
    }

    SelectScript pauseScript;//ポーズ関連

    /// <summary>
    /// 追加日：180616 追加者：何
    /// ポーズ入力
    /// </summary>
    void PauseXInput()
    {
        if(pauseScript == null)
            pauseScript = GameObject.Find("PauseCtrl").GetComponent<SelectScript>();

        //ポーズ中じゃなかった
        if(pauseScript.pauseState == PauseState.NONEPAUSE)
        {
            //STARTボタンを押したら（ポーズ開始）解除->SelectScript
            if (previousState.Buttons.Start == ButtonState.Released &&
                currentState.Buttons.Start == ButtonState.Pressed)
            {
                pauseScript.PausePlayerIndex = playerIndex;//自分がポーズプレイヤー
                //pauseScript.IsStartBtnPushed = true;
            }
            else
            {
                //pauseScript.IsStartBtnPushed = false;
            }
        }
    }
    /// <summary>
    /// 他プレイヤーからタックルされたとき
    /// </summary>
    private void IsTackle()
    {
        //当たっていなければ
        if (!isHit)
        {
            transform.GetComponent<BoxCollider>().enabled = true;
            transform.GetChild(2).GetComponent<BoxCollider>().enabled = true;
            transform.GetChild(3).GetComponent<BoxCollider>().enabled = true;
        }
        else
        {
            transform.GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(2).GetComponent<BoxCollider>().enabled = false;
            transform.GetChild(3).GetComponent<BoxCollider>().enabled = false;
            hitTime -= Time.deltaTime;
            if (hitTime <= 0)
            {
                isHit = false;
                hitTime = 0.1f;
            }
        }
    }

    /// <summary>
    /// 移動制限
    /// </summary>
    private void Clamp()
    {
        //playerPos_X=Mathf.Clamp(transform.position.x,,);
        playerPos_Y = Mathf.Clamp(transform.position.y, 0.5f, 8f);
        //playerPos_Z=Mathf.Clamp(transform.position.z,,);

        if (GameObject.Find("SmallStage(Clone)"))
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -11.5f, 11.5f),
                playerPos_Y,
                Mathf.Clamp(transform.position.z, -11.5f, 11.5f));
        }

        if (GameObject.Find("SmallStage 1(Clone)"))
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -9.5f, 9.5f),
                playerPos_Y,
                Mathf.Clamp(transform.position.z, -9.5f, 9.5f));
        }

        if (GameObject.Find("SmallStage2(Clone)"))
        {
            transform.position = new Vector3(
                Mathf.Clamp(transform.position.x, -7.5f, 7.5f),
                playerPos_Y,
                Mathf.Clamp(transform.position.z, -7.5f, 7.5f));
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
