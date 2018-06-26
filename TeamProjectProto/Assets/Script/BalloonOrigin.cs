//
//作成日時：6月8日
//バルーン基底クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; //コントローラー振動用
using DG.Tweening;

/// <summary>
/// 爆発物の状態
/// </summary>
public enum BalloonState
{
    SAFETY,//安全
    CAUTION,//注意
    DANGER//危険
}

/// <summary>
/// バルーン継承元クラス
/// </summary>
public class BalloonOrigin : MonoBehaviour
{
    [HideInInspector]
    public GameObject player;//プレイヤー

    [HideInInspector]
    public float scaleCount = 1;//現在の大きさ
    public float scaleLimit = 3;//大きさ限界
    float scaleRate = 0;//拡大率

    [HideInInspector]
    public float blastCount = 0;//内容物総数
    public float blastLimit = 30;//内容物総数限界

    float moveTime = 2;//爆発物が移る際のインターバル
    bool isMove = true;//爆発物が移れるかどうか

    public float setStopTime; //Unity側でのコントローラー振動停止までの時間設定用
    private float stopTime = 1.0f; //振動してから止まるまでのタイムラグ
    private bool isStop = false; //振動を止めるかどうか

    BalloonState _balloonState;//爆発物の状態
    public BalloonState BalloonState
    {
        get { return _balloonState; }
    }

    public bool isTimeBlast = false;//時間経過で爆破の切り替え
    public float originBlastTime = 1.0f;//爆発物が膨らむ間隔
    float blastTime;

    public AudioClip soundSE1;//風船が移るときの音
    public AudioClip soundSE2;//破裂時の音

    bool _isBlast = false;//爆発したか（エフェクト用）
    public bool IsBlast
    {
        get { return _isBlast; }
        set { _isBlast = value; }
    }

    float angle = 0;//上下移動遷移用角度

    [HideInInspector]
    public bool isDestroy = false;//風船に移るオブジェクトを破棄させるための判定
    GameObject air;//風船に移るオブジェクト

    Vector3 BalloontransfromSave; //balloon移動開始前の座標
    float height;//PlayerからBalloonまでの高さ

    BalloonState preState;
    BalloonState curState;
    bool _isColorChaged = false;
    public bool IsColorChanged
    {
        get { return _isColorChaged; }
    }
    
    FinishCall finishCall;//終了合図Script

    public bool isCameraDistance = false;

    [HideInInspector]
    public BalloonMaster balloonMaster;//バルーン総合管理クラス

    //各状態の色
    //public Color safeColor;
    //public Color coutionColor;
    //public Color dangerColor;

    public List<Texture> balloonStateTexture;//バルーン状態テクスチャリスト

    [HideInInspector]
    public Collider[] detonationList;//誘爆範囲に入ったプレイヤーリスト
    [HideInInspector]
    public float detonationRadius = 4.0f;//誘爆半径
    public GameObject originDetonationArea;//誘爆半径表示オブジェクト
    GameObject detonationArea;

    bool isTexSet = true;//テクスチャ設定用bool

    // Use this for initialization
    void Start()
    {
        //初期化処理
        scaleCount = 1;
        moveTime = 3.0f;
        isMove = true;
        _balloonState = BalloonState.SAFETY;
        scaleRate = scaleLimit / blastLimit;
        blastCount = 0;
        blastTime = originBlastTime;

        stopTime = setStopTime; //振動してから止まるまでのタイムラグ
        isStop = false; //振動を止めるかどうか

        isDestroy = false;
        BalloontransfromSave = player.transform.position;
        height = 2.5f;

        preState = _balloonState;
        curState = _balloonState;

        finishCall = GameObject.Find("FinishCall").GetComponent<FinishCall>();//終了処理オブジェクト取得
        
    }

