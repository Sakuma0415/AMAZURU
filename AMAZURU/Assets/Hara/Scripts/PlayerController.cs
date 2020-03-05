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

        // 床が存在するかをRayでチェック
        Ray rayForward = new Ray(transform.position, Vector3.forward - transform.up);
        Ray rayBack = new Ray(transform.position, Vector3.back - transform.up);
        Ray rayRight = new Ray(transform.position, Vector3.right - transform.up);
        Ray rayLeft = new Ray(transform.position, Vector3.left - transform.up);
        float rayLength = 10.0f;    // Rayの長さ
        bool findGroundForward = Physics.Raycast(rayForward, out _, rayLength);
        bool findGroundBack = Physics.Raycast(rayBack, out _, rayLength);
        bool findGroundRight = Physics.Raycast(rayRight, out _, rayLength);
        bool findGroundLeft = Physics.Raycast(rayLeft, out _, rayLength);
        Vector3 movePosition = new Vector3((direction.x > 0 && findGroundRight == false) || (direction.x < 0 && findGroundLeft == false) ? 0 : direction.x, direction.y, (direction.z > 0 && findGroundForward == false) || (direction.z < 0 && findGroundBack == false) ? 0 : direction.z);

        // 入力方向を向く処理
        if (direction.magnitude > 0.01f)
        {
            transform.localRotation = Quaternion.LookRotation(direction);
        }

        // 斜め移動時の移動速度の補正
        var r = Mathf.Sqrt(movePosition.x * movePosition.x + movePosition.z * movePosition.z);
        var radian = Vector3.Angle(Vector3.right, movePosition) * Mathf.Deg2Rad;
        r *= (0.7071f + 0.2928f * Mathf.Abs(Mathf.Cos(2 * radian)));

        // 移動処理
        if (moveZ >= 0)
        {
            transform.position += new Vector3(r * Mathf.Cos(radian), 0, r * Mathf.Sin(radian)) * playerSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += new Vector3(r * Mathf.Cos(radian), 0, -r * Mathf.Sin(radian)) * playerSpeed * Time.deltaTime;
        }
    }

    /// <summary>
    /// ローカルな重力を設定
    /// </summary>
    private void SetLocalGravity()
    {
        if (rb.useGravity) { return; }
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
