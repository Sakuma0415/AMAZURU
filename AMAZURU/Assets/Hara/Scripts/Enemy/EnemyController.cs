using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MyAnimation
{
    [SerializeField, Tooltip("敵のCharacterController")] private CharacterController enemy = null;
    [SerializeField, Tooltip("プレイヤーのレイヤー")] private LayerMask playerLayer;
    private enum moveType
    {
        Lap,
        Wrap
    }

    [SerializeField, Header("行動計画")] private Vector2[] movePlan = null;
    [SerializeField, Header("行動パターン")] private moveType type = moveType.Lap;
    [SerializeField, Header("行動遅延時間"), Range(0, 3)] private float lateTime = 1.0f; 
    [SerializeField, Header("敵の移動速度"), Range(0, 5)] private float enemySpeed = 1.0f;
    [SerializeField, Header("重力値"), Range(0, 10)] private float gravity = 1.0f;
    [SerializeField, Header("回転力"), Range(0, 20)] private float rotatePower = 1.0f;
    [SerializeField]private int location = 0;
    private float time = 0;
    private int step = 0;
    private bool stepEnd = false;
    private bool actionStop = false;
    private PlayState.GameMode mode = PlayState.GameMode.Stop;
    private float enemyPosY = 0;
    private bool playerHit = false;
    private bool finishOneLoop = false;


    // Start is called before the first frame update
    void Start()
    {
        EnemyInit();
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
        enemy = GetComponent<CharacterController>();
    }

    /// <summary>
    /// 敵の初期化
    /// </summary>
    private void EnemyInit()
    {
        if(movePlan == null || movePlan.Length <= 0) { Debug.LogError(gameObject.name + "のアメフラシさん : 「どこに行ったらいいかわからないよぉ...(泣)」"); }
    }

    /// <summary>
    /// 敵の移動処理
    /// </summary>
    /// <param name="fixedUpdate"></param>
    private void EnemyMove(bool fixedUpdate)
    {
        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;
        Vector3 moveDirection = Vector3.zero;
        mode = PlayState.playState.gameMode;
        enemyPosY = transform.position.y + enemy.center.y;
        
        actionStop = movePlan == null || movePlan.Length <= 0 || mode != PlayState.GameMode.Play;

        if(actionStop == false)
        {
            int nextLocation;
            if (finishOneLoop)
            {
                nextLocation = location - 1 < 0 ? type == moveType.Lap ? movePlan.Length - 1 : location + 1 : location - 1;
            }
            else
            {
                nextLocation = location + 1 >= movePlan.Length ? type == moveType.Lap ? 0 : location - 1 : location + 1;
            }
            Vector3 nextPos = new Vector3(movePlan[nextLocation].x, enemyPosY, movePlan[nextLocation].y);
            Vector3 forward = (nextPos - new Vector3(transform.position.x, enemyPosY, transform.position.z)).normalized;

            switch (step)
            {
                case 0:
                    enemy.enabled = false;
                    transform.position = new Vector3(movePlan[location].x, enemyPosY, movePlan[location].y);
                    enemy.enabled = true;
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
                    if(playerHit == false)
                    {
                        float vec = Mathf.Abs(forward.x) >= Mathf.Abs(forward.z) ? forward.z / forward.x : forward.x / forward.z;
                        vec = 1.0f / Mathf.Sqrt(1.0f + vec * vec);
                        moveDirection = forward * enemySpeed * delta * vec;
                        if(Vector3.Distance(new Vector3(transform.position.x, enemyPosY, transform.position.z), nextPos) < 0.1f)
                        {
                            stepEnd = true;
                        }
                    }
                    break;
                default:
                    step = 0;
                    _ = finishOneLoop ? location-- : location++;
                    if (location >= movePlan.Length || location < 0) { location = 0; }
                    if(type == moveType.Wrap)
                    {
                        if(location >= movePlan.Length - 1 && finishOneLoop == false)
                        {
                            finishOneLoop = true;
                            break;
                        }

                        if(location <= 0 && finishOneLoop)
                        {
                            finishOneLoop = false;
                        }
                    }
                    break;
            }

            if (stepEnd)
            {
                stepEnd = false;
                step++;
                time = 0;
            }
        }

        moveDirection.y -= gravity * delta;
        enemy.Move(moveDirection);
    }

    /// <summary>
    /// プレイヤーと衝突したときに実行
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            playerHit = true;
        }
    }

    /// <summary>
    /// プレイヤーとの衝突が解消されたときに実行
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            playerHit = false;
        }
    }
}
