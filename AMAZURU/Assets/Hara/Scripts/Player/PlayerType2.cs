using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerType2 : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのCharacterController")] private CharacterController character = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;
    [SerializeField, Tooltip("透明な壁")] private BoxCollider hiddenWallPrefab = null;

    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 5)] private float playerSpeed = 0;
    [SerializeField, Header("移動時の起点カメラ")] private Camera playerCamera = null;
    [SerializeField, Header("RayのLayerMask")] private LayerMask layerMask;
    [SerializeField, Header("Rayの長さ"), Range(0, 10)] private float rayLength = 0.5f;
    [SerializeField, Header("重力値"), Range(0, 10)] private float gravity = 10.0f;
    public Camera PlayerCamera { set { playerCamera = value; } }

    // コントローラーの入力
    private bool forward = false;
    private bool back = false;
    private bool right = false;
    private bool left = false;

    // プレイヤーの位置(高さ)
    public float PlayerPositionY { private set; get; } = 0;

    // プレイヤーが水に浸かったか
    public bool InWater { set; private get; } = false;

    // 透明な壁関連の変数
    private Vector3[] rayPosition = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
    private BoxCollider[] hiddenWalls = null;
    private bool[] wallFlags = null;

    // Start is called before the first frame update
    void Start()
    {
        PlayerInit();
        CreateHiddenWall();
    }

    // Update is called once per frame
    void Update()
    {
        GetInputController();
    }

    private void FixedUpdate()
    {
        PlayerMove(true);
        SetHiddenWall();
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

        if(forward && back) { forward = false; back = false; }
        if(right && left) { right = false; left = false; }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove(bool fixedUpdate)
    {
        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        // 入力方向
        Vector3 inputDirection = Vector3.zero;

        // 移動方向
        Vector3 moveDirection = Vector3.zero;

        // プレイヤーの位置情報を更新
        PlayerPositionY = transform.position.y + character.center.y;

        if (forward || back || right || left)
        {
            if (forward) { inputDirection += Vector3.forward; }
            if (back) { inputDirection += Vector3.back; }
            if (right) { inputDirection += Vector3.right; }
            if (left) { inputDirection += Vector3.left; }

            // カメラの向いている方向を取得
            Vector3 cameraForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

            // プレイヤーカメラ起点の入力方向
            Vector3 direction = cameraForward * inputDirection.z + playerCamera.transform.right * inputDirection.x;

            // 入力方向を向く処理
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
            rot = Quaternion.Slerp(transform.rotation, rot, 15 * delta);
            transform.rotation = rot;

            // 移動方向の決定
            float moveX = right ? 1 : left ? -1 : 0;
            float moveZ = forward ? 1 : back ? -1 : 0;
            Vector3 moveVector = new Vector3(moveX * Mathf.Sqrt(1 - (moveZ * moveZ) * 0.5f), 0, moveZ * Mathf.Sqrt(1 - (moveX * moveX) * 0.5f));
            float vec = Mathf.Sqrt(moveVector.x * moveVector.x + moveVector.z * moveVector.z);
            moveDirection = transform.forward * vec;

            // 床にRayを飛ばして斜面の角度を取得
            Ray ground = new Ray(new Vector3(transform.position.x, PlayerPositionY, transform.position.z), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ground, out hit, rayLength, layerMask))
            {
                var nomal = hit.normal;
                Vector3 dir = moveDirection - Vector3.Dot(moveDirection, nomal) * nomal;
                moveDirection = dir.normalized;
                Debug.Log(nomal);
            }

            moveDirection *= playerSpeed * delta;
        }

        // プレイヤーを移動させる
        moveDirection.y -= gravity * delta;
        character.Move(moveDirection);

        // アニメーション実行
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("wate", forward || back || right || left);
        }
    }

    /// <summary>
    /// 透明な壁を生成
    /// </summary>
    private void CreateHiddenWall()
    {
        wallFlags = new bool[rayPosition.Length];
        hiddenWalls = new BoxCollider[rayPosition.Length];
        for(int i = 0; i < hiddenWalls.Length; i++)
        {
            hiddenWalls[i] = Instantiate(hiddenWallPrefab);
            hiddenWalls[i].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 透明な壁を設置
    /// </summary>
    private void SetHiddenWall()
    {
        // 床があるかチェック
        for (int i = 0; i < wallFlags.Length; i++)
        {
            Ray findGround = new Ray(new Vector3(transform.position.x, PlayerPositionY, transform.position.z) + rayPosition[i] * character.radius, Vector3.down);
            wallFlags[i] = Physics.Raycast(findGround, rayLength, layerMask);

            if (wallFlags[i] == false && hiddenWalls[i].gameObject.activeSelf == false)
            {
                // 透明な壁を設置
                hiddenWalls[i].transform.position = new Vector3(findGround.origin.x, PlayerPositionY, findGround.origin.z) + rayPosition[i] * (0.5f + 0.1f);
                hiddenWalls[i].gameObject.SetActive(true);
            }

            if(wallFlags[i] == false && hiddenWalls[i].gameObject.activeSelf)
            {
                if(i % 2 == 0)
                {
                    hiddenWalls[i].transform.position = new Vector3(transform.position.x, PlayerPositionY, hiddenWalls[i].transform.position.z);
                }
                else
                {
                    hiddenWalls[i].transform.position = new Vector3(hiddenWalls[i].transform.position.x, PlayerPositionY, transform.position.z);
                }
            }
            else
            {
                hiddenWalls[i].gameObject.SetActive(false);
            }
        }
    }
}