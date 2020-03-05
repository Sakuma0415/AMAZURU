using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのRigidbody")] private Rigidbody rb = null;
    [SerializeField, Header("重力の設定")] private Vector3 localGravity;
    [SerializeField, Header("移動速度")] private float playerSpeed = 1.0f;

    /// <summary>
    /// 初期化
    /// </summary>
    private void PlayerInit()
    {
        
    }

    /// <summary>
    /// 移動を管理する
    /// </summary>
    private void PlayerMove()
    {
        // 入力値を取得
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(moveX, 0, moveZ);

        // 斜め移動時の移動速度の補正
        var r = Mathf.Sqrt(direction.x * direction.x + direction.z * direction.z);
        var radian = Vector3.Angle(Vector3.right, direction) * Mathf.Deg2Rad;
        r *= (0.7071f + 0.2928f * Mathf.Abs(Mathf.Cos(2 * radian)));

        // 移動処理
        if(moveZ >= 0)
        {
            transform.position += new Vector3(r * Mathf.Cos(radian), 0, r * Mathf.Sin(radian)) * playerSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(r * Mathf.Cos(radian), 0, -r * Mathf.Sin(radian)) * playerSpeed * Time.deltaTime;
        }

        // 移動方向を向く処理
        if(direction.magnitude > 0.01f)
        {
            transform.localRotation = Quaternion.LookRotation(direction);
        }
    }

    /// <summary>
    /// ローカルな重力を設定
    /// </summary>
    private void SetLocalGravity()
    {
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerInit();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMove();
    }

    /// <summary>
    /// Rigidbody系の処理
    /// </summary>
    private void FixedUpdate()
    {
        SetLocalGravity();
    }

    /// <summary>
    /// Editor上で初期化
    /// </summary>
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
    }
}
