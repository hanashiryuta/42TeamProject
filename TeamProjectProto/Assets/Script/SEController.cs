/*
 * 作成日時：180622
 * SEコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEController : MonoBehaviour
{
    [SerializeField]
    List<AudioClip> seList = new List<AudioClip>();//SE格納リスト
    AudioSource audio;// AudioSource

    // Use this for initialization
    void Start ()
    {
        //このスクリプトを付けていつオブジェにAudioSourceコンポーネントを追加
        transform.gameObject.AddComponent<AudioSource>();
        //Audio設定
        audio = transform.GetComponent<AudioSource>();
        audio.playOnAwake = false;
        audio.loop = false;//ループしない
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
