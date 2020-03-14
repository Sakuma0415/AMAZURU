using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのRigidbody")] private Rigidbody rb = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;
    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 5)] private float playerSpeed = 0;
    [SerializeField, Header("移動時の起点カメラ")] private Camera playerCamera = null;
    [SerializeField, Header("RayのLayerMask")] private LayerMask layerMask;
    [SerializeField, Header("Rayの長さ"), Range(0, 10)] private float rayLength = 0.5f;
    [SerializeField, Header("足の位置"), Range(-5, 5)] private float footHeight = 0;

    public Camera PlayerCamera { set { playerCamera = value; } }

    // コントローラーの入力
    private bool forward = false;
    private bool back = false;
    private bool right = false;
    private bool left = false;

    // Y軸方向
    private float Yangle = 90;

    /// <summary>
    /// 初期化
    /// </summary>
    private void PlayerInit()
    {
        if(playerCamera == null) { playerCamera = Camera.main; }
    }

    /// <summary>
    /// コントローラ入力を取得
    /// </summary>
    private void GetInputController()
    {
        float inputMin = 0.1f;

        // キー入力取得
        forward = Input.GetAxis("Vertical") > inputMin;
        back = Input.GetAxis("Vertical") < -inputMin;
        right = Input.GetAxis("Horizontal") > inputMin;
        left = Input.GetAxis("Horizontal") < -inputMin;

        // 反対方向の入力を検知したら入力を打ち消す
        if (forward && back) { forward = false; back = false; }
        if (right && left) { right = false; left = false; }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove(bool fixedUpdate)
    {
        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        // プレイヤーカメラの向いているY軸方向
        float playerCameraAngle = -playerCamera.transform.eulerAngles.y;

        // 入力方向
        Vector3 inputDirection = Vector3.zero;

        // 入力を検知したら実行
        if(forward || back || right || left)
        {
            if (forward) { inputDirection += Vector3.forward; }
            if (back) { inputDirection += Vector3.back; }
            if (right) { inputDirection += Vector3.right; }
            if (left) { inputDirection += Vector3.left; }

            // 向きをプレイヤーカメラから見た入力方向へ修正
            float refAngle = 0;
            Yangle = Mathf.SmoothDampAngle(Yangle, Mathf.Atan2(inputDirection.z, inputDirection.x) * Mathf.Rad2Deg + playerCameraAngle, ref refAngle, 0.05f);

            // カメラの向いている方向を取得
            Vector3 cameraForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

            // プレイヤーカメラ起点の入力方向
            Vector3 direction = cameraForward * inputDirection.z + playerCamera.transform.right * inputDirection.x;

            // 入力方向を向く処理
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
            rot = Quaternion.Slerp(transform.rotation, rot, 15 * delta);
            transform.rotation = rot;

            // Rayを飛ばして進めるかをチェック
            float angleLate = 1;
            float forwardAngle = Yangle;
            Ray playerAround = new Ray(transform.position + new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), 0, Mathf.Sin(forwardAngle * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
            if (Physics.Raycast(playerAround, rayLength, layerMask) == false)
            {
                angleLate = 0;
                for (float f = 0; f < 90; f += 10)
                {
                    bool flag = false;
                    playerAround = new Ray(transform.position + new Vector3(Mathf.Cos((f + Yangle) * Mathf.Deg2Rad), 0, Mathf.Sin((f + Yangle) * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
                    if (Physics.Raycast(playerAround, rayLength, layerMask))
                    {
                        forwardAngle += f;
                        flag = true;
                    }
                    playerAround = new Ray(transform.position + new Vector3(Mathf.Cos((Yangle - f) * Mathf.Deg2Rad), 0, Mathf.Sin((Yangle - f) * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
                    if (Physics.Raycast(playerAround, rayLength, layerMask))
                    {
                        forwardAngle -= f;
                        flag = true;
                    }
                    if (flag)
                    {
                        angleLate = Mathf.Cos(f * Mathf.Deg2Rad);
                        break;
                    }
                }
            }

            // 坂道にRayを飛ばして進めるかチェック
            float Xangle = 0;
            Ray playerForward = new Ray(transform.position + Vector3.down * footHeight, new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), 0, Mathf.Sin(forwardAngle * Mathf.Deg2Rad)));
            if (Physics.Raycast(playerForward, angleLate * playerSpeed * delta, layerMask))
            {
                for (float f = 0; f < 90; f += 5)
                {
                    playerForward = new Ray(transform.position + Vector3.down * footHeight, new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad), Mathf.Sin(f * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad)));
                    if (Physics.Raycast(playerForward, angleLate * playerSpeed * delta, layerMask) == false)
                    {
                        Xangle = f;
                        break;
                    }
                }
            }
            else
            {
                for (float f = 0; f > -90; f -= 5)
                {
                    playerForward = new Ray(transform.position + Vector3.down * footHeight, new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad), Mathf.Sin(f * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad)));
                    if (Physics.Raycast(playerForward, angleLate * playerSpeed * delta, layerMask))
                    {
                        Xangle = f + 5;
                        break;
                    }
                }
            }

            // Rayを飛ばしてプレイヤーの移動先座標を決定する
            Vector3 movePosition;
            Ray playerFoot = new Ray(transform.position + new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * playerSpeed * delta * angleLate, Vector3.down);
            if (Physics.Raycast(playerFoot, Mathf.Sin(Xangle * Mathf.Deg2Rad) * playerSpeed * delta * angleLate, layerMask))
            {
                movePosition = new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), 0, Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad));
            }
            else
            {
                movePosition = new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad));
            }

            // プレイヤーを移動させる
            rb.position += movePosition * playerSpeed * delta * angleLate;
        }

        // アニメーション実行
        if(playerAnimator != null)
        {
            playerAnimator.SetBool("wate", forward || back || right || left);
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
        GetInputController();
    }

    /// <summary>
    /// Rigidbody系の処理
    /// </summary>
    private void FixedUpdate()
    {
        PlayerMove(true);
    }

    /// <summary>
    /// Editor上で初期化
    /// </summary>
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
}
