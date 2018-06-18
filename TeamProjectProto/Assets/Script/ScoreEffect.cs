using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoreEffect : MonoBehaviour {
    RectTransform effect; //ScoreEffect
    RectTransform targetUI; //ポイントを入れたプレイヤーのScoreUIの位置
    [HideInInspector]
    public string playerName; //ポイントを入れたプレイヤーの名前

    bool isCreat = false;

	// Use this for initialization
	void Start () {
        effect = GetComponent<RectTransform>();
        print(playerName);

        targetUI = GameObject.Find(playerName + "UI").GetComponent<RectTransform>(); //取得したプレイヤー名のUIを見つける
    }

    // Update is called once per frame
    void Update ()
    {
        targetUI = GameObject.Find(playerName + "UI").GetComponent<RectTransform>(); //取得したプレイヤー名のUIを見つける

        if(isCreat == false)
        {
            //PlayerUIの座標までもっていく
            DOTween.To(
                () => effect.anchoredPosition,
                pos => effect.anchoredPosition = pos,
                targetUI.anchoredPosition - new Vector2(0, targetUI.rect.height / 2),
                1.5f);

            isCreat = true;
        }

        //UIの位置（UIの下端）まで行ったら
        if (effect.anchoredPosition.y >= 360 - (targetUI.rect.height + 2))
        {
            Debug.Log(effect.anchoredPosition.y);
            Destroy(gameObject); //破棄する
        }
	}
}
