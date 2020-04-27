using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerType2 : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのCharacterController")] private CharacterController character = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;
    [SerializeField, Tooltip("透明な壁")] private BoxCollider hiddenWallPrefab = null;
    [SerializeField, Tooltip("移動時の起点カメラ")] private Camera playerCamera = null;
    [SerializeField, Tooltip("ステージの水オブジェクト")] private WaterHi stageWater = null;
    [SerializeField, Tooltip("PlayStateの設定")] private PlayState.GameMode mode = PlayState.GameMode.Stop;
    [SerializeField, Tooltip("PlayStateと同期させる")] private bool stateSet = true;

    // コントローラーの入力
    [SerializeField, Tooltip("X入力")] private float inputX = 0;
    [SerializeField, Tooltip("Z入力")] private float inputZ = 0;

    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 10)] private float playerSpeed = 0;
    [SerializeField, Header("プレイヤーの水中移動速度"), Range(0, 10)] private float playerWaterSpeed = 0;
    [SerializeField, Header("地面のLayerMask")] private LayerMask layerMask;
    [SerializeField, Header("水面のLayerMask")] private LayerMask waterLayerMask;
    [SerializeField, Header("Rayの長さ"), Range(0, 10)] private float rayLength = 0.5f;
    [SerializeField, Header("重力値"), Range(0, 10)] private float gravity = 10.0f;
    [SerializeField, Header("透明な壁のサイズ"), Range(0.01f, 5.0f)] private float wallSize = 1.0f;

    public Camera PlayerCamera { set { playerCamera = value; } }

    /// <summary>
    /// Stageの水オブジェクトをセットする
    /// </summary>
    public WaterHi StageWater { set { stageWater = value; } }

    // プレイヤーの位置(高さ)
    public float PlayerPositionY { private set; get; } = 0;

    // 透明な壁関連の変数
    private Vector3[] rayPosition = new Vector3[4] { Vector3.forward, Vector3.right, Vector3.back, Vector3.left };
    private BoxCollider[] hiddenWalls = null;

    // 水が腰の高さになったか
    private bool inWater = false;

    /// <summary>
    /// プレイヤーが水没したことを検知するフラグ
    /// </summary>
    public bool UnderWater { private set; get; } = false;

    /// <summary>
    /// 敵と接触した時のフラグ
    /// </summary>
    public bool ContactEnemy { set; get; } = false;

    /// <summary>
    /// 一方通行の崖を検知する用のフラグ
    /// </summary>
    public bool CliffFlag { set; private get; } = false;

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

    private void FixedUpdate()
    {
        PlayerMove(true);
    }

    private void Reset()
    {
        character = GetComponent<CharacterController>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void PlayerInit()
    {
        if (playerCamera == null) { playerCamera = Camera.main; }

        if (stageWater == null)
        {
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, 200, waterLayerMask))
            {
                stageWater = hit.transform.gameObject.GetComponent<WaterHi>();
            }
        }

        if(stateSet == false) { stateSet = true; }

        CreateHiddenWall();
    }

    /// <summary>
    /// コントローラ入力を取得
    /// </summary>
    private void GetInputController()
    {
        if (stateSet) { mode = PlayState.playState.gameMode; }
        
        // キー入力取得
        inputX = mode == PlayState.GameMode.Play && CliffFlag == false ? Input.GetAxis("Horizontal") : 0;
        inputZ = mode == PlayState.GameMode.Play && CliffFlag == false ? Input.GetAxis("Vertical") : 0;
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove(bool fixedUpdate)
    {
        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        inWater = stageWater != null && PlayerPositionY < stageWater.max;
        UnderWater = stageWater != null && PlayerPositionY + character.height * 0.5f < stageWater.max;

        // 移動方向
        Vector3 moveDirection = Vector3.zero;

        // プレイヤーのY座標の位置情報を更新
        PlayerPositionY = transform.position.y + character.center.y;

        // 入力の最低許容値
        float inputMin = 0.1f;
        bool input = Mathf.Abs(inputX) > inputMin || Mathf.Abs(inputZ) > inputMin;

        if (input)
        {
            // カメラの向いている方向を取得
            Vector3 cameraForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

            // プレイヤーカメラ起点の入力方向
            Vector3 direction = cameraForward * inputZ + playerCamera.transform.right * inputX;

            // 入力方向を向く処理
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
            rot = Quaternion.Slerp(transform.rotation, rot, 15 * delta);
            transform.rotation = rot;

            // 移動方向の決定
            float vec = Mathf.Abs(inputX) >= Mathf.Abs(inputZ) ? inputZ / inputX : inputX / inputZ;
            vec = 1.0f / Mathf.Sqrt(1.0f + vec * vec);
            moveDirection = direction * vec;

            // 床にRayを飛ばして斜面の角度を取得
            Ray ground = new Ray(new Vector3(transform.position.x, PlayerPositionY, transform.position.z), Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ground, out hit, rayLength, layerMask))
            {
                var nomal = hit.normal;
                Vector3 dir = moveDirection - Vector3.Dot(moveDirection, nomal) * nomal;
                moveDirection = dir.normalized;
            }

            // プレイヤーの移動先の算出
            float speed = inWater ? playerWaterSpeed : playerSpeed;
            moveDirection *= speed * delta;
        }

        // プレイヤーを移動させる
        moveDirection.y -= gravity * delta;
        character.Move(moveDirection);

        // 透明な壁の設置処理
        if(input) { SetHiddenWall(); }

        // アニメーション実行
        if (playerAnimator != null)
        {
            playerAnimator.SetBool("wate", input);
        }

        if (UnderWater)
        {
            Debug.Log("息苦しぃ...");
        }

        if (ContactEnemy)
        {
            Debug.Log("痛い...");
        }
    }

    /// <summary>
    /// 透明な壁を生成
    /// </summary>
    private void CreateHiddenWall()
    {
        hiddenWalls = new BoxCollider[rayPosition.Length];
        for(int i = 0; i < hiddenWalls.Length; i++)
        {
            hiddenWalls[i] = Instantiate(hiddenWallPrefab);
            hiddenWalls[i].enabled = false;
        }
    }

    /// <summary>
    /// 透明な壁を設置
    /// </summary>
    private void SetHiddenWall()
    {
        for (int i = 0; i < hiddenWalls.Length; i++)
        {
            // 床があるかチェック
            Ray findGround = new Ray(new Vector3(transform.position.x, PlayerPositionY, transform.position.z) + rayPosition[i] * character.radius, Vector3.down);
            bool go = Physics.Raycast(findGround, rayLength, layerMask);
            
            // 床が無ければ透明な壁を有効化する
            if (go == false && hiddenWalls[i].enabled == false)
            {
                hiddenWalls[i].size = Vector3.one * wallSize;
                hiddenWalls[i].transform.position = new Vector3(findGround.origin.x, PlayerPositionY, findGround.origin.z) + rayPosition[i] * (wallSize * 0.5f + 0.05f);
                hiddenWalls[i].enabled = true;
            }
            
            if(go == false && hiddenWalls[i].enabled)
            {
                // 透明な壁をプレイヤーの移動に合わせて移動させる
                if (i % 2 == 0)
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
                // 透明な壁を無効化する
                hiddenWalls[i].enabled = false;
            }
        }
    }
}