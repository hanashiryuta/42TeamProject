//
//作成日時：7月27日
//プレイヤーのAIの動き
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlayerAI : MonoBehaviour {
    
    AIMapController aiMapController;//AIのマップ準備クラス
    float[][] influenceMap;//影響マップ
    float[][] otherInfluenceMap;//他のプレイヤーの影響マップ
    [HideInInspector]
    public float[][] coinInfluenceMap;//アイテムの影響マップ

    float aiJumpTime = 0;//AIのジャンプ間隔
    float currentNearBlockHeight = 0;//現在の近いブロック
    float previousNearBlockHeight = 0;//1フレーム前のブロック

    float currentFarBlockHeight = 0;//現在の遠いブロック
    float previousFarBlockHeight = 0;//1フレーム前のブロック

    Vector3 nowPoint = Vector3.zero;//現在の位置

    string[][] aiPersonalityArray;//AIの性格エクセル

    TimeController timeController;//時間管理クラス

    bool isAIDash;//AIがダッシュできるかどうか

    PlayerMove playerMove;
    
    /// <summary>
    /// AIの初期化メソッド
    /// </summary>
    public void AIInitialize()
    {
        playerMove = GetComponent<PlayerMove>();
        ////時間オブジェ取得
        timeController = GameObject.Find("TimeController").GetComponent<TimeController>();
        //影響マップ準備取得
        aiMapController = GameObject.Find("AIMapController").GetComponent<AIMapController>();

        //各影響配列初期化
        influenceMap = new float[(int)aiMapController.mapHeight][];
        otherInfluenceMap = new float[(int)aiMapController.mapHeight][];
        coinInfluenceMap = new float[(int)aiMapController.mapHeight][];

        int I = 0, J = 0;//配列位置保存用int
        float l = 100;//距離保存用float

        //全セル判定
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            //各二重配列初期化
            influenceMap[i] = new float[(int)aiMapController.mapWidth];
            otherInfluenceMap[i] = new float[(int)aiMapController.mapWidth];
            coinInfluenceMap[i] = new float[(int)aiMapController.mapWidth];

            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //各影響マップの影響値初期化
                influenceMap[i][j] = 0;
                if (aiMapController.stageArray[i][j] == "-1")
                    //ステージセルがプレイヤー進入禁止エリアだったら影響値下げる
                    influenceMap[i][j] = -10000;
                otherInfluenceMap[i][j] = 0;
                coinInfluenceMap[i][j] = 0;

                //現在のポジションと取得したセルの距離判定
                float Length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(transform.position.x, aiMapController.positionArray[i][j].y, transform.position.z));

                //距離が1.5以下
                if (Length <= 1.5f)
                {
                    //保存した距離より短ければ
                    if (Length <= l)
                    {
                        //距離更新
                        l = Length;
                        //配列位置保存
                        I = i;
                        J = j;
                    }
                }
            }
        }
        //一番近いセルの位置を保存
        currentNearBlockHeight = previousNearBlockHeight = float.Parse(aiMapController.stageArray[I][J]);

        //性格設定
        aiPersonalityArray = ArraySet();
    }

    string[][] ArraySet()
    {
        //現在のステージに対応したcsv読み込み
        StreamReader sr = new StreamReader(Application.streamingAssetsPath + "/AIPersonality/" + playerMove.PlayerAIState.ToString() + ".csv");
        //リスト作成
        List<string[]> sList = new List<string[]>();
        //リストに各値収納
        while (sr.EndOfStream == false)
        {
            string line = sr.ReadLine();
            sList.Add(line.Split(','));
        }
        sr.Close();
        //リスト返す
        return sList.ToArray();
    }

    /// <summary>
    /// AIプレイヤーの挙動メソッド
    /// </summary>
    public void PlayerAIThink()
    {
        //各影響マップ作製
        ObjectInfluenceSet();
        //影響マップ作製
        MapInfluenceSet();

        Vector3 nextPoint = Vector3.zero;//次のポイント
        Vector3 nearPoint = Vector3.zero;//影響値が高い近いポイント
        Vector3 farPoint = Vector3.zero;//影響値が高い遠いポイント
        float nearInfluence = 0;//近いポイントの影響値
        float farInfluence = 0;//遠いポイントの影響値

        //近いポイントセット
        NextPointSet(out nearPoint, out nearInfluence, out currentNearBlockHeight, 1.5f);
        //遠いポイントセット
        NextPointSet(out farPoint, out farInfluence, out currentFarBlockHeight, 3.5f);

        //近めを目指すか遠目を目指すか
        NearFarCheck(out nextPoint, nearPoint, farPoint, nearInfluence, farInfluence);

        //AIの移動処理
        AIMove(nextPoint);
        //AIのダッシュ処理
        AIDash();

        //ジャンプ間隔更新
        aiJumpTime -= Time.deltaTime;
        //前方にあたり判定飛ばす
        Collider[] colArray = Physics.OverlapBox(transform.position + new Vector3(playerMove.MoveJoy.x, 1, playerMove.MoveJoy.y), new Vector3(0.4f, 0.9f, 0.4f));
        foreach (var cx in colArray)
        {
            //フィールドに当たっていたら
            if (cx.transform.tag == "Field")
            {
                //AIのジャンプ処理
                AIJump();
            }

        }

        //1フレーム前の値を現在の値に更新
        previousNearBlockHeight = currentNearBlockHeight;
        previousFarBlockHeight = currentFarBlockHeight;
        nowPoint = nextPoint;
    }

    /// <summary>
    /// オブジェクト影響値セット
    /// </summary>
    void ObjectInfluenceSet()
    {
        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //各影響値初期化
                otherInfluenceMap[i][j] = 0;
                coinInfluenceMap[i][j] = 0;
            }
        }

        //自分以外の影響値セット
        foreach (var otherPlayer in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (otherPlayer.name != gameObject.name)
            {
                ObjectMapSet_Player(otherPlayer, false, otherInfluenceMap);
            }
        }

        //アイテムの影響値セット
        foreach (var coin in GameObject.FindGameObjectsWithTag("Item"))
        {
            int I = 0, J = 0;//配列位置保存
            float l = 100;//距離保存

            //全セル取得
            for (int i = 0; i < aiMapController.mapHeight; i++)
            {
                for (int j = 0; j < aiMapController.mapWidth; j++)
                {
                    //取得したセルと取得したアイテムとの距離取得
                    float Length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(coin.transform.position.x, aiMapController.positionArray[i][j].y, coin.transform.position.z));

                    //距離が1.5以下で
                    if (Length <= 1.5f)
                    {
                        //距離が保存した値以下なら
                        if (Length <= l)
                        {
                            //距離更新
                            l = Length;
                            //配列位置保存
                            I = i;
                            J = j;
                        }
                    }
                }
            }

            //プレイヤーの位置が床で、コインの位置が高い壁の位置なら
            if (float.Parse(aiMapController.stageArray[I][J]) == 2 && currentNearBlockHeight < 1)
                //影響値を設定しない
                continue;

            //影響値を設定する
            ObjectMapSet_Player(coin, false, coinInfluenceMap);
        }
    }

    /// <summary>
    /// 影響マップ作製
    /// </summary>
    void MapInfluenceSet()
    {
        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {

                //影響値セット
                //エクセルからどの影響割合を取り出すか

                int infState = 1;//取り出す添え字

                //鬼との距離が8以下なら
                if (playerMove.balloonMaster.nowPlayer != null && playerMove.balloon == null && Vector3.Distance(transform.position, playerMove.balloonMaster.nowPlayer.transform.position) <= 8.0f)
                {
                    if (playerMove.DashCountDown >= playerMove.DashLimitTime / 2 && Vector3.Distance(transform.position, playerMove.balloonMaster.nowPlayer.transform.position) <= 5.0f)
                        isAIDash = true;
                    infState = 4;
                }
                //ロスタイム中なら
                else if (timeController.timeState == TimeState.LOSSTIME)
                    infState = 3;
                //バルーンが貯金を破裂させるバルーンなら
                else if (playerMove.balloonMaster.NowBalloon != null && playerMove.balloonMaster.NowBalloon.name.Contains("PostBalloon"))
                    infState = 2;
                //それ以外
                else
                    infState = 1;
                //バルーンが　ついていなければ
                if (playerMove.balloon == null)
                {
                    //影響値セット
                    influenceMap[i][j] =
                    aiMapController.balloonInfluenceMap[i][j] * float.Parse(aiPersonalityArray[infState][1]) +
                    aiMapController.postInfluenceMap[i][j] * float.Parse(aiPersonalityArray[infState][2]) +
                    coinInfluenceMap[i][j] * float.Parse(aiPersonalityArray[infState][3]) +
                    ReturnInfluence(otherInfluenceMap[i][j]) * float.Parse(aiPersonalityArray[infState][4]);


                    //現在の位置が床の場合、高い壁の影響値は0
                    if (float.Parse(aiMapController.stageArray[i][j]) == 2 && previousNearBlockHeight == 0)
                        influenceMap[i][j] = 0;
                }
                else
                {
                    //影響値セット
                    influenceMap[i][j] =
                    aiMapController.balloonInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][1]) +
                    aiMapController.postInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][2]) +
                    coinInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][3]) +
                    otherInfluenceMap[i][j] * float.Parse(aiPersonalityArray[5][4]);
                }

                //セルがプレイヤー進入禁止エリアだったら
                if (float.Parse(aiMapController.stageArray[i][j]) <= -1)
                    //影響値0
                    influenceMap[i][j] = 0;
            }
        }
    }

    /// <summary>
    /// 各オブジェクト影響値設定
    /// </summary>
    /// <param name="obj">オブジェクト</param>
    /// <param name="isReturn">反転させるかどうか</param>
    /// <param name="objInfluenceMap">影響マップ</param>
    void ObjectMapSet_Player(GameObject obj, bool isReturn, float[][] objInfluenceMap)
    {
        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //オブジェクトと取得したセルの距離を取得
                float Length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(obj.transform.position.x, aiMapController.positionArray[i][j].y, obj.transform.position.z));

                float influence = 0;//影響値

                //距離に応じた影響値を設定
                if (isReturn)
                    //反転させるなら
                    //近いほど影響値低く設定
                    influence = Length / 20;
                else
                    //反転させないなら
                    //近いほど影響値高く設定
                    influence = 1 - Length / 20;
                //影響値の範囲を0から1の間で設定
                if (influence < 0)
                    influence = 0;
                else if (influence > 1)
                    influence = 1;

                //取り出したセルの影響値より求めた影響値が高ければ
                if (objInfluenceMap[i][j] < influence)
                {
                    //影響値更新
                    objInfluenceMap[i][j] = influence;
                }
            }
        }
    }

    /// <summary>
    /// 影響値反転メソッド
    /// </summary>
    /// <param name="objectInfuence">影響マップ</param>
    /// <returns>影響マップ</returns>
    float ReturnInfluence(float objectInfuence)
    {
        float returnInfluence = 1 - objectInfuence;

        return returnInfluence;
    }

    /// <summary>
    /// 次に目指すポイントをセット
    /// </summary>
    /// <param name="point">次のポイント</param>
    /// <param name="influence">影響値</param>
    /// <param name="blockHight">ブロックの高さ</param>
    /// <param name="findRadius">検索範囲</param>
    void NextPointSet(out Vector3 point, out float influence, out float blockHight, float findRadius)
    {
        //初期化
        point = Vector3.zero;
        influence = 0;
        blockHight = 0;

        //全セル取得
        for (int i = 0; i < aiMapController.mapHeight; i++)
        {
            for (int j = 0; j < aiMapController.mapWidth; j++)
            {
                //取得したセルと現在位置との距離取得
                float length = Vector3.Distance(aiMapController.positionArray[i][j], new Vector3(transform.position.x, aiMapController.positionArray[i][j].y, transform.position.z));

                //距離が検索範囲以内　かつ　0.5以上（近すぎるのは判定外）
                if (length <= findRadius && length >= 0.5f)
                {
                    //取得したセルの影響値が保存している影響値より高かったら
                    if (influenceMap[i][j] > influence)
                    {
                        //保存影響値更新
                        influence = influenceMap[i][j];
                        //次に目指す位置設定
                        point = aiMapController.positionArray[i][j];
                        //目指す位置のブロック位置設定
                        blockHight = float.Parse(aiMapController.stageArray[i][j]);

                        if (playerMove.balloon != null && influence >= 0.5 && playerMove.DashCountDown >= playerMove.DashLimitTime / 5)
                            isAIDash = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 近場を目指すか遠目を目指すか（谷判定用）
    /// </summary>
    /// <param name="nextPoint">次のポイント</param>
    /// <param name="nearPoint">近場で影響値が高いポイント</param>
    /// <param name="farPoint">遠目で影響値が高いポイント</param>
    /// <param name="nearInfluence">近場のポイントの影響値</param>
    /// <param name="farInfluence">遠目のポイントの影響値</param>
    void NearFarCheck(out Vector3 nextPoint, Vector3 nearPoint, Vector3 farPoint, float nearInfluence, float farInfluence)
    {
        /*
        自身が壁の上にいる場合に谷を跳び越すかどうかの判定に使う
        遠目を目指す、自身が壁の上以上、目指す位置が高い壁の上、間に壁がない　なら谷判定
        */

        //遠目の影響値のほうが高く　かつ
        //1フレーム前（移動前の今いる位置）のブロックが壁以上　かつ
        //現在（移動前の目指す場所）が高い壁ならば
        if (nearInfluence < farInfluence &&
            previousNearBlockHeight >= 1 &&
            currentFarBlockHeight == 2)
        {
            //レイに当たった物体
            RaycastHit hit;

            //レイを飛ばす　自分が立っている壁の底辺から1.5（1セル分）のレイ
            if (Physics.Raycast(new Vector3(nowPoint.x, -2, nowPoint.z), (farPoint - nowPoint).normalized, out hit, 1.5f))
            {
                //もし壁以外があれば
                if (hit.transform.tag != "Field")
                {
                    //遠目を目指す
                    nextPoint = farPoint;
                    //ジャンプ
                    AIJump();
                }
                //壁があれば
                else
                {
                    //近場を目指す
                    nextPoint = nearPoint;
                }
            }
            //何もレイが当たらなければ
            else
            {
                //遠目を目指す
                nextPoint = farPoint;
                //ジャンプ
                AIJump();
            }
        }
        //普通は
        else
        {
            //近場を目指す
            nextPoint = nearPoint;
        }
    }

    /// <summary>
    /// AIのジャンプ処理
    /// </summary>
    void AIJump()
    {
        //AIのジャンプ間隔が来たなら
        if (aiJumpTime <= 0.0f)
        {
            //ジャンプパワー設定
            playerMove.jumpPower = playerMove.balloon != null ? playerMove.balloonJumpPower * 700 : playerMove.originJumpPower * 700;

            //地面にいたら
            if (playerMove.jumpCount == 0)
            {
                //ジャンプ
                playerMove.rigid.AddForce(new Vector3(0, playerMove.jumpPower, 0));
                //SE鳴らす
                playerMove.playerSE.PlayPlayerSEOnce((int)SEController.PlayerSE.Jump);
                //ジャンプ間隔再設定
                aiJumpTime = 0.3f;
            }

            //ジャンプカウント増加
            playerMove.jumpCount++;

            //上限設定
            if (playerMove.jumpCount >= 1)
                playerMove.jumpCount = 1;
        }
    }

    /// <summary>
    /// AIの移動処理
    /// </summary>
    /// <param name="nextPoint">次の位置</param>
    void AIMove(Vector3 nextPoint)
    {
        //方向指定
        Vector3 moves = nextPoint - transform.position;
        //正規化
        moves = moves.normalized;

        //ジャンプ中は方向変えない
        if (playerMove.jumpCount < 1)
        {
            //移動に渡す
            playerMove.MoveJoy = new Vector2(moves.x, moves.z);

            //正規化
            playerMove.MoveJoy = playerMove.MoveJoy.normalized;
        }

        //風船を持っていないとき
        if (playerMove.balloon == null)
        {
            //ダッシュ中
            if (playerMove.IsDash)
            {
                //倍率によって乗算
                playerMove.moveSpeed = playerMove.originMoveSpeed * playerMove.dashSpeedScale;
            }
            else playerMove.moveSpeed = playerMove.originMoveSpeed;
        }
        //風船を持っている時
        else
        {
            //ダッシュ中
            if (playerMove.IsDash)
            {
                //倍率によって乗算
                playerMove.moveSpeed = playerMove.balloonMoveSpeed * playerMove.dashSpeedScale;
            }
            else playerMove.moveSpeed = playerMove.balloonMoveSpeed;
        }
    }

    /// <summary>
    /// AIのダッシュ処理
    /// </summary>
    void AIDash()
    {
        if (playerMove.IsDash)
        {
            //ダッシュ中にパーティクルを生成
            playerMove.dashParticleTime -= Time.deltaTime;
            if (playerMove.dashParticleTime <= 0)
            {
                Instantiate(playerMove.origin_Dash_Particle, transform.position, Quaternion.identity);
                playerMove.dashParticleTime = 0.1f;
            }
        }
        else
        {
            playerMove.SetDashLimitTime(playerMove.holdItemCount, playerMove.dashTimePerItem);
        }

        // ダッシュ可能なら
        if (isAIDash)
        {
            //カウントダウン
            if (playerMove.balloon != null)
            {
                //風船を持つプレイヤーは消費量半分
                playerMove.DashCountDown -= Time.deltaTime / 2f;
            }
            else
            {
                playerMove.DashCountDown -= Time.deltaTime;
            }

            if (playerMove.DashCountDown > 0)
            {
                playerMove.IsDash = true;

                //ダッシュしたら1度だけ音を鳴らす
                if (playerMove.dashStart)
                {
                    //効果音追加
                    playerMove.playerSE.PlayPlayerSEOnce((int)SEController.PlayerSE.Dash);
                }
                playerMove.dashStart = false;
            }
            else
            {
                playerMove.DashCountDown = 0;
                playerMove.IsDash = false;
                //ダッシュ不可能に
                isAIDash = false;
            }
        }
        //ダッシュ不可能なら
        else
        {
            playerMove.IsDash = false;

            //ゲージ切れでしたら１秒待つから回復
            if (playerMove.DashCountDown <= 0 && playerMove.dashTiredTime > 0)
            {
                playerMove.dashTiredTime -= Time.deltaTime;
            }
            else
            {
                //半分の速度でカウントダウン回復
                playerMove.DashCountDown += Time.deltaTime/* / 2f*/;
                playerMove.dashTiredTime = 1f;
            }
        }

        //上限に超えないようにする
        if (playerMove.DashCountDown >= playerMove.DashLimitTime)
        {
            playerMove.DashCountDown = playerMove.DashLimitTime;
        }
    }
}
