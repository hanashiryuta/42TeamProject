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
                reel.AddComponent<Image>().sprite = spriteList[(int)nextPlayerList[0].GetComponent<PlayerMove>().playerIndex];
                //位置設定
                reel.GetComponent<RectTransform>().localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * i, 0);
            }

        }
        else {
            //画像分リール生成
            foreach (var player in nextPlayerList)
            {
                //生成
                GameObject reel = new GameObject(reelName + nextPlayerList.IndexOf(player));
                //リスト追加
                spriteObjList.Add(reel);
                //子に設定
                reel.transform.parent = transform;
                //画像設定
                reel.AddComponent<Image>().sprite = spriteList[(int)player.GetComponent<PlayerMove>().playerIndex];
                //位置設定
                reel.GetComponent<RectTransform>().localPosition = new Vector3(0, GetComponent<RectTransform>().sizeDelta.y * nextPlayerList.IndexOf(player), 0);
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
        return (T)(object)nextPlayerList[spriteObjList.IndexOf(centerSprite)];
    }
}
