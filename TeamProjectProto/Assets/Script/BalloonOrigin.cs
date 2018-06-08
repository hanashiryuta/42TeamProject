//
//作成日時：6月8日
//バルーン基底クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure; //コントローラー振動用

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

    float moveTime = 3.0f;//爆発物が移る際のインターバル
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

    StartCountDown startCntDown;//カウントダウンScript
    FinishCall finishCall;//終了合図Script

    public bool isCameraDistance = false;

    [HideInInspector]
    public BalloonMaster balloonMaster;//バルーン総合管理クラス

    //各状態の色
    public Color safeColor;
    public Color coutionColor;
    public Color dangerColor;

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
    }

    // Update is called once per frame
    void Update()
    {
        //裏コマンド
        if (Input.GetKeyDown(KeyCode.A))
        {
            BalloonBlast();
        }

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

        transform.localScale = new Vector3(scaleCount, scaleCount, scaleCount);//内容物の数により大きさ変更    

        //一度移ってから再度移るまで3秒のインターバルが存在する
        if (!isMove)
        {
            moveTime -= Time.deltaTime;
            if (moveTime < 0)
            {
                moveTime = 3.0f;
                isMove = true;
            }
        }
        
        ColorChange();//色変更

        _isColorChaged = CheckColorChange();
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
        if (blastCount < 10) //1.7以下なら安全状態
            _balloonState = BalloonState.SAFETY;

        switch (_balloonState)
        {
            //1.7以下なら安全状態
            //色は緑
            case BalloonState.SAFETY:
                //gameObject.GetComponent<Renderer>().material.color = new Color(34 / 255f, 195 / 255f, 80 / 255f);
                gameObject.GetComponent<Renderer>().material.color = safeColor;
                if (blastCount > 10)
                    _balloonState = BalloonState.CAUTION;
                break;

            //1.7以上なら注意状態
            //色は黄色
            case BalloonState.CAUTION:
                //gameObject.GetComponent<Renderer>().material.color = new Color(255 / 255f, 241 / 255f, 15 / 255f);
                gameObject.GetComponent<Renderer>().material.color = coutionColor;
                if (blastCount > 20)
                    _balloonState = BalloonState.DANGER;
                break;

            //2.4以上なら危険状態
            //色は赤
            case BalloonState.DANGER:
                //gameObject.GetComponent<Renderer>().material.color = new Color(229 / 255f, 0 / 255f, 11 / 255f);
                gameObject.GetComponent<Renderer>().material.color = dangerColor;
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
            player2.GetComponent<PlayerMove>().balloon = transform.gameObject;//移動先に自信を指定
            player = player2;
            balloonMaster.nowPlayer = player2;
            isMove = false;
            player2.gameObject.GetComponent<PlayerMove>().isStan = true;
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
        _isBlast = true;//爆発した
        player.GetComponent<PlayerMove>().isStan = true;
        GetComponent<AudioSource>().PlayOneShot(soundSE2);
        //次のプレイヤー指定
        balloonMaster.nextPlayer = BalloonExChangeByDistance(balloonMaster.pList, player);
        isDestroy = true;//破棄できるようにする
        Camera.main.GetComponent<CameraShake>().Shake();

        Destroy(this.gameObject);
    }
}
