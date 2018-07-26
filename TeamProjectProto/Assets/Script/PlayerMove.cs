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
using System.IO;

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

    Rigidbody rigid;//リジットボディ

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

    [HideInInspector]
    public PlayerState playerState = PlayerState.NormalAI;//プレイヤー操作状態
    AIMapController aiMapController;//AIのマップ準備クラス
    float[][] influenceMap;//影響マップ
    float[][] otherInfluenceMap;//他のプレイヤーの影響マップ
    [HideInInspector]
    public float[][] coinInfluenceMap;//アイテムの影響マップ

    float aiJumpTime = 0;//AIのジャンプ間隔
    float currentNearBlockHeight = 0;//現在の近いブロック
    float previousNearBlockHeight = 0;//1フレーム前のブロック

    float currentFarBlockHeight = 0;//現在の遠いブロック
    float previousFarBlockHeight = 0;//1フレーム前のブロック
    
    Vector3 nowPoint = Vector3.zero;//現在の位置

    string[][] aiPersonalityArray;

    TimeController timeController;

    public bool isAI;

    // Use this for initialization
    void Start()
    {
        if (isAI)
            playerState = PlayerState.NormalAI;
        else
            playerState = PlayerState.CONTROLLER;
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

        if (playerState != PlayerState.CONTROLLER)
            AIInitialize();

        //時間オブジェ取得
        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
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
                PlayerAI();//AIメソッド
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
            isJump = false;;
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
            }
        }
    }

    /// <summary>
    /// AIの初期化メソッド
    /// </summary>
    void AIInitialize()
    {
        //影響マップ準備取得
        aiMapController = GameObject.Find("AIMapController").GetComponent<AIMapController>();

        //各影響配列初期化
        influenceMap = new float[(int)aiMapController.mapHeight][];
        otherInfluenceMap = new float[(int)aiMapController.mapHeight][];
        coinInfluenceMap = new float[(int)aiMapController.mapHeight][];

        int I = 0, J = 0;//配列位置保存用int
        float l = 100;//距離保存用float

        //全セル判定
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            //各二重配列初期化
            influenceMap[i] = new float[(int)aiMapController.mapWidth];
            otherInfluenceMap[i] = new float[(int)aiMapController.mapWidth];
            coinInfluenceMap[i] = new float[(int)aiMapController.mapWidth];

            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //各影響マップの影響値初期化
                influenceMap[i][j] = 0;
                if (aiMapController.stageArray[i][j] == "-1")
                    //ステージセルがプレイヤー進入禁止エリアだったら影響値下げる
                    influenceMap[i][j] = -10000;
                otherInfluenceMap[i][j] = 0;
                coinInfluenceMap[i][j] = 0;

                //現在のポジションと取得したセルの距離判定
                float Length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(transform.position.x, aiMapController.positionArray[i][j].y, transform.position.z));

                //距離が1.5以下
                if (Length <= 1.5f)
                {
                    //保存した距離より短ければ
                    if (Length <= l)
                    {
                        //距離更新
                        l = Length;
                        //配列位置保存
                        I = i;
                        J = j;
                    }
                }
            }
        }
        //一番近いセルの位置を保存
        currentNearBlockHeight = previousNearBlockHeight = float.Parse(aiMapController.stageArray[I][J]);

        aiPersonalityArray = ArraySet();
    }

    string[][] ArraySet()
    {
        //現在のステージに対応したcsv読み込み
        StreamReader sr = new StreamReader("Assets/AIPersonality/" + playerState.ToString() + ".csv");
        //リスト作成
        List<string[]> sList = new List<string[]>();
        //リストに各値収納
        while (sr.EndOfStream == false)
        {
            string line = sr.ReadLine();
            sList.Add(line.Split(','));
        }
        sr.Close();
        //リスト返す
        return sList.ToArray();
    }

    /// <summary>
    /// AIプレイヤーの挙動メソッド
    /// </summary>
    void PlayerAI()
    {
        //各影響マップ作製
        ObjectInfluenceSet();
        //影響マップ作製
        MapInfluenceSet();

        Vector3 nextPoint = Vector3.zero;//次のポイント
        Vector3 nearPoint = Vector3.zero;//影響値が高い近いポイント
        Vector3 farPoint = Vector3.zero;//影響値が高い遠いポイント
        float nearInfluence = 0;//近いポイントの影響値
        float farInfluence = 0;//遠いポイントの影響値

        //近いポイントセット
        NextPointSet(out nearPoint, out nearInfluence, currentNearBlockHeight, 1.5f);
        //遠いポイントセット
        NextPointSet(out farPoint, out farInfluence, currentFarBlockHeight, 3.5f);

        //近めを目指すか遠目を目指すか
        NearFarCheck(out nextPoint, nearPoint, farPoint, nearInfluence, farInfluence);
        
        //AIの移動処理
        AIMove(nextPoint);
        
        //ジャンプ間隔更新
        aiJumpTime -= Time.deltaTime;
        //前方にあたり判定飛ばす
        Collider[] colArray = Physics.OverlapBox(transform.position + new Vector3(moveJoy.x, 1,moveJoy.y),new Vector3(0.5f,1.0f,0.5f));
        foreach (var cx in colArray)
        {
            //フィールドに当たっていたら
            if (cx.transform.tag == "Field")
            {
                //AIのジャンプ処理
                AIJump();
            }
            
        }

        //1フレーム前の値を現在の値に更新
        previousNearBlockHeight = currentNearBlockHeight;
        previousFarBlockHeight = currentFarBlockHeight;
        nowPoint = nextPoint;
    }

    /// <summary>
    /// オブジェクト影響値セット
    /// </summary>
    void ObjectInfluenceSet()
    {
        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //各影響値初期化
                otherInfluenceMap[i][j] = 0;
                coinInfluenceMap[i][j] = 0;
            }
        }

        //自分以外の影響値セット
        foreach (var otherPlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (otherPlayer.name != gameObject.name)
            {
                ObjectMapSet_Player(otherPlayer, false, otherInfluenceMap);
            }
        }

        //アイテムの影響値セット
        foreach (var coin in GameObject.FindGameObjectsWithTag("Item"))
        {
            int I = 0, J = 0;//配列位置保存
            float l = 100;//距離保存

            //全セル取得
            for (int i = 0; i < aiMapController.mapHeight; i++)
            {
                for (int j = 0; j < aiMapController.mapWidth; j++)
                {
                    //取得したセルと取得したアイテムとの距離取得
                    float Length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(coin.transform.position.x, aiMapController.positionArray[i][j].y, coin.transform.position.z));

                    //距離が1.5以下で
                    if (Length <= 1.5f)
                    {
                        //距離が保存した値以下なら
                        if (Length <= l)
                        {
                            //距離更新
                            l = Length;
                            //配列位置保存
                            I = i;
                            J = j;
                        }
                    }
                }
            }

            //プレイヤーの位置が床で、コインの位置が高い壁の位置なら
            if (float.Parse(aiMapController.stageArray[I][J]) == 2 && currentNearBlockHeight < 1)
                //影響値を設定しない
                continue;

            //影響値を設定する
            ObjectMapSet_Player(coin, false, coinInfluenceMap);
        }
    }

    /// <summary>
    /// 影響マップ作製
    /// </summary>
    void MapInfluenceSet()
    {
        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {

                //影響値セット
                //エクセルからどの影響割合を取り出すか

                int infState = 1;//取り出す添え字
                
                //鬼との距離が8以下なら
                if (balloonMaster.nowPlayer != null&&Vector3.Distance(transform.position, balloonMaster.nowPlayer.transform.position) <= 8.0f)
                    infState = 4;
                //ロスタイム中なら
                else if (timeController.timeState == TimeState.LOSSTIME)
                    infState = 3;
                //バルーンが貯金を破裂させるバルーンなら
                else if (balloonMaster.NowBalloon != null && balloonMaster.NowBalloon.name.Contains("PostBalloon"))
                    infState = 2;
                //それ以外
                else
                    infState = 1;
                //バルーンが　ついていなければ
                if (balloon == null)
                {
                    //影響値セット
                    influenceMap[i][j] =
                    aiMapController.balloonInfluenceMap[i][j] * float.Parse(aiPersonalityArray[infState][1]) +
                    aiMapController.postInfluenceMap[i][j] * float.Parse(aiPersonalityArray[infState][2]) +
                    coinInfluenceMap[i][j] * float.Parse(aiPersonalityArray[infState][3]) +
                    ReturnInfluence(otherInfluenceMap[i][j]) * float.Parse(aiPersonalityArray[infState][4]);

                
                    //現在の位置が床の場合、高い壁の影響値は0
                    if (float.Parse(aiMapController.stageArray[i][j]) == 2 && previousNearBlockHeight == 0)
                        influenceMap[i][j] = 0;
                }
                else
                { //影響値セット
                    influenceMap[i][j] =
                    aiMapController.balloonInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][1]) +
                    aiMapController.postInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][2]) +
                    coinInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][3]) +
                    otherInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][4]);

                }

                //セルがプレイヤー進入禁止エリアだったら
                if (float.Parse(aiMapController.stageArray[i][j]) <= -1)
                    //影響値0
                    influenceMap[i][j] = 0;
            }
        }
    }

    /// <summary>
    /// 各オブジェクト影響値設定
    /// </summary>
    /// <param name="obj">オブジェクト</param>
    /// <param name="isReturn">反転させるかどうか</param>
    /// <param name="objInfluenceMap">影響マップ</param>
    void ObjectMapSet_Player(GameObject obj, bool isReturn, float[][] objInfluenceMap)
    {
        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //オブジェクトと取得したセルの距離を取得
                float Length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(obj.transform.position.x, aiMapController.positionArray[i][j].y, obj.transform.position.z));

                float influence = 0;//影響値

               //距離に応じた影響値を設定
                if (isReturn)
                    //反転させるなら
                    //近いほど影響値低く設定
                    influence = Length / 20;
                else
                    //反転させないなら
                    //近いほど影響値高く設定
                    influence = 1 - Length / 20;
                //影響値の範囲を0から1の間で設定
                if (influence < 0)
                    influence = 0;
                else if (influence > 1)
                    influence = 1;

                //取り出したセルの影響値より求めた影響値が高ければ
                if (objInfluenceMap[i][j] < influence)
                {
                    //影響値更新
                    objInfluenceMap[i][j] = influence;
                }
            }
        }
    }

    /// <summary>
    /// 影響値反転メソッド
    /// </summary>
    /// <param name="objectInfuence">影響マップ</param>
    /// <returns>影響マップ</returns>
    float ReturnInfluence(float objectInfuence)
    {
        float returnInfluence = 1 - objectInfuence;

        return returnInfluence;
    }

    /// <summary>
    /// 次に目指すポイントをセット
    /// </summary>
    /// <param name="point">次のポイント</param>
    /// <param name="influence">影響値</param>
    /// <param name="blockHight">ブロックの高さ</param>
    /// <param name="findRadius">検索範囲</param>
    void NextPointSet(out Vector3 point, out float influence, float blockHight, float findRadius)
    {
        //初期化
        point = Vector3.zero;
        influence = 0;

        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //取得したセルと現在位置との距離取得
                float length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(transform.position.x, aiMapController.positionArray[i][j].y, transform.position.z));

                //距離が検索範囲以内　かつ　0.5以上（近すぎるのは判定外）
                if (length <= findRadius && length >= 0.5f) 
                {
                    //取得したセルの影響値が保存している影響値より高かったら
                    if (influenceMap[i][j] > influence)
                    {
                        //保存影響値更新
                        influence = influenceMap[i][j];
                        //次に目指す位置設定
                        point = aiMapController.positionArray[i][j];
                        //目指す位置のブロック位置設定
                        blockHight = float.Parse(aiMapController.stageArray[i][j]);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 近場を目指すか遠目を目指すか（谷判定用）
    /// </summary>
    /// <param name="nextPoint">次のポイント</param>
    /// <param name="nearPoint">近場で影響値が高いポイント</param>
    /// <param name="farPoint">遠目で影響値が高いポイント</param>
    /// <param name="nearInfluence">近場のポイントの影響値</param>
    /// <param name="farInfluence">遠目のポイントの影響値</param>
    void NearFarCheck(out Vector3 nextPoint, Vector3 nearPoint, Vector3 farPoint, float nearInfluence, float farInfluence)
    {
        /*
        自身が壁の上にいる場合に谷を跳び越すかどうかの判定に使う
        遠目を目指す、自身が壁の上以上、目指す位置が高い壁の上、間に壁がない　なら谷判定
        */

        //遠目の影響値のほうが高く　かつ
        //1フレーム前（移動前の今いる位置）のブロックが壁以上　かつ
        //現在（移動前の目指す場所）が高い壁ならば
        if (nearInfluence < farInfluence && 
            previousNearBlockHeight >= 1 &&
            currentFarBlockHeight == 2)
        {
            //レイに当たった物体
            RaycastHit hit;

            //レイを飛ばす　自分が立っている壁の底辺から1.5（1セル分）のレイ
            if (Physics.Raycast(new Vector3(nowPoint.x, -2, nowPoint.z), (farPoint - nowPoint).normalized, out hit, 1.5f))
            {
                //もし壁以外があれば
                if (hit.transform.tag != "Field")
                {
                    //遠目を目指す
                    nextPoint = farPoint;
                    //ジャンプ
                    AIJump();
                }
                //壁があれば
                else
                {
                    //近場を目指す
                    nextPoint = nearPoint;
                }
            }
            //何もレイが当たらなければ
            else
            {
                //遠目を目指す
                nextPoint = farPoint;
                //ジャンプ
                AIJump();
            }
        }
        //普通は
        else
        {
            //近場を目指す
            nextPoint = nearPoint;
        }
    }

    /// <summary>
    /// AIのジャンプ処理
    /// </summary>
    void AIJump()
    {
        //AIのジャンプ間隔が来たなら
        if (aiJumpTime <= 0.0f)
        {
            //ジャンプパワー設定
            jumpPower = balloon != null ? balloonJumpPower * 700 : originJumpPower * 700;

            //地面にいたら
            if (jumpCount == 0)
            {
                //ジャンプ
                rigid.AddForce(new Vector3(0, jumpPower, 0));
                //SE鳴らす
                playerSE.PlayPlayerSEOnce((int)SEController.PlayerSE.Jump);
                //ジャンプ間隔再設定
                aiJumpTime = 0.3f;
            }

            //ジャンプカウント増加
            jumpCount++;

            //上限設定
            if (jumpCount >= 1)
                jumpCount = 1;
        }
    }

    /// <summary>
    /// AIの移動処理
    /// </summary>
    /// <param name="nextPoint">次の位置</param>
    void AIMove(Vector3 nextPoint)
    {
        //方向指定
        Vector3 moves = nextPoint - transform.position;
        //正規化
        moves = moves.normalized;

        //ジャンプ中は方向変えない
        if (jumpCount < 1)
        {
            //移動に渡す
            moveJoy.x = moves.x;
            moveJoy.y = moves.z;

            //正規化
            moveJoy = moveJoy.normalized;
        }

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
