/*
 * 作成日時：180515
 * 衝撃波シェーダ―中心指定
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipDropCircleCenter : MonoBehaviour
{
    private Vector3 m_position;//中心点

    [SerializeField]
    Material mat;

    // Use this for initialization
    void Start ()
    {
        m_position = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
        mat.SetVector("_CenterPosition", m_position);
    }
}
