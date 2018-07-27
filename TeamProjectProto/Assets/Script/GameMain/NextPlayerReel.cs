//
//作成日時：6月12日
//次のプレイヤーを決めるリール
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NextPlayerReel : ReelSpin
{
    public GameObject Roulette;//ルーレット本体
    List<GameObject> nextPlayerList;//次のプレイヤー候補リスト
    List<Sprite> playerSpriteList;//プレイヤー画像リスト

    void Start()
    {
        //次のプレイヤー候補を設定
        nextPlayerList = Roulette.GetComponent<RouletteController>().playerList;
        //回すプレイヤー以外でリールを作成
        MakeSpriteObjList();
    }

    /// <summary>
    /// リール生成
    /// </summary>
    void MakeSpriteObjList()
    {
        //初期化処理
        spriteObjList = new List<GameObject>();
        playerSpriteList = new List<Sprite>();

        //画像設定
        foreach(var player in nextPlayerList)
        {
            playerSpriteList.Add(spriteList[(int)player.GetComponent<PlayerMove>().playerIndex]);
        }

        ReelSpriteSet(playerSpriteList);

        if (nextPlayerList.Count <= 1)
        {
            for(int i = 0;i<2;i++)
            {
                //生成
                GameObject reel = new GameObject(reelName + nextPlayerList.IndexOf(nextPlayerList[0]));
                //リスト追加
                spriteObjList.Add(reel);
                //子に設定
                reel.transform.parent = transform;
                //画像設定
                reel.AddComponent<Image>().sprite = SpriteRandomReturn(0);
                //位置設定
                reel.GetComponent<RectTransform>().localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * i, 0);
            }
        }
        else
        {
            for(int i = 0; i < reelCount; i++)
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
        }
        //初期中心画像設定
        centerSprite = spriteObjList[0];
        //中心位置設定
        centerPosition = Vector3.zero;
    }

    public override T ReelValue<T>()
    {
        //プレイヤー1人なら
        if(nextPlayerList.Count <= 1)
            return (T)(object)nextPlayerList[0];
        //次のプレイヤーを返す
        return (T)(object)nextPlayerList[playerSpriteList.IndexOf(centerSprite.GetComponent<Image>().sprite)];
    }
}
