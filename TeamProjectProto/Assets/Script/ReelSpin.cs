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
    public bool isEnd;//リールが止められたかどうか
    
    public List<Sprite> spriteList;//リール画像リスト

    [HideInInspector]
    public List<GameObject> spriteObjList;//リール画像オブジェクトリスト

    [HideInInspector]
    public GameObject centerSprite;//中心画像
    Vector3 centerPosition;//中心位置

    public string reelName;//リール種類名

    public float reelSpeed = 1;//リール回転速度
    
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
            //画像オブジェクトリスト空1つずつ
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

        //画像分リール生成
        foreach (var sprite in spriteList)
        {
            //生成
            GameObject reel = new GameObject(reelName + spriteList.IndexOf(sprite));
            //リスト追加
            spriteObjList.Add(reel);
            //子に設定
            reel.transform.parent = transform;
            //画像設定
            reel.AddComponent<Image>().sprite = sprite;
            //位置設定
            reel.GetComponent<RectTransform>().localPosition =  new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * spriteList.IndexOf(sprite), 0);
        }
        //初期中心画像設定
        centerSprite = spriteObjList[0];
        //中心位置設定
        centerPosition = Vector3.zero;
    }

    /// <summary>
    /// 画像オブジェクトリスト作成メソッド（一つ除外）
    /// </summary>
    /// <param name="Excusion"></param>
    public void MakeSpriteObjList(int Excusion)
    {
        //初期化処理
        spriteObjList = new List<GameObject>();
        //一つ分除外
        spriteList.RemoveAt(Excusion);
        //画像分リール生成
        foreach (var sprite in spriteList)
        {
            //生成
            GameObject reel = new GameObject(reelName + spriteList.IndexOf(sprite));
            //リスト追加
            spriteObjList.Add(reel);
            //子に設定
            reel.transform.parent = transform;
            //画像設定
            reel.AddComponent<Image>().sprite = sprite;
            //位置設定
            reel.GetComponent<RectTransform>().localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * spriteList.IndexOf(sprite), 0);
        }
        //初期中心画像設定
        centerSprite = spriteObjList[0];
        //中心位置設定
        centerPosition = Vector3.zero;
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