    // Update is called once per frame
    void Update()
    {
        //裏コマンド
        if (Input.GetKeyDown(KeyCode.A))
        {
            BalloonBlast();
        }

        //スタートカウントダウン中＆終了処理時膨らまない
        if (!finishCall.IsCalling)
        {
            //時間経過で膨らむ処理
            if (isTimeBlast)
            {
                blastTime -= Time.deltaTime;
                if (blastTime <= 0)
                {
                    BalloonBlast();
                    blastTime = originBlastTime;
                }
            }
        }

        transform.localScale = new Vector3(scaleCount, scaleCount, scaleCount);//内容物の数により大きさ変更    

        //一度移ってから再度移るまで3秒のインターバルが存在する
        if (!isMove)
        {
            moveTime -= Time.deltaTime;
            if (moveTime < 0)
            {
                moveTime = 2;
                isMove = true;
            }
        }
        
        ColorChange();//色変更

        _isColorChaged = CheckColorChange();

        detonationList = Physics.OverlapCapsule(player.transform.position + new Vector3(0, 1, 0),
                                                player.transform.position + new Vector3(0, 3, 0),
                                                detonationRadius,
                                                1 << LayerMask.NameToLayer("Player"));//円形のあたり判定で誘爆範囲指定して入ったプレイヤー設定
        if (detonationArea != null)//プレイヤーに誘爆半径を追従
            detonationArea.transform.parent = player.transform;

        transform.rotation = player.transform.rotation;
    }

    void FixedUpdate()
    {
        //BalloonがついているPlayerのコントローラーのAxisを取得
        Vector3 playerAxis = new Vector3(player.GetComponent<PlayerMove>().AxisX, 0.0f, player.GetComponent<PlayerMove>().AxisZ);
        //BalloonがついているPlayerの座標を取得
        Vector3 playertransfrom = player.transform.position;
        //BalloonがついているPlayerとBalloonの差
        Vector3 balloonPlayer = new Vector3(player.transform.position.x - BalloontransfromSave.x, player.transform.position.y - BalloontransfromSave.y + height, player.transform.position.z - BalloontransfromSave.z);
        //BalloonがついているPlayerとBalloonの差の絶対値
        Vector3 balloonPlayerAbs = new Vector3(Mathf.Abs(balloonPlayer.x), Mathf.Abs(balloonPlayer.y), Mathf.Abs(balloonPlayer.z));

        //常にプレイヤーの上にいるようにする
        if (player != null)
        {
            //Playerの後を追うように移動させる処理

            if (playerAxis.z > -0.1)
            {
                height = 5.5f;
            }
            else {
                height = 2.5f;
            }

            if (playerAxis.x < -0.1f || playerAxis.x > 0.1f || playerAxis.z < -0.1f || playerAxis.z > 0.1f)
            {
                if (balloonPlayerAbs.x > 0.9 || balloonPlayerAbs.z > 0.9)
                {
                    BalloontransfromSave += balloonPlayer / 20;
                }
            }
            else {
                BalloontransfromSave += balloonPlayer / 20;
            }

            float range = 0.5f;//振れ幅
            transform.position = new Vector3(BalloontransfromSave.x, BalloontransfromSave.y + transform.localScale.y / 2 + range + (Mathf.Sin(angle) * range), BalloontransfromSave.z);//揺れながらプレイヤーの上に配置処理
            angle += 0.05f;//角度増加
        }
    }

