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
    List<AudioClip> systemSEList = new List<AudioClip>();//SE格納リスト
    [SerializeField]
    List<AudioClip> plsyerSEList = new List<AudioClip>();//playerSE格納リスト
    AudioSource _audio;// AudioSource
    public AudioSource Audio
    {
        get { return _audio; }
        set { _audio = value; }
    }

    /// <summary>
    /// システムSE
    /// </summary>
    public enum SystemSE
    {
        CursorMove,     //カーソル移動
        OK,             //決定
        CharacterSpawn, //キャラ生成
        Cancel     //キャンセル
    }

    /// <summary>
    /// プレイヤーSE
    /// </summary>
    public enum PlayerSE
    {
        Jump,           //ジャンプ時
        GetItem,        //アイテム取得時
        SendPost,       //ポスト投函時
        Dash            //ダッシュした時
    }

    // Use this for initialization
    void Awake ()
    {
        //AudioSourceコンポーネントがなかったら
        if (transform.GetComponent<AudioSource>() == null)
        {
            //AudioSourceコンポーネントを追加
            transform.gameObject.AddComponent<AudioSource>();
        }
        //Audio設定
        _audio = transform.GetComponent<AudioSource>();
        _audio.playOnAwake = false;
        _audio.loop = false;//ループしない
    }

    /// <summary>
    /// システムSEを鳴らす
    /// </summary>
    /// <param name="index"></param>
    public void PlaySystemSE(int index)
    {
        _audio.PlayOneShot(systemSEList[index]);
    }

    /// <summary>
    /// プレイヤーSEを鳴らす
    /// </summary>
    /// <param name="index"></param>
    public void PlayerPlayreSEOnce(int index)
    {
        _audio.PlayOneShot(plsyerSEList[index]);
    }

    /// <summary>
    /// キャンセルSE
    /// </summary>
    public void Cancel_SE()
    {
        _audio.PlayOneShot(systemSEList[(int)SystemSE.Cancel]);
    }

    /// <summary>
    /// SEの長さ
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public float SE_Length(int index)
    {
        return systemSEList[index].length;
    }
}
