using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerControllerCC : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのCharacterController")] private CharacterController character = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;

    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 5)] private float playerSpeed = 0;
    [SerializeField, Header("移動時の起点カメラ")] private Camera playerCamera = null;
    [SerializeField, Header("RayのLayerMask")] private LayerMask layerMask;
    [SerializeField, Header("Rayの長さ"), Range(0, 10)] private float rayLength = 0.5f;
    [SerializeField, Header("足の位置"), Range(-5, 5)] private float footHeight = 0;
    [SerializeField, Header("重力値"), Range(0, 10)] private float gravity = 10.0f;

    public Camera PlayerCamera { set { playerCamera = value; } }

    // コントローラーの入力
    private bool forward = false;
    private bool back = false;
    private bool right = false;
    private bool left = false;

    // Y軸方向
    private float Yangle = 90;

    // プレイヤーの位置(高さ)
    public float PlayerPositionY { private set; get; } = 0;

    // プレイヤーが水に浸かったか
    public bool InWater { set; private get; } = false;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInit();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputController();
        PlayerMove(false);
    }

    private void Reset()
    {
        character = GetComponent<CharacterController>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void PlayerInit()
    {
        if (playerCamera == null) { playerCamera = Camera.main; }
    }

    /// <summary>
    /// コントローラ入力を取得
    /// </summary>
    private void GetInputController()
    {
        // 入力の最低許容値
        float inputMin = 0.1f;

        // キー入力取得
        float inputX = InWater == false ? Input.GetAxis("Horizontal") : 0;
        float inputY = InWater == false ? Input.GetAxis("Vertical") : 0;
        forward = inputY > inputMin;
        back = inputY < -inputMin;
        right = inputX > inputMin;
        left = inputX < -inputMin;
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

        // 移動方向
        Vector3 moveDirection = Vector3.zero;

        if (forward || back || right || left)
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
            Ray playerAround = new Ray(transform.position + new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), footHeight, Mathf.Sin(forwardAngle * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
            if (Physics.Raycast(playerAround, rayLength, layerMask) == false)
            {
                angleLate = 0;
                for (float f = 0; f < 90; f += 10)
                {
                    bool flag = false;
                    playerAround = new Ray(transform.position + new Vector3(Mathf.Cos((f + Yangle) * Mathf.Deg2Rad), footHeight, Mathf.Sin((f + Yangle) * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
                    if (Physics.Raycast(playerAround, rayLength, layerMask))
                    {
                        forwardAngle += f;
                        flag = true;
                        Debug.Log("検知１");
                    }
                    playerAround = new Ray(transform.position + new Vector3(Mathf.Cos((Yangle - f) * Mathf.Deg2Rad), footHeight, Mathf.Sin((Yangle - f) * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
                    if (Physics.Raycast(playerAround, rayLength, layerMask))
                    {
                        forwardAngle -= f;
                        flag = true;
                        Debug.Log("検知２");
                    }
                    if (flag)
                    {
                        angleLate = Mathf.Cos(f * Mathf.Deg2Rad);
                        break;
                    }
                }
            }

            moveDirection = new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), 0, Mathf.Sin(forwardAngle * Mathf.Deg2Rad));

            // 床にRayを飛ばして斜面の角度を取得
            Ray ground = new Ray(transform.position + new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), footHeight, Mathf.Sin(forwardAngle * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(ground, out hit, rayLength, layerMask))
            {
                var nomal = hit.normal;
                Vector3 dir = moveDirection - Vector3.Dot(moveDirection, nomal) * nomal;
                moveDirection = dir.normalized;
                Debug.Log(nomal);
            }
            
            moveDirection *= playerSpeed * delta * angleLate;
        }

        // プレイヤーを移動させる
        moveDirection.y -= gravity * delta;
        character.Move(moveDirection);

        // プレイヤーの位置情報を更新
        PlayerPositionY = transform.position.y + character.center.y;

        // アニメーション実行
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("wate", forward || back || right || left);
        }
    }
}
