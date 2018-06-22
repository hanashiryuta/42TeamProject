using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDetonationArea : MonoBehaviour
{
    private Vector3 m_position;//中心点

    [SerializeField]
    Material mat;

    // Use this for initialization
    void Start ()
    {
        mat.SetFloat("_Radius", 4f);
        m_position = transform.position;
    }

    // Update is called once per frame
    void Update ()
    {
        m_position = transform.position;
        mat.SetVector("_CenterPosition", m_position);
    }

    private void OnDestroy()
    {
        mat.SetFloat("_Radius", 0f);
    }
}
