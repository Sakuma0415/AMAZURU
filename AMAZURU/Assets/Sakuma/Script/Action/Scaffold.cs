using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaffold : MonoBehaviour
{
    [SerializeField]
    WaterHi waterHi;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    Rigidbody rb;

    [SerializeField, Tooltip("地面のレイヤー")] private LayerMask groundLayer;
    [SerializeField, Tooltip("ステージの最大マス")] private float maxSize = 20;
    private Vector3 maxPos = Vector3.zero;
    
    // Start is called before the first frame update
    void Start()
    {
        Ray ray = new Ray(transform.position ,transform.TransformDirection(Vector3.down));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,200, layerMask))
        {
            waterHi=hit.collider.gameObject.GetComponent<WaterHi>();
        }

        SetMaxPos();
    }

    private void FixedUpdate()
    {
        if (waterHi != null)
        {
            Vector3 position;
            if(waterHi.max < maxPos.y)
            {
                position = new Vector3(transform.position.x, waterHi.max, transform.position.z);
            }
            else
            {
                position = maxPos;
            }
            rb.transform.position = position;
        }
    }

    /// <summary>
    /// 足場の上昇できる上限座標を取得
    /// </summary>
    private void SetMaxPos()
    {
        // 足場の厚さの半分の値
        float scaffoldHalfHeight = transform.localScale.y * 0.5f;

        Ray ray = new Ray(transform.position, Vector3.up);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 200, groundLayer))
        {
            maxPos = new Vector3(hit.point.x, hit.point.y - scaffoldHalfHeight, hit.point.z);
        }
        else
        {
            maxPos = new Vector3(maxSize, maxSize - scaffoldHalfHeight, maxSize);
        }
    }

}
