//
//6月28日
//ルール画像表示クラス
//作成者：葉梨竜太
//
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

public class Rule : MonoBehaviour {
    public GameObject ruleImage;//ルール画像表示オブジェクト
    public List<Sprite> ruleSpriteList;//ルール表示画像リスト
    List<GameObject> ruleList;//ルールリスト
    [HideInInspector]
    public bool isRuleEnd;//ルール表示が終了したかどうか
    [HideInInspector]
    public GameObject PageCount;//ページカウント

    float stayTime = 0.0f;//ボタン待機時間
    int imageCount = 0;//表示しているイメージの番号

    // Use this for initialization
    void Start()
    {
        //初期化処理
        ruleList = new List<GameObject>();
        for (int i = 0; i < ruleSpriteList.Count; i++)
        {
            GameObject rule;
            //生成
            rule = Instantiate(ruleImage, transform);
            //位置指定
            rule.GetComponent<RectTransform>().position += new Vector3(i * Screen.width, 0, 0);
            //画像設定
            rule.transform.GetComponent<Image>().sprite = ruleSpriteList[i];
            //リスト追加
            ruleList.Add(rule);
        }
        //ページカウント表示
        PageCount.SetActive(true);
    }

    // Update is called once per frame
    void Update() {
        //ページカウントを表示
        PageCount.GetComponent<Text>().text = HalfWidth2FullWidth.Set2FullWidth((imageCount + 1).ToString("0")) + "／" + HalfWidth2FullWidth.Set2FullWidth(ruleSpriteList.Count.ToString("0"));
        //一番手前に
        PageCount.transform.SetAsLastSibling();
        //時間経過
        stayTime -= Time.deltaTime;
    }

    /// <summary>
    /// 画像移動メソッド
    /// </summary>
    /// <param name="previousState">前フレームコントローラ状態</param>
    /// <param name="currentState">現在のコントローラ状態</param>
    public void Move(GamePadState previousState, GamePadState currentState)
    {
        //Aボタンを押したら
        if (stayTime <= 0 && (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed))
        {
            //時間更新
            stayTime = 1.0f;
            //カウント追加
            imageCount++;
            //画像移動
            foreach (var rule in ruleList)
            {
                rule.GetComponent<RectTransform>().DOMoveX(rule.GetComponent<RectTransform>().position.x - Screen.width, 1.0f);
            }
        }

        //カウントが画像数以上になるか、スタートボタンが押されたら
        if (imageCount >= ruleSpriteList.Count|| (previousState.Buttons.Start == ButtonState.Released &&currentState.Buttons.Start == ButtonState.Pressed))
        {
            //終了処理
            Death();
        }
    }

    /// <summary>
    /// ルール画像終了処理
    /// </summary>
    public void Death()
    {
        //ページ数表示しない
        PageCount.SetActive(false);
        //画像リストクリア
        foreach (var rule in ruleList)
        {
            Destroy(rule);
        }
        ruleList.Clear();
        //フラグ設定
        isRuleEnd = true;
    }
}
