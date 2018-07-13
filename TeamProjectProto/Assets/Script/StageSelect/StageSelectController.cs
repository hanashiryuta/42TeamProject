/*
 * 作成日：180612
 * ステージセレクトコントローラー
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using XInputDotNetPure;
using DG.Tweening;

public class StageSelectController : SceneController
{
    //ステージ関連
    [SerializeField]
    GameObject[] stagesList;//すべてのステージを格納するリスト
    GameObject nowStage;//今表示しているステージ
    int nowStageIndex = 0;//今表示しているステージのインデックス

    //接続プレイヤー
    ConnectedPlayerStatus connectedPlayerStatus;//プレイヤーステータス(選択したステージをここに渡す)

    //ボタン
    [SerializeField]
    Button leftBtn, rightBtn;

    //ステージポイント関連
    [SerializeField]
    GameObject stagePointsSet;//ポイント生成場所
    [SerializeField]
    GameObject stagePoint;//ポイントプレハブ
    List<GameObject> stagePointsList;//ポイントリスト
    public float pointsOffset = 40f;//ポイント生成間隔
    float firstPointX = 0;//一番目（左）のポイントのX座標

    //SceneState
    StageSceneState sceneState = StageSceneState.FadeIn;

    // Use this for initialization
    public override void Start()
    {
        if (connectedPlayerStatus == null)
        {
            // ConnectedPlayerStatusで接続しているプレイヤーを受け取る
            connectedPlayerStatus = GameObject.FindGameObjectWithTag("PlayerStatus").GetComponent<ConnectedPlayerStatus>();
        }

        //一つ目のステージを出す
        nowStage = Instantiate(stagesList[0]);
        //ステージポイントを設定
        SetStagePoints();

        base.Start();
    }

    /// <summary>
    /// ステージのポイント生成
    /// </summary>
    void SetStagePoints()
    {
        stagePointsList = new List<GameObject>();

        for(int i = 0; i < stagesList.Length; i++)
        {
            if(i == 0)//一回目だけ
            {
                firstPointX = (stagesList.Length / 2) * (-pointsOffset);//最初の位置を設定
                if (stagesList.Length % 2 == 0)//偶数だったら
                {
                    firstPointX += pointsOffset / 2;//間隔をもう半分ずらす
                }
            }
            //ステージのポイント生成と格納
            stagePointsList.Add(Instantiate(stagePoint, stagePointsSet.transform));
            //位置調整（アンカー位置）
            stagePointsList[i].transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(firstPointX + i * pointsOffset, 0, 0);
        }
        //セットのサイズ設定(両側に付く矢印の位置はセットのサイズに沿って位置変更)
        Vector2 setSize = stagePointsSet.transform.GetComponent<RectTransform>().sizeDelta;
        stagePointsSet.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Abs(firstPointX), setSize.y);
        //最初表示するポイント
        PointToSelected(nowStageIndex);
    }

    /// <summary>
    /// シーンの状態に沿ってメソッド実行
    /// </summary>
    public override void CheckSceneState()
    {
        //Get XInput
        currentState = GamePad.GetState(playerIndex);

        switch (sceneState)
        {
            case StageSceneState.FadeIn://フェードイン中
                if (fadeController.IsFadeInFinish)
                    sceneState = StageSceneState.None;
                break;

            case StageSceneState.None://基準状態
                moveX = currentState.ThumbSticks.Left.X;
                StageSelectXInput();
                break;

            case StageSceneState.ToPreScene://前のシーンに移行
                DOTween.KillAll();
                ToCharacterSelectScene();
                //フェードアウト終わったら
                if (fadeController.IsFadeOutFinish)
                    //NextSceneLoad
                    gameLoad.LoadingStartWithOBJ();
                break;

            case StageSceneState.ToNextScene://次のシーンに移行
                DOTween.KillAll();
                GameStart();
                //フェードアウト終わったら
                if (fadeController.IsFadeOutFinish)
                    //NextSceneLoad
                    gameLoad.LoadingStartWithOBJ();
                break;
        }

        previousState = currentState;
    }

    /// <summary>
    /// ステージセレクト入力
    /// </summary>
    void StageSelectXInput()
    {
        //今のステージを選択してゲームへ（Aボタン）
        if (previousState.Buttons.A == ButtonState.Released &&
            currentState.Buttons.A == ButtonState.Pressed)
        {
            se.PlaySystemSE((int)SEController.SystemSE.OK);//SE
            sceneState = StageSceneState.ToNextScene;
        }
        //左右ボタンの選択状態表示
        ShowBtnSelected();
        //表示しているステージを変更
        ShowStageChage();

        //前のシーンへ（Backボタン）
        if (previousState.Buttons.B == ButtonState.Released &&
            currentState.Buttons.B == ButtonState.Pressed)
        {
            se.PlaySystemSE((int)SEController.SystemSE.Cancel);
            sceneState = StageSceneState.ToPreScene;
        }
    }

    /// <summary>
    /// 選択したステージを表示
    /// </summary>
    void ShowSeletedStage(float rotate)
    {
        //今のステージを削除
        if(nowStage != null)
        {
            Destroy(nowStage);
        }
        //新しいステージを今のステージとする
        nowStage = Instantiate(stagesList[nowStageIndex], new Vector3(0, 0.5f, 0), Quaternion.Euler(0, rotate, 0));
    }

    /// <summary>
    /// 左右ボタンの選択状態表示
    /// </summary>
    void ShowBtnSelected()
    {
        if (nowStageIndex <= 0)//一番左のステージが表示されている
        {
            leftBtn.gameObject.SetActive(false);//左矢印非アクティブ化
        }
        else if (nowStageIndex >= stagesList.Length - 1)//一番右のステージが表示されている
        {
            rightBtn.gameObject.SetActive(false);//右矢印非アクティブ化
        }
        else
        {
            leftBtn.gameObject.SetActive(true);
            rightBtn.gameObject.SetActive(true);
        }
        //右
        if (moveX >= 0.5f)
        {
            rightBtn.Select();
        }
        //左
        if (moveX <= -0.5f)
        {
            leftBtn.Select();

        }
        //ステージ非選択
        if (moveX > -0.05f ||
            moveX < 0.05f)
        {
            transform.GetComponent<EventSystem>().SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// 表示しているステージを変更
    /// </summary>
    void ShowStageChage()
    {
        //右
        if (moveX > 0.8f)
        {
            //遅延中ではなかったら
            if (!isInputDelay)
            {
                inputDelayCnt = inputDelayTime;//遅延時間初期化
                rightBtn.onClick.Invoke();//ボタン処理呼び出し
                //ボタン動く
                DOTween.Punch(() => rightBtn.gameObject.transform.position,
                                x => rightBtn.gameObject.transform.position = x,
                                new Vector3(5, 0, 0),
                                inputDelayTime,
                                5);
                isInputDelay = true;//遅延状態にする
            }
            else
            {
                InputDelayTimeCountDown();
            }
        }

        //左
        if (moveX < -0.8f)
        {
            //遅延中ではなかったら
            if (!isInputDelay)
            {
                inputDelayCnt = inputDelayTime;//遅延時間初期化
                leftBtn.onClick.Invoke();//ボタン処理呼び出し
                //ボタン動く
                DOTween.Punch(() => leftBtn.gameObject.transform.position,
                                x => leftBtn.gameObject.transform.position = x,
                                new Vector3(-5, 0, 0),
                                inputDelayTime,
                                5);
                isInputDelay = true;//遅延状態にする
            }
            else
            {
                InputDelayTimeCountDown();
            }
        }

        //中心位置に戻ったら
        if (moveX > -0.05f &&
            moveX < 0.05f)
        {
            inputDelayCnt = inputDelayTime;//遅延時間初期化
            isInputDelay = false;
        }
    }

    /// <summary>
    /// 右ボタン
    /// </summary>
    public void R_Btn()
    {
        if (nowStageIndex != stagesList.Length - 1)
        {
            PointToDefalt(nowStageIndex);

            float rotate = nowStage.transform.eulerAngles.y;
            nowStageIndex++;
            ShowSeletedStage(rotate);

            PointToSelected(nowStageIndex);

            se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
        }
    }

    /// <summary>
    /// 左ボタン
    /// </summary>
    public void L_Btn()
    {
        if(nowStageIndex != 0)
        {
            PointToDefalt(nowStageIndex);

            float rotate = nowStage.transform.eulerAngles.y;
            nowStageIndex--;
            ShowSeletedStage(rotate);

            PointToSelected(nowStageIndex);

            se.PlaySystemSE((int)SEController.SystemSE.CursorMove);
        }
    }

    /// <summary>
    /// ポイントの外見を元に
    /// </summary>
    /// <param name="index"></param>
    void PointToDefalt(int index)
    {
        //色
        stagePointsList[nowStageIndex].transform.GetComponent<Image>().color = Color.white;
        //大きさ
        stagePointsList[nowStageIndex].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(20, 20);
    }
    /// <summary>
    /// ポイントの外見を選択状態に
    /// </summary>
    /// <param name="index"></param>
    void PointToSelected(int index)
    {
        //色
        stagePointsList[nowStageIndex].transform.GetComponent<Image>().color = Color.yellow;
        //大きさ
        stagePointsList[nowStageIndex].transform.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
    }

    /// <summary>
    /// 今のステージを選択し、格納
    /// </summary>
    void SetStage()
    {
        string stageName = stagesList[nowStageIndex].name;
        connectedPlayerStatus.StageName = stageName;
    }

    /// <summary>
    /// 今のステージを選択してゲームへ
    /// </summary>
    public void GameStart()
    {
        SetStage();
        gameLoad.NextScene = GameLoad.Scenes.Main;
        isSceneChange = true;
    }

    /// <summary>
    /// 前のステージへ
    /// </summary>
    public void ToCharacterSelectScene()
    {
        gameLoad.NextScene = GameLoad.Scenes.CharacterSelect;
        isSceneChange = true;
    }
}
