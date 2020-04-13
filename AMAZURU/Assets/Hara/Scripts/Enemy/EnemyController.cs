using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MyAnimation
{
    [SerializeField, Tooltip("敵のCharacterController")] private SphereCollider enemy = null;
    [SerializeField, Tooltip("プレイヤーのレイヤー")] private LayerMask playerLayer;
    [SerializeField, Tooltip("地面のレイヤー")] private LayerMask groundLayer;
    [SerializeField, Tooltip("Rayの長さ"), Range(0, 5)] private float rayLength = 1.0f;
    private enum moveType
    {
        Lap,
        Wrap
    }
    private Vector3 gameStartPos = Vector3.zero;

    [SerializeField, Header("行動計画")] private Vector2[] movePlan = null;
    [SerializeField, Header("行動パターン")] private moveType type = moveType.Lap;
    [SerializeField, Header("行動遅延時間"), Range(0, 3)] private float lateTime = 1.0f; 
    [SerializeField, Header("敵の移動速度"), Range(0, 5)] private float enemySpeed = 1.0f;
    [SerializeField, Header("回転力"), Range(0, 20)] private float rotatePower = 1.0f;
    private Vector3[] moveSchedule = null;
    private int location = 0;
    private float time = 0;
    private int step = 0;
    private bool stepEnd = false;
    private bool actionStop = false;
    private PlayState.GameMode mode = PlayState.GameMode.Stop;
    private float enemyPosY = 0;
    private bool finishOneLoop = false;
    private bool standby = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        EnemyMove(true);
    }

    private void Reset()
    {
        enemy = GetComponent<SphereCollider>();
    }

    /// <summary>
    /// Activeがtrueになったら実行
    /// </summary>
    private void OnEnable()
    {
        EnemyInit();
    }

    /// <summary>
    /// 敵の初期化
    /// </summary>
    private void EnemyInit()
    {
        step = 0;

        // 床の高さを取得
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + enemy.center.y, transform.position.z), Vector3.down);
        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1.25f, groundLayer))
        {
            enemyPosY = hit.point.y + enemy.radius + enemy.center.y;
            gameStartPos = gameStartPos == Vector3.zero ? new Vector3(transform.position.x, enemyPosY, transform.position.z) : gameStartPos;
            transform.position = gameStartPos;
            SetMoveSchedule(movePlan);
            standby = true;
        }
        else
        {
            Debug.LogError(gameObject.name + "のアメフラシさん : 「地面から離れすぎてて怖いよぉ (>_<) 」");
            standby = false;
        }
    }

    /// <summary>
    /// 敵の移動処理
    /// </summary>
    /// <param name="fixedUpdate"></param>
    private void EnemyMove(bool fixedUpdate)
    {
        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        mode = PlayState.playState.gameMode;
        Vector3 nowPos = new Vector3(transform.position.x, enemyPosY, transform.position.z);

        actionStop = mode != PlayState.GameMode.Play || standby == false;

        if(actionStop == false)
        {
            int nextLocation;
            if (finishOneLoop)
            {
                nextLocation = location - 1 < 0 ? type == moveType.Lap ? moveSchedule.Length - 1 : location + 1 : location - 1;
            }
            else
            {
                nextLocation = location + 1 >= moveSchedule.Length ? type == moveType.Lap ? 0 : location - 1 : location + 1;
            }
            Vector3 nextPos = moveSchedule[nextLocation];
            Vector3 forward = (nextPos - nowPos).normalized;

            Ray ray = new Ray(new Vector3(transform.position.x, enemyPosY - enemy.radius * 2.0f, transform.position.z), Vector3.up);
            bool playerHit = Physics.SphereCast(ray, (enemy.radius) * 2.0f, rayLength, playerLayer);
            if (playerHit)
            {
                Debug.Log(gameObject.name + "のアメフラシさん : 「プレイヤーを見つけたよぉ ('ω')ノ 」");
            }

            switch (step)
            {
                case 0:
                    transform.position = moveSchedule[location];
                    if (Vector3.Distance(transform.position, moveSchedule[nextLocation]) < 0.1f)
                    {
                        step = -5;
                    }
                    stepEnd = true;
                    break;
                case 1:
                    stepEnd = Wait(time, lateTime);
                    time += delta;
                    break;
                case 2:
                    stepEnd = RotateAnimation(transform.gameObject, forward, Vector3.up, rotatePower * delta, false);
                    break;
                case 3:
                    stepEnd = Wait(time, lateTime);
                    time += delta;
                    break;
                case 4:
                    if (playerHit == false)
                    {
                        float vec = Mathf.Abs(forward.x) >= Mathf.Abs(forward.z) ? forward.z / forward.x : forward.x / forward.z;
                        vec = 1.0f / Mathf.Sqrt(1.0f + vec * vec);
                        transform.position += forward * enemySpeed * delta * vec;
                        if (Vector3.Distance(nowPos, nextPos) < 0.1f)
                        {
                            stepEnd = true;
                        }
                    }
                    break;
                default:
                    step = 0;
                    if (finishOneLoop)
                    {
                        location--;
                        if(type == moveType.Lap)
                        {
                            if(location < 0) { location = moveSchedule.Length - 1; }
                        }
                        else
                        {
                            if(location < 1) { finishOneLoop = false; }
                        }
                    }
                    else
                    {
                        location++;
                        if (type == moveType.Lap)
                        {
                            if (location >= moveSchedule.Length) { location = 0; }
                        }
                        else
                        {
                            if (location >= moveSchedule.Length - 1) { finishOneLoop = true; }
                        }
                    }
                    return;
            }

            if (stepEnd)
            {
                stepEnd = false;
                step++;
                time = 0;
            }
        }
    }

    /// <summary>
    /// 敵の移動スケジュールを設定
    /// </summary>
    private void SetMoveSchedule(Vector2[] plan)
    {
        moveSchedule = new Vector3[plan.Length < 2 ? 2 : plan.Length + 1];
        for(int i = 0; i < moveSchedule.Length; i++)
        {
            if(i > 0 && plan.Length > 0)
            {
                moveSchedule[i] = moveSchedule[i - 1];
                moveSchedule[i].x += plan[i - 1].x;
                moveSchedule[i].z += plan[i - 1].y;
            }
            else
            {
                moveSchedule[i] = transform.position;
            }
        }
    }
}
