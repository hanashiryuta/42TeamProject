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

    public List<GameObject> haveItemCounts;//表示テキストリスト
    RouletteController rouletteController;//ルーレット管理クラス

    bool isFirstTimeTextSeted = false;//最初のテキスト設定終わったか

    void Start()
    {
        
    }
    // Update is called once per frame
    void Update ()
    {
        if (isFirstTimeTextSeted)
        {
            //ルーレットが入場状態なら
            if (rouletteController.rouletteState == RouletteState.ENTRY)
            {
                //出てくる
                GetComponent<RectTransform>().DOLocalMoveY(-310, 1);
            }
            //ルーレットが退場状態なら
            else if (rouletteController.rouletteState == RouletteState.EXIT)
            {
                //引っ込む
                GetComponent<RectTransform>().DOLocalMoveY(-410, 1);
            }
        }

    }
    /// <summary>
    /// 初期化クラス
    /// </summary>
    /// <param name="pList">プレイヤーリスト</param>
    /// <param name="rouletteController">ルーレット管理クラス</param>
    public void RouletteStart(GameObject[] pList,RouletteController rouletteController)
    {
        ////一番奥に表示する
        //transform.SetAsFirstSibling();
        //ダッシュゲージより前に表示する
        transform.SetSiblingIndex(6);
        //取得
        this.rouletteController = rouletteController;
        //存在しているプレイヤーのインデックス格納
        List<int> existPlayerIndexList = new List<int>();
        //テキスト設定
        for (int i = 0; i < pList.Length; i++)
        {
            //存在しているプレイヤーのインデックス取出し
            int existPlayerIndex = (int)pList[i].GetComponent<PlayerMove>().playerIndex;
            //格納
            existPlayerIndexList.Add(existPlayerIndex);
            //テキスト設定
            haveItemCounts[existPlayerIndex].transform.
                Find("Player" + (existPlayerIndex + 1) + "HaveCount").
                GetComponent<Text>().text = HalfWidth2FullWidth.Set2FullWidth(pList[i].GetComponent<PlayerMove>().holdItemCount);
        }

        int tmpIndex = 0;//一時用インデックス
        //いないプレイヤー探し
        foreach(var h in haveItemCounts)
        {
            //存在しているプレイヤーのインデックスリストになければ
            if (!existPlayerIndexList.Contains(tmpIndex))
            {
                //非アクティブ化
                h.SetActive(false);
            }
            tmpIndex++;
        }

        //最初のテキスト設定終わった
        isFirstTimeTextSeted = true;
    }
}
