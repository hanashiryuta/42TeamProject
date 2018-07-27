//
//作成日時：7月13日
//カットインの移動・切り替えクラス
//作成者：安部崇寛
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class CutinController : MonoBehaviour {
    
    RectTransform rectTran;
    
    [SerializeField]
    List<Sprite> cutImageTex;
    
    BalloonMaster balloonMaster;

    public float time = 0.5f;
    bool isTime = false;

    Image playerImage;
    
    void Start()　{
        //カットインのプレイヤーイメージを入れ替えるImageを取得
        playerImage = transform.GetChild(4).GetComponent<Image>();
        //カットインのプレイヤー画像を変えるためにPlayerを取得
        balloonMaster = GameObject.Find("BalloonMaster(Clone)").GetComponent<BalloonMaster>();
        //カットインのプレイヤー画像を変更
        Cutin_SpriteImageChange(balloonMaster.nowPlayer.name);
        //画面内へカットインを移動
        StartCoroutine(CutinCoroutine());
    }
    
    void Update() {
        //画面中央に来た時に一定時間停止させる
        if(transform.position.x <= 640 && !isTime) {
            time -= Time.deltaTime;
            if(time < 0) {
                isTime = true;
            }
        }

        //停止時間終了後画面外へ移動
        if(isTime) {
            StartCoroutine(CutoutCoroutine());
        }
    }

    //画面外から画面内へカットインを移動
    IEnumerator CutinCoroutine()
    {
        DOTween.To(() => transform.position, x => transform.position = x, new Vector3(640, 360, 0), 0.25f);
        yield return new WaitForSeconds(0.25f);
    }

    //画面内から画面外へカットインを移動
    IEnumerator CutoutCoroutine()
    {
        DOTween.To(() => transform.position, x => transform.position = x, new Vector3(-640, 360, 0), 0.25f);
        yield return new WaitForSeconds(0.25f);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// カットインのプレイヤー画像が風船の持ち主の顔に変化
    /// </summary>
    public void Cutin_SpriteImageChange(string playerName)
    {
        switch (playerName)
        {
            case "Player1":
                playerImage.sprite = cutImageTex[0];
                break;
            case "Player2":
                playerImage.sprite = cutImageTex[1];
                break;
            case "Player3":
                playerImage.sprite = cutImageTex[2];
                break;
            case "Player4":
                playerImage.sprite = cutImageTex[3];
                break;
        }
    }
}
