//
//作成日時：4月17日
//爆発物(風船)の処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爆発物の状態
/// </summary>
enum BalloonState
{
    SAFETY,//安全
    CAUTION,//注意
    DANGER//危険
}

public class BalloonController : MonoBehaviour {
    
    [HideInInspector]
    public GameObject player;//プレイヤー
    
    [HideInInspector]
    public float scaleCount = 1;//現在の大きさ
    public float scaleLimit = 3;//大きさ限界
    float scaleRate = 0;//拡大率

    float blastCount = 0;//内容物総数
    public float blastLimit = 30;//内容物総数限界

    float moveTime = 1.0f;//爆発物が移る際のインターバル
    bool isMove = true;//爆発物が移れるかどうか

    [HideInInspector]
    public bool isEnd = false;//ゲームが終わるかどうか

    BalloonState balloonState;//爆発物の状態

	// Use this for initialization
	void Start () {
        //初期化処理
        scaleCount = 1;
        moveTime = 1.0f;
        isMove = true;
        isEnd = false;
        balloonState = BalloonState.SAFETY;
        scaleRate = scaleLimit / blastLimit;
        blastCount = 0;

        player = GameObject.Find("Player" + Random.Range(1, 5));//プレイヤーをランダムで指定
        player.GetComponent<PlayerMove>().balloon = transform.gameObject;//プレイヤー内に自分を指定
    }
	
	// Update is called once per frame
	void Update () {
        //裏コマンド
        if(Input.GetKeyDown(KeyCode.A))
        {
            scaleCount+=0.1f;
        }

        //プレイヤーについていなければ
        if (player == null)
        {
            GameObject[] pList = GameObject.FindGameObjectsWithTag("Player");//プレイヤー配列を作成

            //プレイヤーが一人しかいなければゲームを終了する
            if (pList.Length <= 1)
            {
                isEnd = true;
                return;
            }

            BalloonExChange(pList);           
        }

        //内容物の数が限界を超えたら
        if(blastCount>blastLimit)
        {
            Destroy(player);//プレイヤーを破棄
            scaleCount = 1.0f;
            blastCount = 0;
        }

        transform.localScale = new Vector3(scaleCount, scaleCount, scaleCount);//内容物の数により大きさ変更        

        //常にプレイヤーの上にいるようにする
        if (player != null)
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 1 + transform.localScale.y / 2, player.transform.position.z);

        //一度移ってから再度移るまで1秒のインターバルが存在する
        if(!isMove)
        {
            moveTime -= Time.deltaTime;
            if (moveTime < 0)
            {
                moveTime = 1.0f;
                isMove = true;
            }
        }

        ColorChange();//色変更
    }

    /// <summary>
    /// 色変更
    /// </summary>
    void ColorChange()
    {
        if (blastCount < 10) //1.7以下なら安全状態
            balloonState = BalloonState.SAFETY;
        
        switch(balloonState)
        {
            //1.7以下なら安全状態
            //色は緑
            case BalloonState.SAFETY:
                gameObject.GetComponent<Renderer>().material.color = new Color(0 / 255f, 255 / 255f, 51 / 255f);
                if (blastCount > 10)
                    balloonState = BalloonState.CAUTION;
                break;

            //1.7以上なら注意状態
            //色は黄色
            case BalloonState.CAUTION:
                gameObject.GetComponent<Renderer>().material.color = new Color(255 / 255f, 255 / 255f, 51 / 255f);
                if (blastCount > 20)
                    balloonState = BalloonState.DANGER;
                break;

            //2.4以上なら危険状態
            //色は赤
            case BalloonState.DANGER:
                gameObject.GetComponent<Renderer>().material.color = new Color(255 / 255f, 0 / 255f, 0 / 255f);
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
            isMove = false;
        }

    }

    /// <summary>
    /// 爆破物ランダム移動処理
    /// </summary>
    /// <param name="playerList"></param>
    public void BalloonExChange(GameObject[] playerList)
    {       
        player = playerList[Random.Range(0, playerList.Length - 1)];//配列内からランダムでプレイヤーを指定
        player.GetComponent<PlayerMove>().balloon = transform.gameObject;
    }

    /// <summary>
    /// 爆破物ランダム移動処理(1人を除く)
    /// </summary>
    /// <param name="playerList"></param>
    public void BalloonExChange(GameObject[] playerList,GameObject p)
    {
        do
        {
            player = playerList[Random.Range(0, playerList.Length - 1)];//配列内からランダムでプレイヤーを指定
        } while (player == p);
        player.GetComponent<PlayerMove>().balloon = transform.gameObject;
    }

    /// <summary>
    /// 爆発物拡大処理
    /// </summary>
    public void BalloonBlast( )
    {
        blastCount ++;
        scaleCount += scaleRate;
    }
}
