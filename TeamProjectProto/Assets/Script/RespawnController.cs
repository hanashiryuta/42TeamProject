//
//作成日時：4月16日
//プレイヤーのリスポーン処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnController : MonoBehaviour {

    public GameObject player;//プレイヤー
    List<GameObject> playerList;//プレイヤーリスト
    List<Color> colorList;//カラーリスト
    [SerializeField]
    List<Texture> texList;//テクスチャリスト

    public GameObject shadow;//影のオブジェクト

	// Use this for initialization
	void Awake () {
        //初期化処理
        playerList = new List<GameObject>();
        colorList = new List<Color>()
        {
            new Color(204 / 255f, 0 / 255f, 0 / 255f),//赤
            new Color(15 / 255f, 82 / 255f, 188 / 255f),//青
            new Color(255 / 255f, 255 / 255f, 20 / 255f),//黄色
            new Color(0 / 255f, 255 / 255f, 65 / 255f)//緑
        };

        //生成位置を数だけプレイヤーを生成する
        for (int i = 0; i < transform.childCount; i++) 
        {
            GameObject p = Instantiate(player, transform.GetChild(i).transform.position,Quaternion.identity);//プレイヤー生成
            //p.transform.localRotation = Quaternion.Euler(0, 45, 0);
            playerList.Add(p);//リストに追加
            p.name = "Player" + (i + 1);//名前変更
            p.GetComponent<PlayerMove>().horizontal = "Horizontal" + (i + 1);//そのプレイヤーの使うInput指定
            p.GetComponent<PlayerMove>().vertical = "Vertical" + (i + 1);//そのプレイヤーの使うInput指定
            p.GetComponent<PlayerMove>().jump = "Jump" + (i + 1);//そのプレイヤーの使うInput指定

            //180518 何　追加
            p.GetComponentInChildren<SkinnedMeshRenderer>().materials[0].mainTexture = texList[i];//テクスチャ変更

            //180508 何　追加　アウトラインの色
            p.GetComponentInChildren<Outline>().color = i;

            //影をPlayerの子にして生成
            GameObject s = Instantiate(shadow, p.transform.position - Vector3.down, Quaternion.identity, p.transform);
        }
	}
}
