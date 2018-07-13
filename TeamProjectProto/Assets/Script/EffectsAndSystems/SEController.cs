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
    List<AudioClip> playerSEList = new List<AudioClip>();//playerSE格納リスト
    [SerializeField]
    List<AudioClip> balloonSEList = new List<AudioClip>();//balloonSE格納リスト
    [SerializeField]
    List<AudioClip> rouletteSEList = new List<AudioClip>();//ルーレットSE格納リスト

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
        Jump,       //ジャンプ時
        GetItem,    //アイテム取得時
        SendPost,   //ポスト投函時
        Dash,       //ダッシュした時
    }

    /// <summary>
    /// 風船SE
    /// </summary>
    public enum BalloonSE
    {
        Spawn,          //生成
        ChangeTarget,   //風船が移る時
        BlowUp,         //膨らむ
        Blast           //破裂
    }

    /// <summary>
    /// ルーレットSE
    /// </summary>
    public enum RouletteSE
    {
        Lever,          //レバー引くとき
        PressA,         //Aボタン押した時
        RouletteAppear  //ルーレット出現時
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
        _audio.volume = 1;
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
    public void PlayPlayerSEOnce(int index)
    {
        _audio.PlayOneShot(playerSEList[index]);
    }

    /// <summary>
    /// 風船SEを鳴らす
    /// </summary>
    /// <param name="index"></param>
    public void PlayBalloonSE(int index)
    {
        _audio.PlayOneShot(balloonSEList[index]);
    }

    /// <summary>
    /// ルーレットSEを鳴らす
    /// </summary>
    /// <param name="index"></param>
    public void PlayRouletteSE(int index)
    {
        _audio.PlayOneShot(rouletteSEList[index]);
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
