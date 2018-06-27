//
//作成日時：4月26日
//中心物体移動処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostRespawn : MonoBehaviour {
    
	public GameObject originPost;//中心物体
	public float postCount;//中心物体数
	List<GameObject> childList;//移動先リスト
    
    public bool isLimitReset = false;//アイテム生成上限リセット判定用
    
    public float originRespawnTime = 5.0f;//生成間隔
    List<GameObject> isPostList;//ポストがある生成位置リスト

    public GameObject origin_Post_Target_Particle;

    [HideInInspector]
    public bool isBalloon;

    TimeController timeController;//時間管理クラス

    // Use this for initialization
    void Start ()
    {
        transform.tag = "Pausable";//タグ設定
        //初期化処理
		childList = new List<GameObject>();
        isPostList = new List<GameObject>();
        isLimitReset = false;

		//移動先リスト追加
		for (int i = 0; i < transform.childCount; i++)
		{
            GameObject child = transform.GetChild(i).gameObject;
            childList.Add(child);
            //コンポーネント追加
            child.AddComponent<PostSet>().StartSet(originRespawnTime,originPost, origin_Post_Target_Particle);           
		}

        ////中心物体生成
        for (int i = 0; i < postCount; i++)
        {
            PostRespawnSet();
        }

        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
    }

    // Update is called once per frame
    void Update()
    {
        isLimitReset = false; //アイテム生成上限リセット判定をリセット

        //バルーンが生成されるタイミングで生成できるようにする
        //ロスタイムはいつでも生成できる
        if (!isBalloon&&timeController.timeState != TimeState.LOSSTIME)
            return;

        //生成確認用bool
        float balloons = 0;

        //ポストがいなければ
        for (int i = 0; i < isPostList.Count; i++)
        {
            if (!isPostList[i].GetComponent<PostSet>().isRespawn)
            {
                isPostList.RemoveAt(i);
                //生成許可
                PostRespawnSet();
            }
            else
            {
                balloons++;
            }
        }

        //2箇所とも生成されていたら
        if(balloons >= 2)
        {
            //次のバルーン生成タイミングまで生成しない
            isBalloon = false;
        }
    }

    /// <summary>
    /// ポスト生成許可メソッド
    /// </summary>
    public void PostRespawnSet()
    {
        int rand;
        //ランダムで選ばれたchildListに子があったら選びなおす
        do
        {
            rand = Random.Range(0, childList.Count);
        } while (childList[rand].GetComponent<PostSet>().isRespawn == true);

        //ポスト生成可能にする
        childList[rand].GetComponent<PostSet>().isRespawn = true;
        isPostList.Add(childList[rand]);
        //ポストの再生成に合わせてアイテムの生成上限をリセットする
        isLimitReset = true;
    }

    public bool isLimit() {
        return isLimitReset;
    }
}
