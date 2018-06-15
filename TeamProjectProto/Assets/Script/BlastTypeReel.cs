//
//作成日時：6月12日
//爆破タイプ決定リール
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastTypeReel : ReelSpin
{
    public List<bool> blastTypeList;//爆破タイプリスト

    public override T ReelValue<T>()
    {
        //決まった爆破タイプ返す
        return (T)(object)blastTypeList[spriteObjList.IndexOf(centerSprite)];
    }
}
