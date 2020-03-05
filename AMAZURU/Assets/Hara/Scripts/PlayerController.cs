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

    private bool inputX = false;
    private bool inputZ = false;

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
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        inputX = Mathf.Abs(moveX) > 0.1f;
        inputZ = Mathf.Abs(moveZ) > 0.1f;

        // カメラの向いている方向を取得
        Vector3 cameraForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 床が存在するかをRayでチェック
        Ray rayForward = new Ray(transform.position, cameraForward - transform.up);
        Ray rayBack = new Ray(transform.position, -cameraForward - transform.up);
        Ray rayRight = new Ray(transform.position, playerCamera.transform.right - transform.up);
        Ray rayLeft = new Ray(transform.position, -playerCamera.transform.right - transform.up);
        float rayLength = 10.0f;    // Rayの長さ
        bool findGroundForward = Physics.Raycast(rayForward, out _, rayLength);
        bool findGroundBack = Physics.Raycast(rayBack, out _, rayLength);
        bool findGroundRight = Physics.Raycast(rayRight, out _, rayLength);
        bool findGroundLeft = Physics.Raycast(rayLeft, out _, rayLength);
        
        // プレイヤーカメラ起点の入力方向
        Vector3 direction = cameraForward * moveZ + playerCamera.transform.right * moveX;
        
        // 入力方向を向く処理
        if(direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
            rot = Quaternion.Slerp(transform.rotation, rot, 10.0f * Time.deltaTime);
            transform.rotation = rot;
        }

        // 斜め移動時の移動速度の補正
        Vector2 moveDirection = new Vector2(moveX * Mathf.Sqrt(1 - (moveZ * moveZ) / 2.0f), moveZ * Mathf.Sqrt(1 - (moveX * moveX) / 2.0f));
        float moveVec = Mathf.Sqrt((moveDirection.x * moveDirection.x) + (moveDirection.y * moveDirection.y));

        // 移動処理
        playerVec = moveVec * transform.forward;
        playerVec = new Vector3((moveX > 0 && findGroundRight == false) || (moveX < 0 && findGroundLeft == false) ? 0 : playerVec.x, 0, (moveZ > 0 && findGroundForward == false) || (moveZ < 0 && findGroundBack == false) ? 0 : playerVec.z);
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
        if (inputX)
        {
            rb.position += new Vector3(playerVec.x, 0, 0) * playerSpeed * Time.fixedDeltaTime;
        }

        if (inputZ)
        {
            rb.position += new Vector3(0, 0, playerVec.z) * playerSpeed * Time.fixedDeltaTime;
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
