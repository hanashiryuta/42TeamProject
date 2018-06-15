using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBXSetMaterial : MonoBehaviour
{
    [SerializeField]
    Texture targetTex;

	// Use this for initialization
	void Start ()
    {
        transform.GetComponentInChildren<MeshRenderer>().materials[0].mainTexture = targetTex;//テクスチャ変更
        transform.GetComponentInChildren<MeshRenderer>().materials[0].SetColor("_EmissionColor", new Color(0.2f, 0.2f, 0.2f));
        transform.GetComponentInChildren<MeshRenderer>().materials[0].SetTexture("_EmissionMap", targetTex);
        transform.GetComponentInChildren<MeshRenderer>().materials[0].EnableKeyword("_EMISSION");
    }

    // Update is called once per frame
    void Update () {
		
	}
}
