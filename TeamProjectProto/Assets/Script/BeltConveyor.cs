using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConveyor : MonoBehaviour {
    public float speed = 1f;//ベルトコンベアのスピード
    [SerializeField]
    Vector3 moveDirection = Vector3.forward;//進む方向

    /// <summary>
    /// ベルトコンベアに乗ったときに進む方向
    /// </summary>
    /// <returns></returns>
    public Vector3 Conveyor()
    {
        return moveDirection.normalized * speed;
    }

    void Update()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 10f))
        {
            //ステージに接するようにRayで座標を取る。ステージにめり込まないようにする
            this.transform.position = hit.point + new Vector3(0, 0.01f, 0);
        }
    }
}
