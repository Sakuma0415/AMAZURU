using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MyAnimation
{
    [SerializeField, Tooltip("敵のCharacterController")] private SphereCollider enemy = null;
    [SerializeField, Tooltip("敵のAnimator")] private Animator enemyAnime = null;
    [SerializeField, Tooltip("プレイヤーのレイヤー")] private LayerMask playerLayer;
    [SerializeField, Tooltip("地面のレイヤー")] private LayerMask groundLayer;
    [SerializeField, Tooltip("水面のレイヤー")] private LayerMask waterLayer;
    [SerializeField, Tooltip("Rayの長さ"), Range(0, 5)] private float rayLength = 1.0f;
    [SerializeField, Tooltip("ステージの水オブジェクト")] private WaterHi stageWater = null;
    [SerializeField, Tooltip("Playerのスクリプト")] private PlayerType2 player = null;
    [SerializeField, Tooltip("PlayStateの設定")] private PlayState.GameMode mode = PlayState.GameMode.Stop;
    [SerializeField, Tooltip("PlayStateと同期させる")] private bool stateSet = true;
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
    [SerializeField, Header("敵の水中移動速度"), Range(0, 5)] private float enemyWaterSpeed = 1.0f;
    [SerializeField, Header("回転力"), Range(0, 20)] private float rotatePower = 1.0f;
    private Vector3[] moveSchedule = null;
    private int location = 0;
    private float time = 0;
    private int step = 0;
    private bool stepEnd = false;
    private bool actionStop = false;
    private float enemyPosY = 0;
    private bool finishOneLoop = false;
    private bool standby = false;
    private bool inWater = false;


    // Start is called before the first frame update
    void Start()
    {
        EnemyInit(true);
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
    /// 敵の初期化
    /// </summary>
    private void EnemyInit(bool first)
    {
        step = 0;
        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + enemy.radius - enemy.center.y, transform.position.z), Vector3.down);
        RaycastHit hit;

        // 水面の取得
        if(stageWater == null)
        {
            if (Physics.Raycast(ray, out hit, 200, waterLayer))
            {
                stageWater = hit.transform.gameObject.GetComponent<WaterHi>();
            }
            else
            {
                Debug.LogError(gameObject.name + "のアメフラシさん ： 「水がないと干からびちゃうよぉ　(T_T)」");
            }
        }

        // 床の高さを取得
        if (Physics.Raycast(ray, out hit, 200, groundLayer))
        {
            enemyPosY = hit.point.y + enemy.radius - enemy.center.y;
            if (first)
            {
                gameStartPos = new Vector3(transform.position.x, enemyPosY, transform.position.z);
            }
            transform.position = gameStartPos;
            SetMoveSchedule(movePlan);
            standby = true;
        }
        else
        {
            Debug.LogError(gameObject.name + "のアメフラシさん : 「地面が見当たらないよぉ (>_<) 」");
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

        if (stateSet) { mode = PlayState.playState.gameMode; }
        Vector3 nowPos = new Vector3(transform.position.x, enemyPosY, transform.position.z);

        actionStop = mode != PlayState.GameMode.Play || standby == false;

        inWater = stageWater != null && enemyPosY < stageWater.max;

        if (actionStop == false)
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

            // プレイヤーと接触しているかをチェック
            Ray ray = new Ray(new Vector3(transform.position.x, enemyPosY - enemy.radius * 2.0f, transform.position.z), Vector3.up);
            RaycastHit hit;
            bool playerHit = false;
            if (Physics.SphereCast(ray, (enemy.radius) * 2.0f, out hit, rayLength, playerLayer))
            {
                playerHit = true;
                if(player == null)
                {
                    player = hit.transform.gameObject.GetComponent<PlayerType2>();
                }
            }

            if(player != null)
            {
                player.ContactEnemy = playerHit;
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
                        float speed = inWater ? enemyWaterSpeed : enemySpeed;
                        transform.position += forward * speed * delta * vec;
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
