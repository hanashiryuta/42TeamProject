/*
 * 作成日時：180601
 * 背景OBJ制御関連
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGController : MonoBehaviour
{
    [SerializeField]
    float _moveSpeedX = 0.2f;
    [SerializeField]
    float _moveSpeedY = 0.2f;

    float offsetX = 0;
    float offsetY = 0;

    // Use this for initialization
	void Start ()
    {
        //画面に合わせて
        transform.localScale = new Vector3(Camera.main.orthographicSize * 2.2f * Screen.width / Screen.height,
                                        Camera.main.orthographicSize * 2.2f,
                                        0.1f);

    }

    // Update is called once per frame
    void Update()
    {
        BG_SpriteOffsetChange();
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

}
