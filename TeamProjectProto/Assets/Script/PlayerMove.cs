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
//using System.IO;

//プレイヤーの操作状態
public enum PlayerState
{
    CONTROLLER,//コントローラーで操作
    NormalAI,//普通のAIで操作
    AfraidAI,//怖がりのAIで操作
    GamblerAI,//ギャンブラーのAIで操作
}

public class PlayerMove : MonoBehaviour
{
    public float AxisX = 0;//プレイヤーのｘ移動方向
    public float AxisZ = 0;//プレイヤーのｚ移動方向
    [HideInInspector]
    public float moveSpeed = 0;//移動速度
    public float originMoveSpeed = 0.5f;//プレイヤーの移動速度    
    public float balloonMoveSpeed = 0.7f;//爆発物を持っている時の移動速度

    bool isJump = false;//ジャンプしているか
    public float originJumpPower = 0.2f;//基本ジャンプの上方向の力
    float gravPower = 0.1f;//重力の力
    [HideInInspector]
    public float jumpPower = 0;//今のジャンプの上方向の力

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

    public bool isStan = false;//動けるかどうか
    public float originStanTime = 1.5f;//動けるようになるまでの時間
    [HideInInspector]
    public float stanTime;

    [HideInInspector]
    public float jumpCount = 0;//ジャンプ回数
    public GameObject originItem;//アイテム
    public GameObject originHighItem;//ハイアイテム

    [HideInInspector]
    public List<string> itemList;//取得アイテム管理リスト
    [HideInInspector]
    public List<string> totalItemList;//累計取得アイテム管理リスト

    [HideInInspector]
    public Rigidbody rigid;//リジットボディ

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
        set { _isDash = value; }
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

    [HideInInspector]
    public bool dashStart = true;//ダッシュしたかどうか

    [HideInInspector]
    public bool isHit = false;//ダッシュしていない他プレイヤーに当たったか
    float hitTime = 0.1f;//BoxColliderを付けなおすまでの時間

    float playerPos_X;//X座標の移動制限（ステージセレクトで選ばれたステージに応じて、変更）
    float playerPos_Y;//Y座標の移動制限
    float playerPos_Z;//Z座標の移動制限（ステージセレクトで選ばれたステージに応じて、変更）

    [HideInInspector]
    public bool isBlastStan;
    [HideInInspector]
    public BalloonMaster balloonMaster;

    [HideInInspector]
    public float dashParticleTime = 0.0f;

    public bool isConstant = false;//ダッシュ上限を固定するか
    public float originDashLimit = 3.0f;//ダッシュ可能の最大時間の設定
    [HideInInspector]
    public SEController playerSE;//SEコントローラー

    public GameObject playerCol;//プレイヤーあたり判定

    [HideInInspector]
    public float totalItemCount_For_Text = 0;//トータル表示用数

    bool onConveyor = false;
    Vector3 direction;

    PlayerState playerState = PlayerState.CONTROLLER;//プレイヤー操作状態
    public PlayerState PlayerAIState
    {
        get { return playerState; }
        set { playerState = value; }
    }

    PlayerAI playerAI;

