//
//作成日時：6月8日
//所持アイテム飛び散りバルーン
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalBalloon : BalloonOrigin {

    public override void BlastAction()
    {
        PlayerMove playerMove = player.GetComponent<PlayerMove>();
        //排出ポイント割合
        int itemRatio = (int)(playerMove.holdItemCount/2);
        //排出ポイント割合が0になるまで排出
        while (itemRatio > 0)
        {
            //排出アイテム設定
            GameObject item = playerMove.originItem;

            //2ポイント以下なら別設定
            if (itemRatio <= 2)
            {
                GameObject spawnItem;//排出させたアイテム
                int TwoOrOne = Random.Range(0, 2);

                //1/2の確率で、排出ポイント割合が2で、2ポイントアイテムをもっていたら
                if (TwoOrOne == 0 && itemRatio == 2 && playerMove.itemList.Contains("2CoinPointItem(Clone)"))
                {
                    playerMove.holdItemCount -= 2;//2ポイント減
                    itemRatio -= 2;//排出ポイント割合2ポイント減
                    item = playerMove.originHighItem;//2ポイントアイテム排出
                    spawnItem = Instantiate(item, player.transform.position + new Vector3(0, item.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                    spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                    spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                    playerMove.itemList.Remove("2CoinPointItem(Clone)");//2ポイントアイテム削除
                    break;
                }
                playerMove.holdItemCount--;//ポイント減
                itemRatio--;//排出ポイント割合ポイント減
                item = playerMove.originItem;//ポイントアイテム排出
                spawnItem = Instantiate(item, player.transform.position + new Vector3(0, item.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                playerMove.itemList.Remove("1CoinPointItem(Clone)");//ポイントアイテム削除
            }
            //それ以外はランダム
            else
            {
                int itemNum = Random.Range(0, playerMove.itemList.Count);//ランダム設定
                switch (playerMove.itemList[itemNum])//取得したアイテムからランダムで選出
                {
                    case "1CoinPointItem(Clone)"://普通のアイテム
                        playerMove.holdItemCount--;//ポイント減
                        itemRatio--;//排出ポイント割合ポイント減
                        item = playerMove.originItem;//ポイントアイテム排出
                        break;
                    case "2CoinPointItem(Clone)"://高ポイントアイテム
                        playerMove.holdItemCount -= 2;//2ポイント減
                        itemRatio -= 2;//排出ポイント割合2ポイント減
                        item = playerMove.originHighItem;//2ポイントアイテム排出
                        break;
                }
                GameObject spawnItem = Instantiate(item, player.transform.position + new Vector3(0, item.transform.localScale.y + 3, 0), Quaternion.Euler(90, 0, 0));//生成
                spawnItem.GetComponent<ItemController>().SetMovePosition();//移動設定
                spawnItem.GetComponent<ItemController>().isGet = false;//取れない設定
                playerMove.itemList.RemoveAt(itemNum);//排出アイテム削除
            }
        }
        base.BlastAction();
    }
}
