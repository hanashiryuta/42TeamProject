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

    public Material[] bg_Mat;

    float offsetX = 0;
    float offsetY = 0;

    BalloonOrigin balloon;
    GameObject balloonControllerObject;

    // Use this for initialization
	void Start ()
    {
        Material[] mats = transform.GetComponent<MeshRenderer>().materials;
        //マテリアル設置
        if (SceneManager.GetActiveScene().name == "main")
        {
            mats[0] = bg_Mat[0];//プレイヤー色
                                          
        }
        else
        {
            mats[0] = bg_Mat[1];//青紫
        }
        transform.GetComponent<MeshRenderer>().materials = mats;


        //画面に合わせて
        transform.localScale = new Vector3(Camera.main.orthographicSize * 2.2f * Screen.width / Screen.height,
                                        Camera.main.orthographicSize * 2.2f,
                                        0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        BG_SpriteOffsetChange();

        if (SceneManager.GetActiveScene().name == "main")
        {
            if (balloonControllerObject == null)
            {
                balloonControllerObject = GameObject.FindGameObjectWithTag("Balloon");
                return;
            }
            balloon = balloonControllerObject.GetComponent<BalloonOrigin>();
            BG_SpriteColorChange();
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
    void BG_SpriteColorChange()
    {
        switch (balloon.player.name)
        {
            case "Player1":
                transform.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.red);
                break;
            case "Player2":
                transform.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.blue);
                break;
            case "Player3":
                transform.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.yellow);
                break;
            case "Player4":
                transform.GetComponent<MeshRenderer>().materials[0].SetColor("_Color", Color.green);
                break;
        }
    }

}
