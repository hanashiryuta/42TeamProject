using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScrollMaterial : MonoBehaviour
{
    [SerializeField]
    public float _moveSpeedX = 0.2f;
    [SerializeField]
    public float _moveSpeedY = 0.2f;

    float offsetX = 0;
    float offsetY = 0;
    
    // Update is called once per frame
    void Update()
    {
        offsetX += _moveSpeedX * Time.deltaTime;
        offsetY += _moveSpeedY * Time.deltaTime;

        transform.GetComponent<MeshRenderer>().materials[0].SetTextureOffset("_MainTex", new Vector2(offsetX, offsetY));
    }
}
