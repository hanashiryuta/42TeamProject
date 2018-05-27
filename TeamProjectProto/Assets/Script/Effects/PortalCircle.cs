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
    bool isSwelled = false;//風船膨らんだか？

    [SerializeField]
    float range = 1f; //変化後半径
    [SerializeField]
    float seconds = 1f; //継続時間
    bool isCreateCircle = false;

    BalloonController balloonController;

    readonly int subTexPropertyId = Shader.PropertyToID("_SubTex");
    readonly int radiusPropertyId = Shader.PropertyToID("_Radius");
    readonly int widthPropertyId = Shader.PropertyToID("_Width");


    void Awake()
    {
        SetPortalRadius(0);//半径を初期化（歪みを無くす）
        SetPortalWidth(portalWidth);//円の太さを指定
        tmpDefaltRadius = defaltPortalRadius;///設定値を格納する
        tmpWidth = portalWidth;
    }

    void Start()
    {
        material.SetTexture(subTexPropertyId, texture);
        balloonController = GameObject.FindGameObjectWithTag("Balloon").GetComponent<BalloonController>();
    }

    void Update()
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

    /// <summary>
    /// ポータルを開く（外側に伸ばす）
    /// </summary>
    void OpenPortal()
    {
        DOTween.KillAll();
        DOTween.To(() => currentPortalRadius, SetPortalRadius, range, seconds).SetEase(Ease.OutBack);
    }

    /// <summary>
    /// ポータルを閉める
    /// </summary>
    void ClosePortal()
    {
        DOTween.KillAll();
        DOTween.To(() => currentPortalRadius, SetPortalRadius, range, seconds).SetEase(Ease.OutBack);
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
        FindDangerTarget();

        if (target != null)//ターゲットがあったら
        {
            currentPortalRadius = defaltPortalRadius;

            BalloonBlowUp(balloonController.blastCount);

            SetPortalRadius(currentPortalRadius);//円の半径を指定
            SetPortalWidth(portalWidth);//円の太さを指定

            var targetPosition = Camera.main.WorldToScreenPoint(target.transform.position);//ターゲットの座標をスクリーン座標に変換

            var uv = new Vector3(
                targetPosition.x / Screen.width,
                targetPosition.y / Screen.height, 0);

            material.SetVector("_Position", uv);

            var fluct = Mathf.Sin(Time.timeSinceLevelLoad * 3) * 0.1f + 0.9f;//円を拡張・縮小
            SetPortalRadius(currentPortalRadius * fluct);

            material.SetFloat("_Aspect", Screen.height / (float)Screen.width);//アスペクトを合わせる
        }

        if (balloonController.IsBlast)//風船が爆発したら
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
            //半径を初期化
            InitRadius();
        }
    }

    /// <summary>
    /// 内向け円
    /// </summary>
    void SetCircleToInside()
    {
        FindColorChangedTarget();

        if (target != null)//ターゲットがあったら
        {
            currentPortalRadius = defaltPortalRadius;

            SetPortalRadius(currentPortalRadius);//円の半径を指定
            SetPortalWidth(portalWidth);//円の太さを指定

            var targetPosition = Camera.main.WorldToScreenPoint(target.transform.position);//ターゲットの座標をスクリーン座標に変換

            var uv = new Vector3(
                targetPosition.x / Screen.width,
                targetPosition.y / Screen.height, 0);

            material.SetVector("_Position", uv);

            material.SetFloat("_Aspect", Screen.height / (float)Screen.width);//アスペクトを合わせる
        }

        if (balloonController.IsColorChanged)//色変わった時
        {
            isCreateCircle = true;
        }

        if (isCreateCircle)
        {
            ClosePortal();
            target = null;
            isCreateCircle = false;
        }

        //円が目的値に越えたら
        if (currentPortalRadius <= range)
        {
            //半径を初期化
            InitRadius();
            Debug.Log(target);
        }
    }

    /// <summary>
    /// ターゲットを探す（赤風船）
    /// </summary>
    private void FindDangerTarget()
    {
        if(balloonController.BalloonState == BalloonState.DANGER)//風船が赤色の時
        {
            target = balloonController.transform.gameObject;//ターゲットに指定
        }
        else
        {
            target = null;
        }
    }

    /// <summary>
    /// ターゲットを探す（色変化）
    /// </summary>
    void FindColorChangedTarget()
    {
        if (balloonController.IsColorChanged)//色変わった時
        {
            target = balloonController.transform.gameObject;//ターゲットに指定
            Debug.Log("target");
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
