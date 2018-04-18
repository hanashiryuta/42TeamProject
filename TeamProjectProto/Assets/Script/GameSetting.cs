//
//作成日時：4月18日
//ゲーム開始時のセット処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSetting : MonoBehaviour {

    public List<GameObject> gameObjectList;//ゲーム開始時配置するオブジェクト

	// Use this for initialization
	void Start () {
        
        for(int i = 0;i<gameObjectList.Count;i++)
        {
            Instantiate(gameObjectList[i]);//生成
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
