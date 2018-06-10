/*
 * 作成日時：180514
 * ポータルっぽい円を生成
 * 作成者：何承恩
 */
using DG.Tweening;
using UnityEngine;

public class PortalCircle : MonoBehaviour
{
    [SerializeField]
    bool isDirectionToOutside = false;

    [SerializeField]
    Material material;
    [SerializeField]
    Texture texture;//サブテクスチャ
    [SerializeField]
    float defaltPortalRadius = 0.05f;//円の半径(設定値)
    float tmpDefaltRadius = 0;//設定値を一時格納する変数
    [SerializeField]
    float currentPortalRadius = 0;//円の半径(現在値)
    [SerializeField]
    float portalWidth = 0.05f;//円の太さ
    float tmpWidth = 0;
    [SerializeField]
    float increaseValue = 0.01f;//膨らむ値
    GameObject target;//円の中心目標(デフォはNULL)
    bool isBlow = false;//円を吹き飛ばすか
    public bool IsBlow
    {
        get { return isBlow; }
        set { isBlow = value; }
    }
    bool isSwelled = false;//風船膨らんだか？

    [SerializeField]
    float range = 1f; //変化後半径
    [SerializeField]
    float seconds = 1f; //継続時間
    bool isCreateCircle = false;

    BalloonMaster _balloonM;//風船総合管理クラス
    public BalloonMaster BalloonM
    {
        get { return _balloonM; }
        set { _balloonM = value; }
    }

    GameObject _targetCenter;//風船エフェクト座標オブジェ
    public GameObject TargetCenter
    {
        get { return _targetCenter; }
        set { _targetCenter = value; }
    }

    readonly int subTexPropertyId = Shader.PropertyToID("_SubTex");
    readonly int radiusPropertyId = Shader.PropertyToID("_Radius");
    readonly int widthPropertyId = Shader.PropertyToID("_Width");


    void Awake()
    {
        SetPortalRadius(0);//半径を初期化（歪みを無くす）
        SetPortalWidth(portalWidth);//円の太さを指定
        tmpDefaltRadius = defaltPortalRadius;///設定値を格納する
        tmpWidth = portalWidth;

        if (material.name.IndexOf("Outside") > 0)//マテリアルに沿って円の方向を設定
        {
            isDirectionToOutside = true;
        }
        else
        {
            isDirectionToOutside = false;
        }

    }

    void Start()
    {
        DOTween.KillAll();

        material.SetTexture(subTexPropertyId, texture);
    }

    void Update()
    {
        //ターゲットがある場合
        if (_targetCenter != null)
        {

            // 爆発円（外向け）
            if (isDirectionToOutside)
            {
                SetCircleToOutside();
            }
            // 集中円（内向け）
            else
            {
                SetCircleToInside();
            }
        }
    }
    float tmpRange = 0;
    /// <summary>
    /// ポータルを開く（外側に伸ばす）
    /// </summary>
    void OpenPortal()
    {
        //DOTween.KillAll(); ここでKillしたらCameraChakeが止まっちゃう
        DOTween.To(() => currentPortalRadius, SetPortalRadius, range, seconds).SetEase(Ease.OutExpo);
    }


    /// <summary>
    /// ポータルを閉める
    /// </summary>
    void ClosePortal()
    {
        DOTween.KillAll();
        DOTween.To(() => currentPortalRadius, SetPortalRadius, range, seconds).SetEase(Ease.OutCubic);
    }

    /// <summary>
    /// 半径を指定
    /// </summary>
    /// <param name="radius"></param>
    void SetPortalRadius(float radius)
    {
        currentPortalRadius = radius;
        material.SetFloat(radiusPropertyId, radius);
    }

    /// <summary>
    /// 輪っかの太さを指定
    /// </summary>
    /// <param name="width"></param>
    void SetPortalWidth(float width)
    {
        material.SetFloat(widthPropertyId, width);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Graphics.Blit(src, dest, material);
    }

    /// <summary>
    /// 外向け円
    /// </summary>
    void SetCircleToOutside()
    {
        if (BalloonM.NowBState == NowBalloonState.DANGER) //風船が赤い時
        {
            currentPortalRadius = defaltPortalRadius;

            BalloonBlowUp(BalloonM.blastCount);

            SetPortalRadius(currentPortalRadius);//円の半径を指定
            SetPortalWidth(portalWidth);//円の太さを指定

            var targetPosition = Camera.main.WorldToScreenPoint(_targetCenter.transform.position);//ターゲットの座標をスクリーン座標に変換

            var uv = new Vector3(
                targetPosition.x / Screen.width,
                targetPosition.y / Screen.height, 0);

            material.SetVector("_Position", uv);

            var fluct = Mathf.Sin(Time.timeSinceLevelLoad * 3) * 0.1f + 0.9f;//円を拡張・縮小
            SetPortalRadius(currentPortalRadius * fluct);

            material.SetFloat("_Aspect", Screen.height / (float)Screen.width);//アスペクトを合わせる
        }

        if (BalloonM.IsBlast)//風船が爆発したら
        {
            isBlow = true;//円を吹き飛ばす
        }

        // 円を吹き飛ばすか
        if (isBlow)
        {
            portalWidth = 0.3f;//円の太さを固定値に拡大
            SetPortalWidth(portalWidth);//円の太さを指定

            isSwelled = true;

            OpenPortal();
            isBlow = false;
        }

        //拡大した円が目的値に越えたら
        if (currentPortalRadius >= range)
        {
            Debug.Log("InitCircle");
            //半径を初期化
            InitRadius();
        }
    }

    /// <summary>
    /// 内向け円
    /// </summary>
    void SetCircleToInside()
    {
        if (BalloonM.IsColorChanged) //色変わった時
        {
            currentPortalRadius = defaltPortalRadius;

            SetPortalRadius(currentPortalRadius);//円の半径を指定
            SetPortalWidth(portalWidth);//円の太さを指定

            var targetPosition = Camera.main.WorldToScreenPoint(_targetCenter.transform.position);//ターゲットの座標をスクリーン座標に変換

            var uv = new Vector3(
                targetPosition.x / Screen.width,
                targetPosition.y / Screen.height, 0);

            material.SetVector("_Position", uv);

            material.SetFloat("_Aspect", Screen.height / (float)Screen.width);//アスペクトを合わせる

            isCreateCircle = true;
        }

        if (isCreateCircle)
        {
            ClosePortal();
            //target = null;
            isCreateCircle = false;
        }

        //円が目的値に越えたら
        if (currentPortalRadius <= range)
        {
            //半径を初期化
            InitRadius();
        }
    }

    /// <summary>
    /// 風船拡大
    /// </summary>
    void BalloonBlowUp(float blastCount)
    {
        if ((blastCount - 20) % 2 == 0 && !isSwelled)//カウントが偶数の時
        {
            defaltPortalRadius = defaltPortalRadius + increaseValue;//半径を拡大
            portalWidth = defaltPortalRadius;
            currentPortalRadius = defaltPortalRadius;
            isSwelled = true;
        }

        if ((blastCount - 20) % 2 == 1)//カウントが奇数の時
        {
            isSwelled = false;
        }
    }

    /// <summary>
    /// 円の半径を初期値に
    /// </summary>
    void InitRadius()
    {
        defaltPortalRadius = tmpDefaltRadius;
        portalWidth = tmpWidth;
        currentPortalRadius = 0;
    }
}
