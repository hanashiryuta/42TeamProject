//
//作成日時：7月13日
//カットイン生成クラス
//作成者：安部崇寛
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutinSpawn : MonoBehaviour {

    public GameObject CutinObject;
    GameObject Cutin;

    //Cutinを生成
    public void cutinSpawn() {
        Cutin = Instantiate(CutinObject, transform.position + new Vector3(1280f, 0, 0), Quaternion.Euler(0, 0, 0), transform);
    }
}
