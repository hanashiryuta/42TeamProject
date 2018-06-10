//
//作成日時：6月8日
//Totalアイテム飛び散りバルーン
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TotalBalloon : BalloonOrigin {

    public override void BlastAction()
    {
        ItemBlast(player, 5, true);
        foreach (var cx in detonationList)
        {
            if (cx.gameObject != player)
            {
                ItemBlast(cx.gameObject, 2, true);
            }
        }
        base.BlastAction();
    }
}
