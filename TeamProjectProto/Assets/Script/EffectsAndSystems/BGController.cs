/*
 * 作成日時：180601
 * 背景OBJ制御関連
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BGController : MonoBehaviour
{
    [SerializeField]
    float _moveSpeedX = 0.2f;
    [SerializeField]
    float _moveSpeedY = 0.2f;

    float offsetX = 0;
    float offsetY = 0;

    //BalloonOrigin balloon;
    //GameObject balloonControllerObject;

    [SerializeField]
    Texture[] bgColorTex;

    [SerializeField]
    GameObject panel;
   
    // Use this for initialization
	void Start ()
    {
        Material[] mats = transform.GetComponent<MeshRenderer>().materials;

        //マテリアル設置
        if (SceneManager.GetActiveScene().name == "main")
        {
            mats[0].SetTexture("_MainTex", null);
        }
        else
        {
            mats[0].SetTexture("_MainTex", bgColorTex[4]);//青紫
            Destroy(panel);//Mainシーン以外パネル不表示
            panel = null;
        }

        //画面に合わせて
        transform.localScale = new Vector3(Camera.main.orthographicSize * 2.2f * Screen.width / Screen.height,
                                        Camera.main.orthographicSize * 2.2f,
                                        0.1f);
        if(panel != null)
        {
            panel.transform.localScale = new Vector3(Camera.main.orthographicSize * 2.2f * Screen.width / Screen.height,
                                            Camera.main.orthographicSize * 2.2f,
                                            0.1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.GetComponent<MeshRenderer>().materials.Length != 1)
        {
            BG_SpriteOffsetChange();
        }
    }

    /// <summary>
    /// 背景OBJの画像Offset移動
    /// </summary>
    /// <param name="offsetx"></param>
    /// <param name="offsety"></param>
    void BG_SpriteOffsetChange()
    {
        offsetX += _moveSpeedX * Time.deltaTime;
        offsetY += _moveSpeedY * Time.deltaTime;

        transform.GetComponent<MeshRenderer>().materials[1].SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }

    /// <summary>
    /// 背景OBJの画像色が風船の持ち主の色に変化
    /// </summary>
    public void BG_SpriteColorChange(string playerName)
    {
        switch (playerName)
        {
            case "Player1":
                transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_MainTex", bgColorTex[0]);
                break;
            case "Player2":
                transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_MainTex", bgColorTex[1]);
                break;
            case "Player3":
                transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_MainTex", bgColorTex[2]);
                break;
            case "Player4":
                transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_MainTex", bgColorTex[3]);
                break;
        }
    }

}