    // Use this for initialization
    void Start()
    {
        //初期化処理
        isJump = false;
        holdItemCount = 0;
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

        //プレイヤー操作状態がAIだったら
        if (playerState != PlayerState.CONTROLLER)
        {
            playerAI = gameObject.AddComponent<PlayerAI>();
            //AI初期化処理
            playerAI.AIInitialize();
        }
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

        //プレイヤーの操作状態で判定
        switch (playerState)
        {
            case PlayerState.CONTROLLER://コントローラーだったら
                //移動&ジャンプ入力処理
                HandleXInput();
                break;
            default://AIだったら
                if (!startCntDown.IsCntDown && !finishCall.IsCalling)
                    playerAI.PlayerAIThink();//AIメソッド
                break;
        }
        //ダッシュ中でなければ
        if (!_isDash)
        {
            dashStart = true;
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //動けないなら
        if (isStan)
        {
            rigid.velocity = Vector3.zero;

            //最初に移動量をゼロに
            if (stanTime <= 0.0f)
            {
                //スタン中あたり判定のレイヤーを変えてプレイヤー同士当たらないようにする
                playerCol.layer = LayerMask.NameToLayer("PlayerStanHit");
                //rigid.velocity = Vector3.zero;
                //プレイヤーがAIではなかったら
                if(playerState == PlayerState.CONTROLLER)
                {
                    GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                }
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
                if (isBlastStan)
                {
                    isBlastStan = false;
                    balloonMaster.isRoulette = true;
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

        //Rayを飛ばしてベルトコンベアに当たっていたらベルトコンベアで動くようにする
        RayHitBeltConveyor();
        //RaycastHit hit;
        //if (Physics.Linecast(transform.position + Vector3.up, transform.position + Vector3.down, out hit, LayerMask.GetMask("BeltConveyor")))
        //{
        //    var beltConveyor = hit.transform.gameObject.GetComponent<BeltConveyor>();
        //    if (beltConveyor != null)
        //    {
        //        direction = beltConveyor.Conveyor();
        //        rigid.velocity += direction;
        //    }
        //}
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    public void Move()
    {
        //移動vector生成
        Vector3 moveVector = Vector3.zero;

        //移動量設定
        moveVector.x = moveSpeed * moveJoy.x;
        moveVector.z = moveSpeed * moveJoy.y;

        //y方向無しの現在のvelocity保存
        Vector3 rigidVelocity = new Vector3(rigid.velocity.x, 0, rigid.velocity.z);

        //移動量追加
        rigid.AddForce(100 * (moveVector - rigidVelocity));

        //移動量2.5倍
        rigidVelocity = new Vector3(rigidVelocity.x * 2.5f, rigid.velocity.y, rigidVelocity.z * 2.5f);

        //設定
        rigid.velocity = rigidVelocity;

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

        //重力追加
        rigid.AddForce(new Vector3(0, -gravPower * 5, 0));

        //地面いるか判定
        bool isField = false;

        //あたり判定を別のオブジェクトに任せた
        if (playerJumpHit.isJumpHit)
        {
            //位置を少し浮かす
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.01f, transform.position.z);
            //地面にいる
            isField = true;
            //ジャンプ終える
            isJump = false; ;
            //地面にいる状態に変更
            if (jumpCount > 0)
            {
                jumpCount = 0;
            }
        }


        //地面にいない判定　かつ　地面にいる状態だったら（壁から落ちる）
        if (!isField && jumpCount == 0)
        {
            //ジャンプ状態に変更
            jumpCount = 1;
        }
    }

    void OnTriggerStay(Collider col)
    {
        //プレイヤーに当たったら
        if (col.gameObject.tag == "Player")
        {
            if (balloon != null)//爆発物があれば
            {
                balloon.GetComponent<BalloonOrigin>().BalloonMove(transform.gameObject, col.gameObject);//爆発物の移動処理
                if (playerState == PlayerState.CONTROLLER) 
                {
                    GamePad.SetVibration(XDInput[(int)(playerIndex)], 0.0f, 1.0f);
                }
                isStop = true;
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
    /// 追加日：180516 追加者：何
    /// プレイヤーアニメーション
    /// </summary>
    /// <param name="anim"></param>
    private void PlayerAnim(Animator anim)
    {
        anim.SetBool("isJump", isJump);//`ジャンプ
        anim.SetFloat("velocity", Mathf.Abs(rigid.velocity.x) <= 0.001f && Mathf.Abs(rigid.velocity.z) <= 0.001f ? 0 : 1);//移動
        anim.SetBool("isStan", isStan);//スタン
    }

    [HideInInspector]
    public PlayerIndex playerIndex;
    GamePadState previousState;
    GamePadState currentState;

    Vector2 moveJoy;
    public Vector2 MoveJoy
    {
        get
        {
            return moveJoy;
        }
        set
        {
            moveJoy = value;
        }
    }

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
    public void SetDashLimitTime(float itemAmount, float time)
    {
        //上限固定をしなければアイテムの個数に応じて上限を設定する
        if (!isConstant)
        {
            _dashLimitTime = (itemAmount + 1) * time;
            if (playerState != PlayerState.CONTROLLER && _dashLimitTime >= 5.0f)
                _dashLimitTime = 5.0f;
        }
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
        if (pauseScript == null)
            pauseScript = GameObject.Find("PauseCtrl").GetComponent<SelectScript>();

        //ポーズ中じゃなかった
        if (pauseScript.pauseState == PauseState.NONEPAUSE)
        {
            //STARTボタンを押したら（ポーズ開始）解除->SelectScript
            if (previousState.Buttons.Start == ButtonState.Released &&
                currentState.Buttons.Start == ButtonState.Pressed)
            {
                pauseScript.PausePlayerIndex = playerIndex;//自分がポーズプレイヤー
            }
        }
    }

    private void RayHitBeltConveyor()
    {
        RaycastHit hit;
        if (Physics.Linecast(transform.position + Vector3.up, transform.position + Vector3.down, out hit, LayerMask.GetMask("BeltConveyor")))
        {
            var beltConveyor = hit.transform.gameObject.GetComponent<BeltConveyor>();
            if (beltConveyor != null)
            {
                direction = beltConveyor.Conveyor();
                rigid.velocity += direction;
                //進行方向がx座標ならば縦中央にもっていく
                if (Mathf.Abs(direction.x) >= 1)
                {
                    var centerDir = (hit.transform.position - transform.position).normalized;
                    if (Mathf.Abs(centerDir.z) > 0.5f)
                        transform.position += new Vector3(0, 0, centerDir.z * Time.deltaTime);
                }
                //進行方向がz座標ならば横中央にもっていく
                else if (Mathf.Abs(direction.z) >= 1)
                {
                    var centerDir = (hit.transform.position - transform.position).normalized;
                    if (Mathf.Abs(centerDir.x) > 0.5f)
                        transform.position += new Vector3(centerDir.x * Time.deltaTime, 0, 0);
                }
            }
        }
    }
}
