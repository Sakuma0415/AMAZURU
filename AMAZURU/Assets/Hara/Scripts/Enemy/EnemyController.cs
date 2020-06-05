﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MyAnimation
{
    [SerializeField, Tooltip("敵のCharacterController")] private SphereCollider enemy = null;
    [SerializeField, Tooltip("敵のAnimator")] private Animator enemyAnime = null;
    [SerializeField, Tooltip("プレイヤーのレイヤー")] private LayerMask playerLayer;
    [SerializeField, Tooltip("地面のレイヤー")] private LayerMask groundLayer;
    [SerializeField, Tooltip("水面のレイヤー")] private LayerMask waterLayer;
    [SerializeField, Tooltip("ステージの水オブジェクト")] private WaterHi stageWater = null;
    [SerializeField, Tooltip("PlayStateの設定")] private PlayState.GameMode mode = PlayState.GameMode.Play;
    private bool connectPlayState = false;
    private PlayerType2 player = null;
    private PlayerControllerNav player2 = null;
    

    private enum EnemyMoveType
    {
        Lap,
        Wrap
    }

    [SerializeField, Header("敵のサイズ"), Range(1.0f, 5.0f)] private float enemySize = 1.0f;
    [SerializeField, Header("行動計画")] private Vector2[] movePlan = null;
    [SerializeField, Header("行動パターン")] private EnemyMoveType moveType = EnemyMoveType.Lap;
    [SerializeField, Header("行動遅延時間"), Range(0, 3)] private float lateTime = 1.0f; 
    [SerializeField, Header("敵の移動速度"), Range(0, 10)] private float enemySpeed = 1.0f;
    [SerializeField, Header("敵の水中移動速度"), Range(0, 20)] private float enemyWaterSpeed = 1.0f;
    [SerializeField, Header("回転力")] private float rotatePower = 50f;
    [SerializeField, Header("敵のコライダーの大きさ"), Range(0.1f, 1.0f)] private float colliderSize = 0.5f;

    private Vector3[] moveSchedule = null;
    private int location = 0;
    private int step = 0;
    private bool stepEnd = false;
    private bool standby = false;
    private bool finishOneLoop = false;
    private bool inWater = false;

    // 足音再生用の変数
    private float animationSpeed = 0;
    private float animationTime = 0;

    [SerializeField, Header("デバッグ用のデータ更新フラグ")] private bool inspectorUpdate = false;


    // Start is called before the first frame update
    void Start()
    {
        EnemyInit(true);
    }

    private void FixedUpdate()
    {
        EnemyMove(true);

        if (inspectorUpdate)
        {
            inspectorUpdate = false;
            EnemyInit(false);
        }
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
        location = 0;
        standby = false;
        finishOneLoop = false;

        connectPlayState = GetPlayState();

        // コライダーの設定
        enemy.radius = colliderSize;
        enemy.center = new Vector3(0, colliderSize, 0);

        Ray ray = new Ray(new Vector3(transform.position.x, transform.position.y + enemy.center.y, transform.position.z), Vector3.down);
        RaycastHit hit;

        // 水面の取得
        if (stageWater == null)
        {
            if (Physics.Raycast(ray, out hit, 200, waterLayer))
            {
                stageWater = hit.transform.gameObject.GetComponent<WaterHi>();
            }
        }

        // アニメーションの速度を取得
        if (enemyAnime != null) { animationSpeed = enemyAnime.GetCurrentAnimatorStateInfo(0).speed; }

        // 床の位置を取得
        if (Physics.Raycast(ray, out hit, 200, groundLayer))
        {
            // 敵の開始時の位置を設定
            transform.position = new Vector3(transform.position.x, hit.point.y, transform.position.z);
            // 敵の開始時のサイズを設定
            transform.localScale = Vector3.one * enemySize;
            // 行動計画を設定
            SetMoveSchedule(movePlan, first);
            // 処理実行
            standby = true;
        }
    }

    /// <summary>
    /// 敵の移動処理
    /// </summary>
    /// <param name="fixedUpdate"></param>
    private void EnemyMove(bool fixedUpdate)
    {
        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        if (connectPlayState) { mode = PlayState.playState.gameMode; }

        inWater = stageWater != null && transform.position.y + enemy.radius < stageWater.max;

        if ((mode == PlayState.GameMode.Play || mode == PlayState.GameMode.Rain) && standby)
        {
            int nextLocation;
            if (finishOneLoop)
            {
                nextLocation = location - 1 < 0 ? moveType == EnemyMoveType.Lap ? moveSchedule.Length - 1 : location + 1 : location - 1;
            }
            else
            {
                nextLocation = location + 1 >= moveSchedule.Length ? moveType == EnemyMoveType.Lap ? 0 : location - 1 : location + 1;
            }
            Vector3 nowPos = moveSchedule[location];
            Vector3 nextPos = moveSchedule[nextLocation];
            Vector3 forward = (nextPos - nowPos).normalized;

            // プレイヤーと接触しているかをチェック
            RaycastHit hit;
            if (Physics.BoxCast(new Vector3(transform.position.x, transform.position.y - enemy.radius * enemySize, transform.position.z), new Vector3(enemy.radius * enemySize * 1.25f, enemy.radius * enemySize, enemy.radius * enemySize * 1.25f), Vector3.up, out hit, Quaternion.Euler(Vector3.zero), 1.0f, playerLayer))
            {
                // プレイヤーと接触している場合はプレイヤーの方向を向く処理を実行
                if(player == null) { player = hit.transform.gameObject.GetComponent<PlayerType2>(); }
                if(player == null && player2 == null)
                {
                    player2 = hit.transform.gameObject.GetComponent<PlayerControllerNav>();
                }
                bool complete = RotateAnimation(transform.gameObject, (new Vector3(hit.transform.position.x, 0, hit.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized, rotatePower * delta * 2.5f, false);
                
                if(player != null) { player.HitEnemy(complete); }
                if(player == null && player2 != null) { player2.HitEnemy(complete); }
            }
            else
            {
                switch (step)
                {
                    case 0:
                        transform.position = nowPos;
                        if (Vector3.Distance(transform.position, moveSchedule[nextLocation]) < 0.1f)
                        {
                            step = -5;
                        }
                        stepEnd = true;
                        break;
                    case 1:
                        stepEnd = RotateAnimation(transform.gameObject, forward, rotatePower * delta, false);
                        break;
                    case 2:
                        if (transform.rotation == Quaternion.LookRotation(forward))
                        {
                            float speed = inWater ? enemyWaterSpeed : enemySpeed;
                            transform.position = Vector3.MoveTowards(transform.position, moveSchedule[nextLocation], speed * delta);
                            stepEnd = transform.position == moveSchedule[nextLocation];
                        }
                        else
                        {
                            step = 1;
                        }
                        break;
                    default:
                        step = 0;
                        if (finishOneLoop)
                        {
                            location--;
                            if (moveType == EnemyMoveType.Lap)
                            {
                                if (location < 0) { location = moveSchedule.Length - 1; }
                            }
                            else
                            {
                                if (location < 1) { finishOneLoop = false; }
                            }
                        }
                        else
                        {
                            location++;
                            if (moveType == EnemyMoveType.Lap)
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
                }
            }

            // 足音の再生
            if (enemyAnime != null)
            {
                enemyAnime.enabled = true;
                animationTime += delta;
                if (animationTime >= animationSpeed)
                {
                    animationTime = 0;
                    SoundManager.soundManager.PlaySe3D("EnemyMove", transform.position, 0.3f);
                }
            }
        }
        else
        {
            // アニメーションの停止
            if(enemyAnime != null)
            {
                if(mode == PlayState.GameMode.StartEf || mode == PlayState.GameMode.Stop)
                {
                    enemyAnime.enabled = true;
                }
                else
                {
                    enemyAnime.enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// 敵の移動スケジュールを設定
    /// </summary>
    private void SetMoveSchedule(Vector2[] plan, bool first)
    {
        if (first)
        {
            moveSchedule = new Vector3[plan.Length < 2 ? 2 : plan.Length + 1];
            moveSchedule[0] = transform.position;
        }
        else
        {
            Vector3 pos = moveSchedule[0];
            moveSchedule = new Vector3[plan.Length < 2 ? 2 : plan.Length + 1];
            moveSchedule[0] = pos;
        }
        
        for(int i = 1; i < moveSchedule.Length; i++)
        {
            moveSchedule[i] = moveSchedule[i - 1];
            moveSchedule[i].x += plan[i - 1].x;
            moveSchedule[i].z += plan[i - 1].y;
        }
    }

    /// <summary>
    /// PlayStateを取得できるかチェック
    /// </summary>
    /// <returns></returns>
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
}