    /// <summary>
    /// 色変更
    /// </summary>
    void ColorChange()
    {
        if (blastCount < blastLimit * 1 / 3) //1.7以下なら安全状態
            _balloonState = BalloonState.SAFETY;

        switch (_balloonState)
        {
            //1/3以下なら安全状態
            //色は緑
            case BalloonState.SAFETY:
                if(isTexSet)
                {
                    //テクスチャ設定
                    gameObject.GetComponentInChildren<MeshRenderer>().materials[0].mainTexture = balloonStateTexture[0];
                    //明るさ
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f));
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_EmissionMap", balloonStateTexture[0]);
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].EnableKeyword("_EMISSION");
                    isTexSet = false;
                }
                if (blastCount > blastLimit * 1 / 3)
                {
                    //次の状態へ
                    isTexSet = true;
                    _balloonState = BalloonState.CAUTION;
                }
                break;

            //1/3以上なら注意状態
            //色は黄色
            case BalloonState.CAUTION:
                if (isTexSet)
                {
                    //テクスチャ設定
                    gameObject.GetComponentInChildren<MeshRenderer>().materials[0].mainTexture = balloonStateTexture[1];
                    //明るさ
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f));
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_EmissionMap", balloonStateTexture[1]);
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].EnableKeyword("_EMISSION");
                    isTexSet = false;
                }
                if (blastCount > blastLimit * 2 / 3)
                {
                    isTexSet = true;
                    _balloonState = BalloonState.DANGER;
                }
                break;

            //2/3以上なら危険状態
            //色は赤
            case BalloonState.DANGER:
                if (detonationArea == null)
                {
                    //テクスチャ設定
                    //誘爆半径表示オブジェクト生成
                    detonationArea = Instantiate(originDetonationArea, player.transform.position + new Vector3(0, 1, 0), Quaternion.identity, player.transform);
                    detonationArea.transform.localScale = new Vector3(detonationRadius * 4, detonationRadius * 4, detonationRadius * 4);
                }
                {
                    gameObject.GetComponentInChildren<MeshRenderer>().materials[0].mainTexture = balloonStateTexture[2];
                    //明るさ
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f));
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_EmissionMap", balloonStateTexture[2]);
                    transform.GetComponentInChildren<MeshRenderer>().materials[0].EnableKeyword("_EMISSION");
                    isTexSet = false;
                }
                break;
        }
    }

    /// <summary>
    /// 爆発物移動処理
    /// </summary>
    /// <param name="player1">移動元</param>
    /// <param name="player2">移動先</param>
    public void BalloonMove(GameObject player1, GameObject player2)
    {
        if (isMove)
        {
            player1.GetComponent<PlayerMove>().balloon = null;//移動元の爆発物をNULLに
            player2.GetComponent<PlayerMove>().balloon = transform.gameObject;//移動先に自身を指定
            player = player2;
            balloonMaster.nowPlayer = player2;
            isMove = false;
            player.GetComponent<PlayerMove>().isStan = true;
            //ダッシュ回復
            //player.GetComponent<PlayerMove>().DashCountDown = player.GetComponent<PlayerMove>().DashLimitTime;
            GetComponent<AudioSource>().PlayOneShot(soundSE1);
        }
    }
    /// <summary>
    /// 爆破物ランダム移動処理(1人を除く)
    /// </summary>
    /// <param name="playerList"></param>
    public void BalloonExChange(GameObject[] playerList, GameObject p)
    {
        player.GetComponent<PlayerMove>().balloon = null;
        player = null;//風船を他のプレイヤーに回すためにnullにする

        do
        {
            player = playerList[Random.Range(0, playerList.Length)];//配列内からランダムでプレイヤーを指定
        } while (player == p);
        player.GetComponent<PlayerMove>().balloon = transform.gameObject;
    }

    /// <summary>
    /// 爆破物移動処理(一番遠い人)
    /// </summary>
    public GameObject BalloonExChangeByDistance(GameObject[] playerList, GameObject p)
    {
        //風船持つ人との距離
        float p2b_Distance = 0;
        GameObject farthestPlayer = null;
        for (int i = 0; i < playerList.Length; i++)
        {
            float distance = 0;
            if (isCameraDistance)
            {
                //画面距離
                distance = Vector3.Distance(Camera.main.WorldToScreenPoint(playerList[i].transform.position),
                                                  Camera.main.WorldToScreenPoint(p.transform.position));
            }
            else
            {
                //実距離
                distance = Vector3.Distance(playerList[i].transform.position, p.transform.position);
            }

            //一番遠い人
            if (distance > p2b_Distance)
            {
                p2b_Distance = distance;
                farthestPlayer = playerList[i];
            }
        }

        return farthestPlayer;        
    }

    /// <summary>
    /// 爆発物拡大処理
    /// </summary>
    public void BalloonBlast()
    {
        blastCount++;
        scaleCount += scaleRate;
        isDestroy = false;//破棄できないようにする
        //内容物の数が限界を超えたら
        if (blastCount >= blastLimit)
        {
            BlastAction();
        }
    }
    /// <summary>
    /// 爆発物拡大処理
    /// </summary>
    public void BalloonBlast(float air)
    {
        blastCount+=air;
        scaleCount += scaleRate*air;
        isDestroy = false;//破棄できないようにする
        //内容物の数が限界を超えたら
        if (blastCount >= blastLimit)
        {
            BlastAction();
        }
    }

    /// <summary>
    /// 爆発物拡大処理(中心物体用)
    /// </summary>
    /// <param name="post">中心物体</param>
    public void BalloonBlast(GameObject post)
    {
        blastCount++;
        scaleCount += scaleRate;
        isDestroy = false;//破棄できないようにする
        //内容物の数が限界を超えたら
        if (blastCount >= blastLimit)
        {
            post.GetComponent<PostController>().blastCount = 0;//次の爆弾へ超過しないように
            BlastAction();
        }
    }

    /// <summary>
    /// 風船色変更
    /// </summary>
    /// <returns></returns>
    bool CheckColorChange()
    {
        preState = curState;
        curState = _balloonState;

        if (_isBlast) //爆発した時はfalse
        {
            return false;
        }
        else
        {
            if (preState != curState)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 爆発時処理メソッド
    /// </summary>
    public virtual void BlastAction()
    {
        //ポーズ対象から外す
        //Pauser.targetsRemove(GetComponent<Pauser>());
        _isBlast = true;//爆発した
        //ルーレットフラグをtrueに
        //balloonMaster.isRoulette = true;
        balloonMaster.IsBlast = true;//爆発した
        player.GetComponent<PlayerMove>().isStan = true;
        player.GetComponent<PlayerMove>().isBlastStan = true;
        //スタン時間更新
        player.GetComponent<PlayerMove>().stanTime = player.GetComponent<PlayerMove>().originStanTime;
        GetComponent<AudioSource>().PlayOneShot(soundSE2);
        //次のプレイヤー指定
        balloonMaster.nextPlayer = BalloonExChangeByDistance(balloonMaster.pList, player);
        isDestroy = true;//破棄できるようにする
        if (detonationArea != null)//誘爆半径削除
            Destroy(detonationArea);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// アイテム飛び散りメソッド
    /// </summary>
    /// <param name="player">プレイヤー</param>
    /// <param name="itemRate">飛び散り割合</param>
    /// <param name="isTotal">トータルか所持からか</param>
    public virtual void ItemBlast(GameObject player,float itemRate,bool isTotal)
    {
        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        playerMove.isStan = true;
        //排出ポイント割合
        List<string> itemList = isTotal ? playerMove.totalItemList : playerMove.itemList;

        float itemCount = isTotal ? playerMove.totalItemCount : playerMove.holdItemCount;

        int itemRatio = (int)(itemCount *(itemRate/10));

        if(isTotal)
            playerMove.totalItemCount -= itemRatio;
        else
            playerMove.holdItemCount -= itemRatio;
        //排出ポイント割合が0になるまで排出
        while (itemRatio > 0)
        {
            //排出アイテム設定
            GameObject outItem = playerMove.originItem;

            //2ポイント以下なら別設定
            if (itemRatio <= 2)
            {
                GameObject spawnItem;//排出させたアイテム
                int TwoOrOne = Random.Range(0, 2);

                //1/2の確率で、排出ポイント割合が2で、2ポイントアイテムをもっていたら
                if (TwoOrOne == 0 && itemRatio == 2 && itemList.Contains("2CoinPointItem(Clone)"))
                {
                    itemRatio -= 2;//排出ポイント割合2ポイント減
                    outItem = playerMove.originHighItem;//2ポイントアイテム排出
                    spawnItem = Instantiate(outItem, player.transform.position + new Vector3(0, outItem.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                    spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                    spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                    itemList.Remove("2CoinPointItem(Clone)");//2ポイントアイテム削除
                    break;
                }
                itemRatio--;//排出ポイント割合ポイント減
                outItem = playerMove.originItem;//ポイントアイテム排出
                spawnItem = Instantiate(outItem, player.transform.position + new Vector3(0, outItem.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
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
                        itemRatio--;//排出ポイント割合ポイント減
                        outItem = playerMove.originItem;//ポイントアイテム排出
                        break;
                    case "2CoinPointItem(Clone)"://高ポイントアイテム
                        itemRatio -= 2;//排出ポイント割合2ポイント減
                        outItem = playerMove.originHighItem;//2ポイントアイテム排出
                        break;
                }
                GameObject spawnItem = Instantiate(outItem, player.transform.position + new Vector3(0, outItem.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                itemList.RemoveAt(itemNum);//排出アイテム削除
            }
        }
    }
}
