//
//作成日時：6月19日
//現在所持数表示クラス
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HaveItemCounts : MonoBehaviour {

    public List<Text> haveItemCounts;//表示テキストリスト
    RouletteController rouletteController;//ルーレット管理クラス
    	
	// Update is called once per frame
	void Update () {

        //ルーレットが入場状態なら
        if (rouletteController.rouletteState == RouletteState.ENTRY)
        {
            //出てくる
            GetComponent<RectTransform>().DOLocalMoveY(200, 1);
        }
        //ルーレットが退場状態なら
        else if(rouletteController.rouletteState == RouletteState.EXIT)
        {
            //引っ込む
            GetComponent<RectTransform>().DOLocalMoveY(275, 1);
        }

	}
    /// <summary>
    /// 初期化クラス
    /// </summary>
    /// <param name="pList">プレイヤーリスト</param>
    /// <param name="rouletteController">ルーレット管理クラス</param>
    public void RouletteStart(GameObject[] pList,RouletteController rouletteController)
    {
        //一番奥に表示する
        transform.SetAsFirstSibling();
        //取得
        this.rouletteController = rouletteController;
        //テキスト設定
        for (int i = 0; i < pList.Length; i++)
        {
            haveItemCounts[i].text = HalfWidth2FullWidth.Set2FullWidth(pList[i].GetComponent<PlayerMove>().holdItemCount);
        }
    }
}
