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
    GameObject target;//円の中心目標(デフォはNULL)
    bool isBlow = false;//円を吹き飛ばすか
    bool isBlastCnt = false;//風船膨らんだか？

    BalloonController balloonController;

    readonly int subTexPropertyId = Shader.PropertyToID("_SubTex");
    readonly int radiusPropertyId = Shader.PropertyToID("_Radius");
    readonly int widthPropertyId = Shader.PropertyToID("_Width");


    void Awake()
    {
        SetPortalRadius(0);//半径を初期化（歪みを無くす）
        SetPortalWidth(portalWidth);//円の太さを指定
        tmpDefaltRadius = defaltPortalRadius;///設定値を一時格納する
    }

    void Start()
    {
        material.SetTexture(subTexPropertyId, texture);
        balloonController = GameObject.FindGameObjectWithTag("Balloon").GetComponent<BalloonController>();
    }

    void Update()
    {
        FindTarget();

        if(target != null)//ターゲットがあったら
        {
            currentPortalRadius = defaltPortalRadius;

            BalloonBlowUp(balloonController.blastCount);

            SetPortalRadius(currentPortalRadius);//円の半径を指定
            SetPortalWidth(defaltPortalRadius);//円の太さを指定

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

            defaltPortalRadius = tmpDefaltRadius;//歪みを初期値に
            portalWidth = tmpDefaltRadius;
        }

        if (isBlow)
        {
            OpenPortal();
        }

    }

    /// <summary>
    /// ポータルを開く（外側に伸ばす）
    /// </summary>
    void OpenPortal()
    {
        DOTween.KillAll();
        DOTween.To(() => currentPortalRadius, SetPortalRadius, 1f, 2f).SetEase(Ease.OutBack);
        isBlow = false;
    }

    /// <summary>
    /// ポータルを閉める
    /// </summary>
    void ClosePortal()
    {
        DOTween.KillAll();
        DOTween.To(() => currentPortalRadius, SetPortalRadius, 0f, 0.6f).SetEase(Ease.InBack);
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
    /// ターゲットを探す
    /// </summary>
    private void FindTarget()
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
    /// 風船拡大
    /// </summary>
    void BalloonBlowUp(float blastCount)
    {
        if (blastCount > 20)//風船が赤ゾーンに入って
        {
            if ((blastCount - 20) % 2 == 0 && !isBlastCnt)//カウントが偶数の時
            {
                defaltPortalRadius = defaltPortalRadius + 0.015f;//半径を拡大
                portalWidth = defaltPortalRadius;
                currentPortalRadius = defaltPortalRadius;
                isBlastCnt = true;
            }

            if((blastCount - 20) % 2 == 1)//カウントが奇数の時
            {
                isBlastCnt = false;
            }
        }
        else
        {
            currentPortalRadius = defaltPortalRadius;
        }
    }
}
