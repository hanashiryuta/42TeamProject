//
//作成日時：6月12日
//爆破時間設定リール
//作成者：葉梨竜太
//
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeReel : ReelSpin {

    public List<float> timeList;//爆破時間リスト

    public override T ReelValue<T>()
    {
        //決まった爆破時間を返す
        return (T)(object)timeList[spriteList.IndexOf(centerSprite.GetComponent<Image>().sprite)];
    }    
}
