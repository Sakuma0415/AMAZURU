using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのRigidbody")] private Rigidbody rb = null;
    [SerializeField, Header("重力の設定")] private Vector3 localGravity;
    [SerializeField, Header("移動速度")] private float playerSpeed = 1.0f;
    [SerializeField, Header("移動時の起点カメラ")] private Camera playerCamera = null;
    public Camera PlayerCamera { set { playerCamera = value; } }

    private Vector3 playerVec = Vector3.zero;
    private float inputX = 0;
    private float inputZ = 0;

    private bool findGroundForward = false;

    /// <summary>
    /// 初期化
    /// </summary>
    private void PlayerInit()
    {
        if(playerCamera == null) { playerCamera = Camera.main; }
    }

    /// <summary>
    /// 移動を管理する
    /// </summary>
    private void PlayerMoveControl()
    {
        // 入力値を取得
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");

        // 床が存在するかをRayでチェック
        Ray rayForward = new Ray(transform.position, transform.forward - transform.up);
        float rayLength = 10.0f;    // Rayの長さ
        findGroundForward = Physics.Raycast(rayForward, out _, rayLength);
    }

    /// <summary>
    /// ローカルな重力を設定
    /// </summary>
    private void SetLocalGravity()
    {
        if (rb.useGravity) { return; }
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove()
    {
        // カメラの向いている方向を取得
        Vector3 cameraForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // プレイヤーカメラ起点の入力方向
        Vector3 direction = cameraForward * inputZ + playerCamera.transform.right * inputX;

        // 入力方向を向く処理
        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
            rot = Quaternion.Slerp(transform.rotation, rot, 10.0f * Time.fixedDeltaTime);
            transform.rotation = rot;
        }

        // 移動処理
        if((Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f) && findGroundForward)
        {
            float moveVec = Mathf.Abs(inputX) >= Mathf.Abs(inputZ) ? inputZ / inputX : inputX / inputZ;
            moveVec = 1.0f / Mathf.Sqrt(1.0f + moveVec * moveVec);
            playerVec = direction * moveVec;
            rb.position += playerVec * playerSpeed * Time.fixedDeltaTime;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerInit();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerMoveControl();
    }

    /// <summary>
    /// Rigidbody系の処理
    /// </summary>
    private void FixedUpdate()
    {
        SetLocalGravity();
        PlayerMove();
    }

    /// <summary>
    /// Editor上で初期化
    /// </summary>
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
}
