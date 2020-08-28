using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField, Tooltip("風の最大有効範囲(マス)")] private int windMaxArea = 1;

    [SerializeField, Header("正面")] private bool forward = true;
    [SerializeField, Header("背面")] private bool back = false;
    [SerializeField, Header("右")] private bool right = false;
    [SerializeField, Header("左")] private bool left = false;

    private Coroutine coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        WindInit();
    }

    // Update is called once per frame
    void Update()
    {
        ShotWind();
    }

    /// <summary>
    /// 初期化と風を飛ばせる範囲を設定する
    /// </summary>
    private void WindInit()
    {
        windMaxArea = Mathf.Min(Mathf.Max(1, windMaxArea), 5);
    }

    /// <summary>
    /// 風を出す処理
    /// </summary>
    private void ShotWind()
    {
        // 対象オブジェクトに風が当たったかをチェック
        if (forward)
        {
            CreateWind(transform.forward);
        }

        if (back)
        {
            CreateWind(-transform.forward);
        }

        if (right)
        {
            CreateWind(transform.right);
        }

        if (left)
        {
            CreateWind(-transform.right);
        }
        
    }

    /// <summary>
    /// 風に当たった時の処理
    /// </summary>
    private void HitWind()
    {
        
    }

    private void CreateWind(Vector3 direction)
    {
        float length = 1.0f;
        RaycastHit hit;
        for(int i = 0; i < windMaxArea; i++)
        {
            Vector3 rayPosition = transform.localPosition + direction * (i + 1);
            Ray ray = new Ray(rayPosition, direction);

            if(Physics.Raycast(ray, 1.3f) == false)
            {
                ray = new Ray(rayPosition, -transform.up);
                if(Physics.Raycast(ray, out hit, 1.3f))
                {

                }
            }
        }


        if (Physics.BoxCast(transform.localPosition, Vector3.one * 0.25f, direction, out hit, transform.localRotation, length))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // プレイヤーに当たった場合
                HitWind();
            }
        }
    }
}
