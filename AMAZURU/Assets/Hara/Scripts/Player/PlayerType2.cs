using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerType2 : MyAnimation
{
    [SerializeField, Tooltip("PlayerのCharacterController")] private CharacterController character = null;
    [SerializeField, Tooltip("PlayerのAnimator")] private Animator playerAnimator = null;
    [SerializeField, Tooltip("Playerの傘のAnimator")] private Animator umbrellaAnimator = null;
    [SerializeField, Tooltip("地面のLayerMask")] private LayerMask groundLayer;
    [SerializeField, Tooltip("AnimationEventスクリプト")] private PlayerAnimeEvent animeEvent = null;
    [SerializeField, Tooltip("感電エフェクト")] private ParticleSystem electroParticle = null;

    // コントローラーの入力
    private float inputX = 0;
    private float inputZ = 0;

    /// <summary>
    /// 入力操作を無効にするフラグ
    /// </summary>
    public bool DontInput { set; private get; } = false;

    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 10)] private float playerSpeed = 5;
    [SerializeField, Header("プレイヤーの水中移動速度"), Range(0, 10)] private float playerWaterSpeed = 2.5f;
    [SerializeField, Header("プレイヤーの加速度グラフ")] private AnimationCurve curve = null;
    [SerializeField, Header("最高速度到達時間"), Range(0.1f, 2.0f)] private float maxSpeedTime = 0.5f;

    // プレイヤーの進行方向ベクトル
    private Vector3 playerMoveDirection = Vector3.zero;

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

    // プレイヤーが坂道に立っているときのフラグ
    private bool isOnSlope = false;
    private Vector3 slopeRight = Vector3.zero;

    /// <summary>
    /// プレイヤーの水中フラグ
    /// </summary>
    public bool InWater { private set; get; } = false;

    /// <summary>
    /// プレイヤーが水没したことを検知するフラグ
    /// </summary>
    public bool UnderWater { private set; get; } = false;

    /// <summary>
    /// 一方通行の崖を検知する用のフラグ
    /// </summary>
    public bool CliffFlag { set; private get; } = false;

    /// <summary>
    /// ゲーム停止中のフラグ
    /// </summary>
    public bool IsGameStop { set; private get; } = false;

    /// <summary>
    /// アメフラシの起動フラグ
    /// </summary>
    public bool IsRain { set; private get; } = false;

    /// <summary>
    /// ゲームクリア時のフラグ
    /// </summary>
    public bool IsGameClear { set; private get; } = false;

    /// <summary>
    /// ゲームオーバー時のフラグ
    /// </summary>
    public bool IsGameOver { set; private get; } = false;

    /// <summary>
    /// 水中時のゲームオーバーフラグ
    /// </summary>
    public bool IsGameOverInWater { set; private get; } = false;

    /// <summary>
    /// エネミーとの接触フラグ
    /// </summary>
    public bool IsHitEnemy { set; get; } = false;

    /// <summary>
    /// 感電時のフラグ
    /// </summary>
    public bool IsElectric { set; private get; } = false;

    /// <summary>
    /// CharacterControllerの移動のみを無効にするフラグ(アニメーションは適用されます)
    /// </summary>
    public bool IsDontCharacterMove { set; private get; } = false;

    /// <summary>
    /// 透明壁を無効にする
    /// </summary>
    public bool IsDontShield { set; private get; } = false;

    private Coroutine windCoroutine = null;

    // 入力受付を無効にするフラグ
    private bool stopInput = false;

    // プレイヤーが動き始めてからの経過時間
    private float speedTime = 0;

    // 特殊な地形用の壁オブジェクト
    private GameObject[] prismWallObjects = null;
    private GameObject[] prismGroundObjects = null;
    private int prismWallActiveCount = 0;

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
        if (IsGameStop == false)
        {
            // プレイヤーの移動処理
            PlayerMove(true);
        }
        else
        {
            if (playerAnimator != null)
            {
                // ポーズ中のみアニメーションを停止
                playerAnimator.enabled = false;
                if (umbrellaAnimator != null) { umbrellaAnimator.enabled = false; }
            }
        }
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
        CreateHiddenWall();
    }

    /// <summary>
    /// コントローラ入力を取得
    /// </summary>
    private void GetInputController()
    {
        // キー入力取得
        try
        {
            inputX = ControllerInput.Instance.stick.LStickHorizontal;
            inputZ = ControllerInput.Instance.stick.LStickVertical;
        }
        catch (System.NullReferenceException)
        {
            inputX = Input.GetAxis("Horizontal");
            inputZ = Input.GetAxis("Vertical");
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove(bool fixedUpdate)
    {
        // カメラの向いている方向を取得
        Vector3 cameraForward = Vector3.Scale(PlayerCamera.transform.forward == Vector3.up ? -PlayerCamera.transform.up : PlayerCamera.transform.forward == Vector3.down ? PlayerCamera.transform.up : PlayerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // カメラから見た入力方向を取得
        Vector3 direction = cameraForward * inputZ + PlayerCamera.transform.right * inputX;

        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        bool input = false;
        float inputSpeed = Mathf.Sqrt((inputX * inputX) + (inputZ * inputZ));

        if (IsDontCharacterMove || CliffFlag)
        {
            DontHiddenWall();
        }
        else
        {
            // 移動方向
            if(stopInput == false)
            {
                playerMoveDirection = Vector3.zero;
            }

            // 入力の最低許容値
            float inputMin = 0.1f;

            // 入力を検知したかチェック
            input = (Mathf.Abs(inputX) > inputMin || Mathf.Abs(inputZ) > inputMin) && DontInput == false && stopInput == false;

            if (input)
            {
                // 入力方向を向く処理
                Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
                rot = Quaternion.Slerp(transform.rotation, rot, 7.5f * delta);
                transform.rotation = rot;

                // 水中かどうかをチェックし、加速度グラフに基づいた移動速度を計算
                float speed = InWater ? playerWaterSpeed : playerSpeed;
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
                slopeRight = Vector3.zero;
                if (Physics.Raycast(ground, out RaycastHit hit, character.height, groundLayer))
                {
                    // 地面の傾斜を取得
                    hitNomalY = hit.normal.y;
                    isOnSlope = hitNomalY < 1.0f;

                    if (isOnSlope)
                    {
                        slopeRight = GetSlopeVec(hit.transform.right);
                    }
                }
                else
                {
                    isOnSlope = false;
                }

                SetPrismWall();

                // 斜め入力時の移動量を修正
                playerMoveDirection = direction.normalized;

                // 坂を移動する際の傾斜を考慮した移動量に修正
                if (hitNomalY != 1.0f)
                {
                    var nomal = hit.normal;
                    Vector3 dir = playerMoveDirection - Vector3.Dot(playerMoveDirection, nomal) * nomal;
                    playerMoveDirection = dir.normalized;
                }

                // 移動量にスピード値を乗算
                playerMoveDirection *= speed * inputSpeed * curve.Evaluate(speedTime / maxSpeedTime);
            }
            else
            {
                speedTime = 0;
            }

            // 重力を反映
            playerMoveDirection.y -= 10.0f;

            // 実際にキャラクターを動かす
            character.Move(playerMoveDirection * delta);

            // 透明な壁の設置
            if(IsDontShield == false)
            {
                SetHiddenWall();
            }
            else
            {
                DontHiddenWall();
            }

            // 水中フラグの設定
            if (StageWater != null)
            {
                InWater = transform.position.y + character.center.y - character.height * 0.25f < StageWater.max;
                UnderWater = transform.position.y + character.center.y + character.height * 0.25f < StageWater.max;
            }
            else
            {
                InWater = false;
                UnderWater = false;
            }

            // AnimationEventの設定
            if (animeEvent != null)
            {
                if (UnderWater)
                {
                    animeEvent.PlayerStepMode = StepMode.UnderWater;
                }
                else if (InWater)
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
            playerAnimator.SetFloat("Speed", InWater ? (inputSpeed * curve.Evaluate(speedTime / maxSpeedTime)) / (playerSpeed / playerWaterSpeed) : inputSpeed * curve.Evaluate(speedTime / maxSpeedTime));

            // アメフラシを起動するアニメーション
            playerAnimator.SetBool("Switch", IsRain);

            // 崖から降りるアニメーション
            playerAnimator.SetBool("Jump", CliffFlag);

            // ゲームオーバー時のアニメーション
            playerAnimator.SetBool("GameOver", IsGameOver);

            // 水中時のゲームオーバーアニメーション
            playerAnimator.SetBool("GameOverInWater", IsGameOverInWater);

            // クリア時のアニメーションを再生
            if (IsGameClear)
            {
                if (RotateAnimation(transform.gameObject, cameraForward * -1, Vector3.up, 360 * delta, true))
                {
                    playerAnimator.SetBool("Run", false);
                    playerAnimator.SetBool("StageClear", true);
                }
            }
        }

        // 感電エフェクトの再生
        ElectroEffect(IsElectric);
    }

    /// <summary>
    /// 透明な壁を生成
    /// </summary>
    private void CreateHiddenWall()
    {
        hiddenWalls = new BoxCollider[rayPosition.Length];
        for(int i = 0; i < hiddenWalls.Length; i++)
        {
            GameObject colObj = new GameObject();
            hiddenWalls[i] = colObj.AddComponent<BoxCollider>();
            hiddenWalls[i].gameObject.name = "WallObject" + i.ToString();
            hiddenWalls[i].gameObject.layer = LayerMask.NameToLayer("Stage");
            hiddenWalls[i].size = Vector3.one;
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
            bool rayDirectionFlag = rayPosition[i] == slopeRight || rayPosition[i] == -slopeRight;
            float rayRange = isOnSlope && rayDirectionFlag ? character.height * 0.6f : character.height;
            Vector3 baseRayPosition = new Vector3(transform.position.x, transform.position.y + character.center.y, transform.position.z);
            mainRay = new Ray(baseRayPosition + rayPosition[i] * character.radius, Vector3.down);
            if(Physics.Raycast(mainRay, out hit, rayRange, groundLayer) && hit.collider.isTrigger == false)
            {
                float hitDistance = hit.distance;
                // プレイヤーの当たり判定の両端からRayを飛ばして進めるかをチェック
                bool isHitSlope = Mathf.Floor(Mathf.Abs(hit.normal.y) * 10) / 10 < 0.8f;
                bool flag = false;

                if (isHitSlope)
                {
                    Vector3 hitVec = GetSlopeVec(hit.transform.forward);
                    for(int j = 0; j < 2; j++)
                    {
                        int index = j == 0 ? (i - 1 < 0 ? rayPosition.Length - 1 : i - 1) : (i + 1 >= rayPosition.Length ? 0 : i + 1);
                        if(Physics.Raycast(new Ray(baseRayPosition + rayPosition[index] * character.radius, Vector3.down), out hit, rayRange, groundLayer) && hit.collider.isTrigger == false)
                        {
                            if (GetSlopeVec(hit.transform.forward) != hitVec && Mathf.Floor(Mathf.Abs(hit.normal.y) * 10) / 10 < 0.8f)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }

                if(flag == false)
                {
                    Ray subRay;
                    int count = 0;
                    for (int j = 0; j < 2; j++)
                    {
                        subRay = new Ray(baseRayPosition + Vector3.down * character.height * (isOnSlope ? 0.525f : 0.475f) + rayPosition[i + 1 < rayPosition.Length ? i + 1 : 0] * character.radius * (j == 0 ? 1 : -1), rayPosition[i]);
                        if (Physics.Raycast(subRay, out hit, character.radius * 1.5f, groundLayer) && hit.collider.isTrigger == false)
                        {
                            Vector3 hitNormal = hit.normal;
                            hitNormal.x = Mathf.Floor(Mathf.Abs(hitNormal.x) * 10) / 10;
                            hitNormal.y = Mathf.Floor(Mathf.Abs(hitNormal.y) * 10) / 10;
                            hitNormal.z = Mathf.Floor(Mathf.Abs(hitNormal.z) * 10) / 10;

                            if (isOnSlope)
                            {
                                if (hitNormal.y < 0.1f)
                                {
                                    count++;
                                }
                            }
                            else
                            {
                                if (hitNormal.y != 0)
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                    }

                    bool isSetSlopeWall = isOnSlope && count == 2;

                    if (flag || isHitSlope)
                    {
                        count = 0;
                        for (int j = 0; j < 2; j++)
                        {
                            subRay = new Ray(mainRay.origin + rayPosition[i + 1 < rayPosition.Length ? i + 1 : 0] * character.radius * (j == 0 ? 1 : -1), mainRay.direction);
                            if (Physics.Raycast(subRay, out hit, character.height, groundLayer) && hit.collider.isTrigger == false)
                            {
                                float disA = Mathf.Ceil(Mathf.Floor(hit.distance * 1000) / 10);
                                float disB = Mathf.Ceil(Mathf.Floor(hitDistance * 1000) / 10);

                                if (disA != disB)
                                {
                                    count++;
                                    break;
                                }
                            }
                        }

                        set = isOnSlope ? isSetSlopeWall && count > 0 : count > 0;
                    }
                    else
                    {
                        set = isOnSlope ? isSetSlopeWall : count > 0;
                    }
                }
                else
                {
                    set = true;
                }
            }
            else
            {
                set = true;
            }

            // 床が無ければ透明な壁を有効化する
            if (set && hiddenWalls[i].enabled == false)
            {
                hiddenWalls[i].transform.position = mainRay.origin + rayPosition[i] * 0.5001f;
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
    /// 風に吹き飛ばされる処理のコルーチン
    /// </summary>
    /// <param name="direction">吹き飛ばす方向</param>
    /// <param name="target">吹き飛ばし先の地点</param>
    /// <param name="speed">吹き飛ばし速度</param>
    /// <returns></returns>
    private IEnumerator WindActionCoroutine(Vector3 direction, Vector3 target, float speed)
    {
        stopInput = true;
        playerMoveDirection = direction * speed;

        IsDontCharacterMove = true;
        Vector3 playerPos = transform.position;
        yield return null;
        IsDontCharacterMove = false;

        while (Vector3.Distance(playerPos, target) > 0.1f)
        {
            if (IsGameStop == false)
            {
                if (IsGameClear || IsGameOver || IsHitEnemy)
                {
                    stopInput = false;
                    yield break;
                }
                playerPos = Vector3.MoveTowards(playerPos, target, speed * Time.deltaTime);
                transform.Rotate(new Vector3(0, 15, 0));
            }
            yield return null;
        }
        stopInput = false;
        windCoroutine = null;
    }

    /// <summary>
    /// 風に吹き飛ばされる処理
    /// </summary>
    /// <param name="direction">吹き飛ばす方向</param>
    /// <param name="target">吹き飛ばし先の地点</param>
    /// <param name="speed">吹き飛ばし速度</param>
    /// <returns></returns>
    public void WindAction(Vector3 direction, Vector3 target, float speed)
    {
        if(windCoroutine != null)
        {
            StopCoroutine(windCoroutine);
        }
        windCoroutine = StartCoroutine(WindActionCoroutine(direction, target, speed));
    }

    /// <summary>
    /// 透明壁を無効にする処理
    /// </summary>
    private void DontHiddenWall()
    {
        foreach (var wall in hiddenWalls)
        {
            wall.enabled = false;
        }
    }

    /// <summary>
    /// 坂のベクトルを取得
    /// </summary>
    /// <param name="slopeVector">坂の元ベクトル</param>
    /// <returns></returns>
    private Vector3 GetSlopeVec(Vector3 slopeVector)
    {
        slopeVector.x = Mathf.Floor(Mathf.Abs(slopeVector.x) * 10) != 0 ? 1 : 0;
        slopeVector.y = 0;
        slopeVector.z = Mathf.Floor(Mathf.Abs(slopeVector.z) * 10) != 0 ? 1 : 0;
        return slopeVector;
    }

    /// <summary>
    /// Prism用の壁を設置する処理
    /// </summary>
    private void SetPrismWall()
    {
        if(prismGroundObjects == null || prismGroundObjects.Length < 1)
        {
            prismGroundObjects = new GameObject[rayPosition.Length];
        }

        for (int i = 0; i < prismGroundObjects.Length; i++)
        {
            int next = i + 1 > rayPosition.Length - 1 ? 0 : i + 1;
            Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + character.center.y, transform.position.z) + rayPosition[i] * character.radius * 1.2f + rayPosition[next] * character.radius * 1.2f, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, character.height, groundLayer))
            {
                string objName = hit.transform.gameObject.name;
                bool prismWallFlag = (objName.Contains("prism") || objName.Contains("Prism")) && (Vector3.Distance(hit.transform.right, ray.direction) < 0.1f || Vector3.Distance(-hit.transform.right, ray.direction) < 0.1f);
                if (prismWallFlag) { prismGroundObjects[i] = hit.transform.gameObject; }
                else { prismGroundObjects[i] = null; }
            }
            else
            {
                prismGroundObjects[i] = null;
            }
        }

        // Prism用の壁を作成
        GameObject[] prismWalls = prismGroundObjects.Distinct().ToArray();
        List<GameObject> list = new List<GameObject>(prismWalls);
        list.RemoveAll(item => item == null);
        prismWalls = list.ToArray();
        
        if((prismWallObjects == null || prismWallObjects.Length < 1) && prismWalls.Length > 0)
        {
            prismWallObjects = new GameObject[prismGroundObjects.Length];
            for(int i = 0; i < prismWallObjects.Length; i++)
            {
                prismWallObjects[i] = Instantiate(prismWalls[0]);
                MeshRenderer mr = prismWallObjects[i].GetComponent<MeshRenderer>();
                if(mr != null) { Destroy(mr); }
                MyCellIndex cell = prismWallObjects[i].GetComponent<MyCellIndex>();
                if(cell != null) { Destroy(cell); }
                prismWallObjects[i].transform.localScale = Vector3.one * 0.95f;
                prismWallObjects[i].name = "prismWallObject" + i.ToString();
                prismWallObjects[i].layer = LayerMask.NameToLayer("Stage");
                prismWallObjects[i].SetActive(false);
            }
        }

        // 作成した壁を配置
        if(prismWallObjects != null && prismWallObjects.Length > 0)
        {
            if(prismWallActiveCount < prismWalls.Length)
            {
                // 壁の生成の必要あり
                int index = 0;
                foreach(var obj in prismWalls)
                {
                    bool flag = false;
                    foreach(var wall in prismWallObjects)
                    {
                        if (wall.transform.position == obj.transform.position + Vector3.up * 1.0f && wall.activeSelf)
                        {
                            flag = true;
                            break;
                        }
                    }

                    if (flag == false)
                    {
                        break;
                    }
                    index++;
                }

                foreach(var wall in prismWallObjects)
                {
                    if(wall.activeSelf == false)
                    {
                        Vector3 vec = prismWalls[index].transform.eulerAngles;
                        vec.y += 180;
                        wall.transform.rotation = Quaternion.Euler(vec);
                        wall.transform.position = prismWalls[index].transform.position + Vector3.up * 1.0f;
                        wall.SetActive(true);
                        prismWallActiveCount = prismWallActiveCount + 1 > prismWallObjects.Length ? prismWallObjects.Length : prismWallActiveCount + 1;
                        break;
                    }
                }
            }
            else if(prismWallActiveCount > prismWalls.Length)
            {
                // 壁の削除の必要あり
                foreach(var wall in prismWallObjects)
                {
                    if (wall.activeSelf)
                    {
                        bool flag = false;
                        foreach (var obj in prismWalls)
                        {
                            if (wall.transform.position == obj.transform.position + Vector3.up * 1.0f)
                            {
                                flag = true;
                                break;
                            }
                        }

                        if(flag == false)
                        {
                            wall.SetActive(false);
                            prismWallActiveCount = prismWallActiveCount - 1 < 0 ? 0 : prismWallActiveCount - 1;
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
    }

    /// <summary>
    /// 感電用のエフェクトを管理する処理
    /// </summary>
    /// <param name="active"></param>
    private void ElectroEffect(bool active)
    {
        if(electroParticle != null)
        {
            if (active)
            {
                if (electroParticle.isStopped)
                {
                    electroParticle.Play();
                    SoundManager.soundManager.PlaySe3D("EnemyMove", transform.position + Vector3.up * character.center.y, 0.3f);
                }
            }
            else
            {
                if (electroParticle.isPlaying)
                {
                    electroParticle.Stop();
                }
            }
        }
    }
}