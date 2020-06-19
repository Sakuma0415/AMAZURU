using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerNav : MyAnimation
{
    [SerializeField, Tooltip("NavMeshSurfaceのPrefab")] private NavMeshSurface surfacePrefab = null;
    private NavMeshSurface playerNav = null;
    [SerializeField, Tooltip("NavMeshAgent")] private NavMeshAgent playerAgent = null;
    [SerializeField, Tooltip("Rigidbody")] private Rigidbody playerRigid = null;
    [SerializeField, Tooltip("Collider")] private CapsuleCollider playerCollider = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;
    [SerializeField, Tooltip("Playerの傘のAnimator")] private Animator umbrellaAnimator = null;
    [SerializeField, Tooltip("地面のLayerMask")] private LayerMask groundLayer;
    [SerializeField, Tooltip("PlayStateの設定")] private PlayState.GameMode mode = PlayState.GameMode.Play;
    [SerializeField, Tooltip("AnimationEventスクリプト")] private PlayerAnimeEvent animeEvent = null;
    private bool connectPlayState = false;
    private bool connectControllerInput = false;

    // コントローラーの入力
    private float inputX = 0;
    private float inputZ = 0;
    private bool dontInput = false; // 操作入力を無効にするフラグ

    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 10)] private float playerSpeed = 5;
    [SerializeField, Header("プレイヤーの水中移動速度"), Range(0, 10)] private float playerWaterSpeed = 2.5f;
    [SerializeField, Header("プレイヤーの加速度グラフ")] private AnimationCurve curve = null;
    [SerializeField, Header("最高速度到達時間"), Range(0.1f, 2.0f)] private float maxSpeedTime = 0.5f;
    [SerializeField, Header("Rayの長さ"), Range(0, 10)] private float rayLength = 0.5f;

    [SerializeField, Header("NavMesh関連の設定項目"), Tooltip("NavMesh用のLayerMask")] private LayerMask navLayerMask;
    private bool navMeshFlag = false;
    private bool specialMove = false;

    /// <summary>
    /// プレイヤーカメラ
    /// </summary>
    public Camera PlayerCamera { set; private get; } = null;

    /// <summary>
    /// Stageの水オブジェクト
    /// </summary>
    public WaterHi StageWater { set; private get; } = null;

    // 水が腰の高さになったか
    private bool inWater = false;

    /// <summary>
    /// プレイヤーが水没したことを検知するフラグ
    /// </summary>
    public bool UnderWater { private set; get; } = false;

    /// <summary>
    /// 敵と接触した時のフラグ
    /// </summary>
    public bool ContactEnemy { private set; get; } = false;

    /// <summary>
    /// 一方通行の崖を検知する用のフラグ
    /// </summary>
    public bool CliffFlag { set; private get; } = false;

    // プレイヤーが動き始めてからの経過時間
    private float speedTime = 0;

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
        playerAgent = GetComponent<NavMeshAgent>();
        playerRigid = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void PlayerInit()
    {
        if (PlayerCamera == null) { PlayerCamera = Camera.main; }

        connectPlayState = GetPlayState();

        connectControllerInput = GetControllerInput();

        BakeNavMesh();
    }

    /// <summary>
    /// コントローラ入力を取得
    /// </summary>
    private void GetInputController()
    {
        // キー入力取得
        inputX = connectControllerInput ? ControllerInput.Instance.stick.LStickHorizontal : Input.GetAxis("Horizontal");
        inputZ = connectControllerInput ? ControllerInput.Instance.stick.LStickVertical : Input.GetAxis("Vertical");
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove(bool fixedUpdate)
    {
        if (connectPlayState)
        {
            mode = PlayState.playState.gameMode;
        }

        // カメラの向いている方向を取得
        Vector3 cameraForward = Vector3.Scale(PlayerCamera.transform.forward == Vector3.up ? -PlayerCamera.transform.up : PlayerCamera.transform.forward == Vector3.down ? PlayerCamera.transform.up : PlayerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // カメラから見た入力方向を取得
        Vector3 direction = cameraForward * inputZ + PlayerCamera.transform.right * inputX;

        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        if (mode != PlayState.GameMode.Pause)
        {
            bool input;
            float inputSpeed = Mathf.Sqrt((inputX * inputX) + (inputZ * inputZ));

            // NavMeshの更新
            if (mode == PlayState.GameMode.Rain)
            {
                navMeshFlag = true;
                specialMove = true;
                playerAgent.updatePosition = false;
                playerRigid.isKinematic = false;
                
            }
            else
            {
                // ナビメッシュの更新をかける
                if (navMeshFlag)
                {
                    navMeshFlag = false;
                    UpdateNavMesh();
                }

                // アメフラシ起動またはジャンプの動作が終了したら座標を更新する
                if (specialMove && CliffFlag == false)
                {
                    specialMove = false;
                    playerAgent.Warp(transform.position);
                    playerAgent.updatePosition = true;
                    playerRigid.isKinematic = true;
                }
            }

            // 一方通行の崖を利用する際に実行
            if (CliffFlag)
            {
                specialMove = true;
                playerAgent.updatePosition = false;
                input = false;
            }
            else
            {
                // 移動方向
                Vector3 moveDirection = Vector3.zero;

                // 入力の最低許容値
                float inputMin = 0.1f;
                input = (Mathf.Abs(inputX) > inputMin || Mathf.Abs(inputZ) > inputMin) && mode == PlayState.GameMode.Play && dontInput == false;

                if (input)
                {
                    // 入力方向を向く処理
                    Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
                    rot = Quaternion.Slerp(transform.rotation, rot, 7.5f * delta);
                    transform.rotation = rot;

                    // 水中かどうかをチェックし、加速度グラフに基づいた移動速度を計算
                    float speed = inWater ? playerWaterSpeed : playerSpeed;
                    if (speedTime < maxSpeedTime)
                    {
                        speedTime += delta;
                    }
                    else
                    {
                        speedTime = maxSpeedTime;
                    }

                    // 地面にRayを飛ばす
                    Ray ground = new Ray(new Vector3(transform.position.x, transform.position.y + playerCollider.center.y, transform.position.z), Vector3.down);
                    float hitNomalY = 1.0f;
                    if (Physics.Raycast(ground, out RaycastHit hit, rayLength, groundLayer))
                    {
                        // 地面の傾斜を取得
                        hitNomalY = hit.normal.y;
                    }

                    // 斜め入力時の移動量を修正
                    moveDirection = direction.normalized;

                    // 坂を移動する際の傾斜を考慮した移動量に修正
                    if (hitNomalY != 1.0f)
                    {
                        var nomal = hit.normal;
                        Vector3 dir = moveDirection - Vector3.Dot(moveDirection, nomal) * nomal;
                        moveDirection = dir.normalized;
                    }

                    // 移動量にスピード値を乗算
                    moveDirection *= speed * inputSpeed * curve.Evaluate(speedTime / maxSpeedTime);
                }
                else
                {
                    speedTime = 0;
                }

                // プレイヤーを移動させる
                if (playerNav != null && playerAgent.updatePosition)
                {
                    playerAgent.Move(moveDirection * delta);
                }

                // 水中フラグの設定
                if (StageWater != null)
                {
                    inWater = (transform.position.y + playerCollider.center.y) - (playerCollider.height * 0.25f) < StageWater.max;
                    UnderWater = transform.position.y + playerCollider.center.y + playerCollider.height * 0.25f < StageWater.max;
                }
                else
                {
                    inWater = false;
                    UnderWater = false;
                }

                // AnimationEventの設定
                if (animeEvent != null)
                {
                    if (UnderWater)
                    {
                        animeEvent.PlayerStepMode = StepMode.UnderWater;
                    }
                    else if (inWater)
                    {
                        animeEvent.PlayerStepMode = StepMode.InWater;
                    }
                    else
                    {
                        animeEvent.PlayerStepMode = StepMode.Nomal;
                    }
                    animeEvent.PlayerPosition = transform.position;
                }
            }

            // アニメーション実行
            if (playerAnimator != null)
            {
                playerAnimator.enabled = true;
                if (umbrellaAnimator != null) { umbrellaAnimator.enabled = true; }

                // 走るアニメーション
                playerAnimator.SetBool("Run", input);
                playerAnimator.SetFloat("Speed", inWater ? (inputSpeed * curve.Evaluate(speedTime / maxSpeedTime)) / (playerSpeed / playerWaterSpeed) : inputSpeed * curve.Evaluate(speedTime / maxSpeedTime));

                // アメフラシを起動するアニメーション
                playerAnimator.SetBool("Switch", mode == PlayState.GameMode.Rain);

                // 崖から降りるアニメーション
                playerAnimator.SetBool("Jump", CliffFlag);

                // ゲームオーバー時のアニメーション
                playerAnimator.SetBool("GameOver", mode == PlayState.GameMode.GameOver);

                // クリア時のアニメーションを再生
                if (mode == PlayState.GameMode.Clear)
                {
                    if (RotateAnimation(transform.gameObject, cameraForward * -1, 360 * delta, true))
                    {
                        playerAnimator.SetBool("Run", false);
                        playerAnimator.SetBool("StageClear", true);
                    }
                }
            }
        }
        else
        {
            // アニメーションの停止
            if (playerAnimator != null)
            {
                // ポーズ中のみアニメーションを停止
                playerAnimator.enabled = false;
                if (umbrellaAnimator != null) { umbrellaAnimator.enabled = false; }
            }
        }
    }

    /// <summary>
    /// PlayStateを取得できるかチェックする
    /// </summary>
    private bool GetPlayState()
    {
        try
        {
            var state = PlayState.playState.gameMode;
            return true;
        }
        catch (System.NullReferenceException)
        {
            return false;
        }
    }

    /// <summary>
    /// ControllerInputにアクセスできるかチェック
    /// </summary>
    /// <returns></returns>
    private bool GetControllerInput()
    {
        try
        {
            var input = ControllerInput.Instance.stick;
            return true;
        }
        catch (System.NullReferenceException)
        {
            return false;
        }
    }

    /// <summary>
    /// 敵と接触しているときに呼び出す処理
    /// </summary>
    /// <param name="flag">条件式</param>
    public void HitEnemy(bool flag)
    {
        if (connectPlayState == false) { return; }

        if (flag)
        {
            // ゲームオーバー処理
            ContactEnemy = true;
        }
        else
        {
            // 敵と接触中は操作ができないようにする
            dontInput = true;
        }
    }

    /// <summary>
    /// NavMeshSurfaceを取得する関数
    /// </summary>
    private void BakeNavMesh()
    {
        if(playerNav == null)
        {
            if(surfacePrefab == null) 
            { 
                Debug.LogError("NavMeshSurfaceを設定してください");
                return;
            }
            else
            {
                playerNav = Instantiate(surfacePrefab, Vector3.zero, Quaternion.identity);
            }
        }
        playerNav.layerMask = navLayerMask;
        playerNav.BuildNavMesh();
        playerAgent.enabled = true;
        playerAgent.agentTypeID = playerNav.agentTypeID;
        navMeshFlag = true;
    }

    /// <summary>
    /// NavMeshのデータを更新する関数
    /// </summary>
    private void UpdateNavMesh()
    {
        if (playerNav == null) { return; }
        playerNav.layerMask = navLayerMask;
        playerNav.UpdateNavMesh(playerNav.navMeshData);
    }
}
