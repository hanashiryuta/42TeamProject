using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScoreEffect : MonoBehaviour {
    RectTransform effect; //ScoreEffect
    RectTransform targetUI; //ポイントを入れたプレイヤーのScoreUIの位置
    [HideInInspector]
    public string playerName; //ポイントを入れたプレイヤーの名前

	// Use this for initialization
	void Start () {
        effect = GetComponent<RectTransform>();
        print(playerName);
    }
	
	// Update is called once per frame
	void Update ()
    {
        targetUI = GameObject.Find(playerName + "UI").GetComponent<RectTransform>(); //取得したプレイヤー名のUIを見つける

        //PlayerUIの座標までもっていく
        DOTween.To(
            () => effect.anchoredPosition,
            pos => effect.anchoredPosition = pos,
            targetUI.anchoredPosition + new Vector2(200, 0),
            1.5f);

        //UIの位置（UIの右端）まで行ったら
        if (effect.anchoredPosition.x < 226)
        {
            Destroy(gameObject); //破棄する
        }
	}
}
