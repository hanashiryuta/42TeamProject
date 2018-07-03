/*
 * 作成日時：180628
 * リザルトでランク表示位置を生成するクラス
 * 作成者：何承恩
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultPositionSpawnController : MonoBehaviour
{
    [SerializeField]
    GameObject origin_rankOBJ;//ランクオブジェ
    List<GameObject> _rankOBJList;//ランクオブジェ格納リスト
    public List<GameObject> RankOBJList
    {
        get { return _rankOBJList; }
        set { _rankOBJList = value; }
    }
    List<Vector2> _defaultPositionsList, _finishPositionsList;//格納リスト
    public List<Vector2> DefaultPositionsList
    {
        get { return _defaultPositionsList; }
        set { _defaultPositionsList = value; }
    }
    public List<Vector2> FinishPositionsList
    {
        get { return _finishPositionsList; }
        set { _finishPositionsList = value; }
    }
    public float positionsXOffset = 300f;//生成間隔
    float firstPositionX = 0;//一番左の位置のX座標
    public float defaultY = 450f;//デフォ待機位置のY座標
    public float finishY = -100f;//アニメ終了位置のY座標

    [SerializeField]
    GameObject playerRankUIParent;//順位表示のテキストOBJの親

    /// <summary>
    /// ランクのDefaltPosition生成・
    /// ランクOBJを生成
    /// </summary>
    public void SetRanksDefaltPosition(int playerNum)
    {
        _defaultPositionsList = new List<Vector2>();
        _rankOBJList = new List<GameObject>();

        for (int i = 0; i < playerNum; i++)
        {
            if (i == 0)//一回目だけ
            {
                firstPositionX = (playerNum / 2) * (-positionsXOffset);//最初の位置を設定
                if (playerNum % 2 == 0)//偶数だったら
                {
                    firstPositionX += positionsXOffset / 2;//間隔をもう半分ずらす
                }
            }
            //デフォ位置格納
            _defaultPositionsList.Add(new Vector2(firstPositionX + i * positionsXOffset, defaultY));
            //デフォ位置にランクOBJ生成し格納
            _rankOBJList.Add(Instantiate(origin_rankOBJ, playerRankUIParent.transform));
            _rankOBJList[i].transform.GetComponent<RectTransform>().localPosition = _defaultPositionsList[i];
        }
    }

    /// <summary>
    /// ランクのFinishPosition生成
    /// </summary>
    public void SetRanksFinishPosition(int playerNum)
    {
        _finishPositionsList = new List<Vector2>();

        for (int i = 0; i < _defaultPositionsList.Count; i++)
        {
            //Finish位置格納
            _finishPositionsList.Add(new Vector2(_defaultPositionsList[i].x, finishY));
        }
    }
}
