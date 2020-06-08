﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerType2 : MyAnimation
{
    [SerializeField, Tooltip("PlayerのCharacterController")] private CharacterController character = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;
    [SerializeField, Tooltip("Playerの傘のAnimator")] private Animator umbrellaAnimator = null;
    [SerializeField, Tooltip("透明な壁")] private BoxCollider hiddenWallPrefab = null;
    [SerializeField, Tooltip("地面のLayerMask")] private LayerMask groundLayer;
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

        // カメラの向いている方向を取得
        Vector3 cameraForward = Vector3.Scale(PlayerCamera.transform.forward == Vector3.up ? -PlayerCamera.transform.up : PlayerCamera.transform.forward == Vector3.down ? PlayerCamera.transform.up : PlayerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // カメラから見た入力方向を取得
        Vector3 direction = cameraForward * inputZ + PlayerCamera.transform.right * inputX;

        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        if (mode != PlayState.GameMode.Pause)
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
                // 移動方向
                Vector3 moveDirection = Vector3.zero;

                // 入力の最低許容値
                float inputMin = 0.1f;

                // 入力を検知したかチェック
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
                    Ray ground = new Ray(new Vector3(transform.position.x, transform.position.y + character.center.y, transform.position.z), Vector3.down);
                    float hitNomalY = 1.0f;
                    if(Physics.Raycast(ground, out RaycastHit hit, rayLength, groundLayer))
                    {
                        // 地面の傾斜を取得
                        hitNomalY = hit.normal.y;
                    }

                    // X：横軸の入力　Y：地面の傾斜　Z：縦軸の入力　の3次元空間上のベクトル
                    Vector3 vec = SquareToCircle(new Vector3(inputX, hitNomalY, inputZ));

                    // vecベクトルの大きさを三平方の定理を用いて算出する
                    float moveVec = Mathf.Sqrt((vec.x * vec.x) + (vec.y * vec.y) * (vec.z * vec.z));

                    // キャラクターの移動方向(ベクトル) = カメラの向いている方向(ベクトル) * vecベクトルの大きさ
                    moveDirection = direction * moveVec;

                    // 移動方向(ベクトル)を地面の傾斜に合わせて補正する
                    if(hitNomalY != 1.0f)
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

                // 重力を反映
                moveDirection.y -= gravity;

                // 実際にキャラクターを動かす
                character.Move(moveDirection * delta);

                // 透明な壁の設置
                if (input) { SetHiddenWall(); }

                // 水中フラグの設定
                if (StageWater != null)
                {
                    inWater = transform.position.y + character.center.y < StageWater.max;
                    UnderWater = transform.position.y + character.center.y + character.height * 0.5f < StageWater.max;
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
            if(playerAnimator != null)
            {
                // ポーズ中のみアニメーションを停止
                playerAnimator.enabled = false;
                if (umbrellaAnimator != null) { umbrellaAnimator.enabled = false; }
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
            int[] index = new int[2] { 0, 0 };
            mainRay = new Ray(new Vector3(transform.position.x, transform.position.y + character.center.y, transform.position.z) + rayPosition[i] * character.radius, Vector3.down);
            if(Physics.Raycast(mainRay, out hit, rayLength, groundLayer))
            {
                float hitDistance = hit.distance;
                // プレイヤーの当たり判定の両端からRayを飛ばして進めるかをチェック
                Ray subRay;
                bool check = false;
                for (int j = 0; j < index.Length; j++)
                {
                    subRay = new Ray(mainRay.origin + rayPosition[i + 1 < rayPosition.Length ? i + 1 : 0] * character.radius * (j == 0 ? 1 : -1), rayPosition[i]);
                    if (Physics.Raycast(subRay, out hit, 2.0f, groundLayer))
                    {
                        if(hit.normal.y != 0)
                        {
                            check = true;
                            break;
                        }
                    }
                }

                if (check)
                {
                    for (int j = 0; j < index.Length; j++)
                    {
                        subRay = new Ray(mainRay.origin + rayPosition[i + 1 < rayPosition.Length ? i + 1 : 0] * character.radius * (j == 0 ? 1 : -1), mainRay.direction);
                        if (Physics.Raycast(subRay, out hit, rayLength, groundLayer))
                        {
                            float disA = Mathf.Ceil(Mathf.Floor(hit.distance * 1000) / 10);
                            float disB = Mathf.Ceil(Mathf.Floor(hitDistance * 1000) / 10);
                            if (disA < disB)
                            {
                                index[j] = 1;
                            }
                            else
                            {
                                index[j] = 2;
                            }
                        }
                    }
                }

                int sum = 0;
                foreach (int k in index)
                {
                    sum += k;
                }

                set = sum == 3;
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
                    hiddenWalls[i].transform.position = new Vector3(transform.position.x, transform.position.y + character.center.y, hiddenWalls[i].transform.position.z);
                }
                else
                {
                    hiddenWalls[i].transform.position = new Vector3(hiddenWalls[i].transform.position.x, transform.position.y + character.center.y, transform.position.z);
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

    /// <summary>
    /// 斜め方向の入力座標を適正座標に変換する関数
    /// </summary>
    /// <param name="input">入力座標</param>
    /// <returns></returns>
    private Vector3 SquareToCircle(Vector3 input)
    {
        Vector3 output;
        output.x = input.x * Mathf.Sqrt(1.0f - ((input.y * input.y) * (input.z * input.z)) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1.0f - ((input.x * input.x) * (input.z * input.z)) / 2.0f);
        output.z = input.z * Mathf.Sqrt(1.0f - ((input.x * input.x) * (input.y * input.y)) / 2.0f);
        return output;
    }
}