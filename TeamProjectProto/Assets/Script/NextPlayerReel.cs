//
//作成日時：6月12日
//次のプレイヤーを決めるリール
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextPlayerReel : ReelSpin
{
    public GameObject Roulette;//ルーレット本体
    List<GameObject> nextPlayerList;//次のプレイヤー候補リスト

    void Start()
    {
        //次のプレイヤー候補を設定
        nextPlayerList = Roulette.GetComponent<RouletteController>().playerList;
        //回すプレイヤー以外でリールを作成
        MakeSpriteObjList((int)Roulette.GetComponent<RouletteController>().playerIndex);
    }

    public override T ReelValue<T>()
    {
        //次のプレイヤーを返す
        return (T)(object)nextPlayerList[spriteObjList.IndexOf(centerSprite)];
    }
}
