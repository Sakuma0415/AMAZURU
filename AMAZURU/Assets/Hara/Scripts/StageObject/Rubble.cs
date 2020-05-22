using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rubble : MonoBehaviour
{
    private WaterHi stageWater = null;
    [SerializeField, Tooltip("水面レイヤー")] private LayerMask layerMask;
    [SerializeField, Tooltip("Rigidbody")] private Rigidbody rb = null;
    [SerializeField, Tooltip("コライダー")] private BoxCollider rubbleCollider = null;

    // ゲーム開始時の座標
    private Vector3 startPosition = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        RubbleInit();
    }

    private void FixedUpdate()
    {
        RubbleMove();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void RubbleInit()
    {
        // 開始座標の設定
        startPosition = transform.position;

        // 水オブジェクトの取得
        Ray ray = new Ray(startPosition, Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 200f, layerMask))
        {
            stageWater = hit.transform.GetComponent<WaterHi>();
        }
    }

    /// <summary>
    /// 水面上昇に伴う移動
    /// </summary>
    private void RubbleMove()
    {
        if(stageWater != null)
        {
            Vector3 position;
            float fixPos = rubbleCollider.size.y / (rubbleCollider.size.y * 4);
            if(stageWater.max < startPosition.y - fixPos)
            {
                position = startPosition;
            }
            else
            {
                position = new Vector3(transform.position.x, stageWater.max + fixPos, transform.position.z);
            }
            rb.position = position;
        }
    }
}
