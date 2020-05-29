using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerType2 : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのCharacterController")] private CharacterController character = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;
    [SerializeField, Tooltip("Playerの傘のAnimator")] private Animator umbrellaAnimator = null;
    [SerializeField, Tooltip("透明な壁")] private BoxCollider hiddenWallPrefab = null;
    [SerializeField, Tooltip("地面のLayerMask")] private LayerMask layerMask;
    [SerializeField, Tooltip("PlayStateの設定")] private PlayState.GameMode mode = PlayState.GameMode.Play;
    [SerializeField, Tooltip("AnimationEventスクリプト")] private PlayerAnimeEvent animeEvent = null;
    private bool connectPlayState = false;

    // コントローラーの入力
    private float inputX = 0;
    private float inputZ = 0;
    private bool dontInput = false; // 操作入力を無効にするフラグ

    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 10)] private float playerSpeed = 5;
    [SerializeField, Header("プレイヤーの水中移動速度"), Range(0, 10)] private float playerWaterSpeed = 2.5f;
    [SerializeField, Header("プレイヤーの加速度グラフ")] private AnimationCurve curve = null;
    [SerializeField, Header("最高速度到達時間"), Range(0.1f, 2.0f)] private float maxSpeedTime = 0.5f;
    [SerializeField, Header("Rayの長さ"), Range(0, 10)] private float rayLength = 0.5f;
    [SerializeField, Header("重力値"), Range(0, 10)] private float gravity = 10.0f;
    [SerializeField, Header("透明な壁のサイズ"), Range(0.01f, 5.0f)] private float wallSize = 1.0f;

    /// <summary>
    /// プレイヤーカメラ
    /// </summary>
    public Camera PlayerCamera { set; private get; } = null;

    /// <summary>
    /// Stageの水オブジェクト
    /// </summary>
    public WaterHi StageWater { set; private get; } = null;

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
        character = GetComponent<CharacterController>();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void PlayerInit()
    {
        if (PlayerCamera == null) { PlayerCamera = Camera.main; }

        connectPlayState = GetPlayState();

        PlayerPositionY = transform.position.y + character.center.y;

        CreateHiddenWall();
    }

    /// <summary>
    /// コントローラ入力を取得
    /// </summary>
    private void GetInputController()
    {
        // キー入力取得
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
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
        else
        {
            mode = PlayState.GameMode.Play;
        }

        if (mode == PlayState.GameMode.Play || mode == PlayState.GameMode.Rain)
        {
            bool input;
            float inputSpeed = (Mathf.Abs(inputX) + Mathf.Abs(inputZ)) * 0.5f < 0.5f ? Mathf.Abs(inputX) + Mathf.Abs(inputZ) : 1.0f;

            // 一方通行の崖を利用する際に実行
            if (CliffFlag)
            {
                foreach (var wall in hiddenWalls)
                {
                    wall.enabled = false;
                }
                input = false;
            }
            else
            {
                float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

                // 移動方向
                Vector3 moveDirection = Vector3.zero;

                // 入力の最低許容値
                float inputMin = 0.1f;
                input = (Mathf.Abs(inputX) > inputMin || Mathf.Abs(inputZ) > inputMin) && mode == PlayState.GameMode.Play && dontInput == false;

                if (input)
                {
                    // カメラの向いている方向を取得
                    Vector3 cameraForward = Vector3.Scale(PlayerCamera.transform.forward == Vector3.up ? -PlayerCamera.transform.up : PlayerCamera.transform.forward == Vector3.down ? PlayerCamera.transform.up : PlayerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

                    // プレイヤーカメラ起点の入力方向
                    Vector3 direction = cameraForward * inputZ + PlayerCamera.transform.right * inputX;

                    // 入力方向を向く処理
                    Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
                    rot = Quaternion.Slerp(transform.rotation, rot, 7.5f * delta);
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
                    if(speedTime < maxSpeedTime)
                    {
                        speedTime += delta;
                    }
                    else
                    {
                        speedTime = maxSpeedTime;
                    }
                    moveDirection *= speed * delta * inputSpeed * curve.Evaluate(speedTime / maxSpeedTime);
                }
                else
                {
                    speedTime = 0;
                }

                // プレイヤーを移動させる
                moveDirection.y -= gravity * delta;
                character.Move(moveDirection);

                // プレイヤーのY座標の位置情報を更新
                PlayerPositionY = transform.position.y + character.center.y;

                // 透明な壁の設置
                if (input) { SetHiddenWall(); }

                // 水中フラグの設定
                if (StageWater != null)
                {
                    inWater = PlayerPositionY < StageWater.max;
                    UnderWater = PlayerPositionY + character.height * 0.5f < StageWater.max;
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
                playerAnimator.SetBool("Run", input);
                playerAnimator.SetFloat("Speed", inWater ? (inputSpeed * curve.Evaluate(speedTime / maxSpeedTime)) / (playerSpeed / playerWaterSpeed) : inputSpeed * curve.Evaluate(speedTime / maxSpeedTime));
                playerAnimator.SetBool("Switch", mode == PlayState.GameMode.Rain);
                playerAnimator.SetBool("Jump", CliffFlag);
            }
        }
        else
        {
            // アニメーションの停止
            if(playerAnimator != null)
            {
                if(mode == PlayState.GameMode.Pause)
                {
                    playerAnimator.enabled = false;
                }
                else
                {
                    playerAnimator.enabled = true;
                    playerAnimator.SetBool("StageClear", mode == PlayState.GameMode.Clear);
                    playerAnimator.SetBool("GameOver", mode == PlayState.GameMode.GameOver);
                }
            }
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
            Ray mainRay;
            RaycastHit hit;
            bool set;
            int count = 0;
            mainRay = new Ray(new Vector3(transform.position.x, PlayerPositionY, transform.position.z) + rayPosition[i] * character.radius, Vector3.down);
            if(Physics.Raycast(mainRay, out hit, rayLength, layerMask))
            {
                if(Vector3.Angle(Vector3.up, hit.normal) <= 5)
                {
                    // プレイヤーの当たり判定の両端からRayを飛ばして進めるかをチェック
                    Ray subRay;
                    float hitDistance = hit.distance;
                    for (int j = 0; j < 2; j++)
                    {
                        subRay = new Ray(mainRay.origin + rayPosition[i + 1 < rayPosition.Length ? i + 1 : 0] * (character.radius - 0.0001f) * (j == 0 ? 1 : -1), mainRay.direction);
                        Debug.DrawRay(subRay.origin, subRay.direction);
                        if (Physics.Raycast(subRay, out hit, rayLength, layerMask))
                        {
                            if(Vector3.Angle(Vector3.up, hit.normal) <= 5 && hit.distance < hitDistance)
                            {
                                count++;
                            }
                        }
                        else
                        {
                            count--;
                            break;
                        }
                    }
                }

                set = count == 1;

                if(set == false)
                {
                    Ray slopeRay;
                    count = 0;
                    for (int j = 0; j < 2; j++)
                    {
                        slopeRay = new Ray(new Vector3(transform.position.x, PlayerPositionY, transform.position.z) + rayPosition[i + 1 < rayPosition.Length ? i + 1 : 0] * (j == 0 ? 0.025f : -0.025f), rayPosition[i]);
                        if (Physics.Raycast(slopeRay, out hit, character.radius + 1.0f, layerMask))
                        {
                            if (Vector3.Angle(Vector3.up, hit.normal) < character.slopeLimit)
                            {
                                count++;
                            }
                        }
                    }
                    set = count == 1;
                }
            }
            else
            {
                set = true;
            }

            // 床が無ければ透明な壁を有効化する
            if (set && hiddenWalls[i].enabled == false)
            {
                hiddenWalls[i].size = Vector3.one * wallSize;
                hiddenWalls[i].transform.position = mainRay.origin + rayPosition[i] * wallSize * 0.5001f;
                hiddenWalls[i].enabled = true;
            }
            
            if(set && hiddenWalls[i].enabled)
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
    /// 敵と接触しているときに呼び出す処理
    /// </summary>
    /// <param name="flag">条件式</param>
    public void HitEnemy(bool flag)
    {
        if(connectPlayState == false) { return; }

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
}