//
//作成日時：4月26日
//中心物体移動処理
//作成者：葉梨竜太
//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostRespawn : MonoBehaviour {

	List<GameObject> postList;//中心物体リスト
	public GameObject originPost;//中心物体
	public float postCount;//中心物体数
	List<GameObject> childList;//移動先リスト

	float interval;//表示するまでの間隔

	// Use this for initialization
	void Start () {
		//初期化処理
		postList = new List<GameObject>();
		childList = new List<GameObject>();

		//移動先リスト追加
		for (int i = 0; i < transform.childCount; i++)
		{
			childList.Add(transform.GetChild(i).gameObject);
		}

		//中心物体生成
		for (int i = 0; i < postCount; i++) 
		{
			int rand;
            //ランダムで選ばれたchildListに子があったら選びなおす
			do {
				rand = Random.Range (0, childList.Count);
			} while(childList[rand].transform.childCount >= 1);
            
			//リスト追加
			postList.Add(Instantiate(originPost, childList[rand].transform.position, Quaternion.identity, childList[rand].transform));
			//移動先候補から現在の位置を除外
			//childList.RemoveAt(rand);
		}
	}

	// Update is called once per frame
	void Update () {
		//中心物体リスト検索
		foreach (var post in postList) {
			//移動可能なら
			if (post.GetComponent<PostController> ().isRespawn) {
				//現在のPosition保存
				//GameObject nowPosition = post.transform.parent.gameObject;

				int rand;
                //ランダムで選ばれたchildListに子があったら選びなおす
                do
                {
					rand = Random.Range (0, childList.Count);
				} while(childList[rand].transform.childCount >= 1);

				//移動先Position保存
				GameObject nextPosition = childList [rand];

                //今の移動先を次回の候補から除外して
                //childList.RemoveAt (rand);

                //今のPositionを次回の候補に入れる
                //childList.Add (nowPosition);


                //移動処理
                post.transform.parent = nextPosition.transform;//次のポジションの地点に親子付けする
				post.transform.position = nextPosition.transform.position;
				post.GetComponent<PostController> ().isRespawn = false;
			}

			//表示されていない場合の処理
			if (post.GetComponent<PostController> ().activity==false) {
				interval += 1;
                if (interval >= 300) {
					post.SetActive (post.GetComponent<PostController>().activity=true);
					interval = 0;
                    post.GetComponent<PostController>().activeCount = 0;
                    post.GetComponent<PostController>().mesh.enabled = true;//透明化を解除
                }
			}
		}
	}
}
