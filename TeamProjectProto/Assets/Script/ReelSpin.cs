//
//作成日時：6月12日
//リール基底クラス
//作成者：葉梨竜太
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;
using DG.Tweening;

public abstract class ReelSpin : MonoBehaviour {

    [HideInInspector]
    public bool isSpin;//スピンしているかどうか
    [HideInInspector]
    public bool isEnd;//リールが止められたかどうか
    
    public List<Sprite> spriteList;//リール画像リスト

    [HideInInspector]
    public List<GameObject> spriteObjList;//リール画像オブジェクトリスト

    [HideInInspector]
    public GameObject centerSprite;//中心画像
    [HideInInspector]
    public Vector3 centerPosition;//中心位置

    public string reelName;//リール種類名

    public float reelSpeed = 1;//リール回転速度
    
    public static readonly float reelCount = 10;//各リール要素数

    [HideInInspector]
    public List<Sprite> reelSpriteList;//リール画像リスト(割合配置後)

    [HideInInspector]
    public List<float> reelRateCountList;//リール各要素割合
    
    // Use this for initialization
    void Start () {
        //初期処理
        MakeSpriteObjList();
    }
	
	// Update is called once per frame
	void Update () {
        //スピン中かつ止められていない
        if (isSpin&&!isEnd)
        {
            //画像オブジェクトリストから1つずつ
            foreach (var sprite in spriteObjList)
            {
                //回転処理
                sprite.GetComponent<RectTransform>().localPosition += new Vector3(0, -reelSpeed, 0);
                if (sprite.GetComponent<RectTransform>().localPosition.y < - sprite.GetComponent<RectTransform>().sizeDelta.y)
                {
                    //下まで行ったら上に戻る
                    sprite.GetComponent<RectTransform>().localPosition =  new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * (spriteObjList.Count - 1), 0);
                }

                //中心画像より、今の画像のほうが中心位置に近かったら
                if (Vector3.Distance(centerPosition, centerSprite.GetComponent<RectTransform>().localPosition) > Vector3.Distance(centerPosition, sprite.GetComponent<RectTransform>().localPosition))
                {
                //中心リール更新処理
                    centerSprite = sprite;
                }
            }
        }

        //リールが止められたら
        if(isEnd)
        {
            //中心とのずれを計算
            Vector3 offset = centerPosition - centerSprite.GetComponent<RectTransform>().localPosition;

            //ずれが0.01以下なら止める
            if (Mathf.Abs(offset.y) <= 0.01f)
                isEnd = false;

            //リールすべてずれ分ずらす
            foreach (var sprite in spriteObjList)
            {
                sprite.GetComponent<RectTransform>().localPosition += offset/3;
            }
        }
    }

    /// <summary>
    /// 画像オブジェクトリスト作成メソッド
    /// </summary>
    void MakeSpriteObjList()
    {
        //初期化処理
        spriteObjList = new List<GameObject>();
        ReelSpriteSet(spriteList);

        for (int i = 0; i < reelCount; i++)
        {
            //生成
            GameObject reel = new GameObject(reelName + (i+1));
            //リスト追加
            spriteObjList.Add(reel);
            //子に設定
            reel.transform.parent = transform;
            //画像設定
            reel.AddComponent<Image>().sprite = SpriteRandomReturn();
            //位置設定
            reel.GetComponent<RectTransform>().localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * i, 0);
        }
        //初期中心画像設定
        centerSprite = spriteObjList[0];
        //中心位置設定
        centerPosition = Vector3.zero;
    }

    /// <summary>
    /// 割合に応じたリール作成メソッド
    /// </summary>
    /// <param name="sList">画像リスト</param>
    public void ReelSpriteSet(List<Sprite> sList)
    {
        reelSpriteList = new List<Sprite>();
        foreach (var sprite in sList)
        {
            //各要素を割合に応じて生成
            for (int i = 0; i < reelCount*(reelRateCountList[sList.IndexOf(sprite)]/10); i++)
            {
                reelSpriteList.Add(sprite);
            }
        }
    }

    /// <summary>
    /// リールランダムに返すメソッド
    /// </summary>
    /// <returns></returns>
    public Sprite SpriteRandomReturn()
    {
        int spriteNum = UnityEngine.Random.Range(0, reelSpriteList.Count-1);
        Sprite sprite = reelSpriteList[spriteNum];
        reelSpriteList.RemoveAt(spriteNum);
        return sprite;
    }
    /// <summary>
    /// リール指定して返すメソッド
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Sprite SpriteRandomReturn(int index)
    {
        Sprite sprite = reelSpriteList[index];
        return sprite;
    }

    /// <summary>
    /// リールを止めるメソッド
    /// </summary>
    public virtual void SpinEnd()
    {
        isSpin = false;
        isEnd = true;
    }

    /// <summary>
    /// 止めたリールの要素取得メソッド
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public abstract T ReelValue<T>();
}

/// <summary>
/// リール要素
/// </summary>
/// <typeparam name="T"></typeparam>
public class Value<T>
{
}
