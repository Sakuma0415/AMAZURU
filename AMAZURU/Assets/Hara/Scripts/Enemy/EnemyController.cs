using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemy
{
    public enum EnemyMoveType
    {
        Lap,
        Wrap
    }

    public enum EnemyType
    {
        Normal,
        Dry,
        Electric
    }

    public class EnemyController : MyAnimation
    {
        private SphereCollider enemy = null;
        [SerializeField, Tooltip("敵のAnimator")] private Animator enemyAnime = null;
        [SerializeField, Tooltip("プレイヤーのレイヤー")] private LayerMask playerLayer;

        /// <summary>
        /// 水位の情報を扱う変数
        /// </summary>
        public WaterHi StageWater { set; private get; } = null;

        [SerializeField, Header("開始向き")] private Vector3 enemyStartRot = Vector3.zero;
        public Vector3 EnemyStartRot { set { enemyStartRot = value; } }

        [SerializeField, Header("敵のサイズ"), Range(1.0f, 5.0f)] private float enemySize = 1.0f;
        public float EnemySize { set { enemySize = value; } }

        [SerializeField, Header("行動計画")] private Vector3[] movePlan = null;
        public Vector3[] MovePlan { set { movePlan = value; } }

        [SerializeField, Header("行動パターン")] private EnemyMoveType moveType = EnemyMoveType.Lap;
        public EnemyMoveType MoveType { set { moveType = value; } }

        [SerializeField, Header("行動遅延時間"), Range(0, 3)] private float lateTime = 1.0f;

        [SerializeField, Header("敵の移動速度"), Range(0, 10)] private float enemySpeed = 1.0f;
        public float EnemySpeed { set { enemySpeed = value; } }

        [SerializeField, Header("敵の水中移動速度"), Range(0, 20)] private float enemyWaterSpeed = 1.0f;
        public float EnemyWaterSpeed { set { enemyWaterSpeed = value; } }

        [SerializeField, Header("回転力")] private float rotatePower = 50f;
        [SerializeField, Header("敵のコライダーの大きさ"), Range(0.1f, 1.0f)] private float colliderSize = 0.5f;

        private int location = 0;
        private int step = 0;
        private bool stepEnd = false;
        private bool finishOneLoop = false;

        /// <summary>
        /// 水中フラグ
        /// </summary>
        public bool InWater { private set; get; } = false;

        // 足音再生用の変数
        private float animationSpeed = 0;
        private float animationTime = 0;

        /// <summary>
        /// プレイヤーと接触した際のアクション処理が完了したことを検知するフラグ
        /// </summary>
        public bool IsActonEnd { private set; get; } = false;

        /// <summary>
        /// プレイヤーとの接触を検知するフラグ
        /// </summary>
        public bool IsHitPlayer { private set; get; } = false;

        /// <summary>
        /// 移動処理とアニメーションを完全に停止するフラグ
        /// </summary>
        public bool IsAllStop { set; private get; } = false;

        /// <summary>
        /// 移動処理のみを停止するフラグ
        /// </summary>
        public bool IsMoveStop { set; private get; } = false;

        private void FixedUpdate()
        {
            if(IsAllStop == false)
            {
                // エネミーの移動処理
                EnemyMove(true);
            }
            else
            {
                // アニメーションの停止
                if (enemyAnime != null)
                {
                    enemyAnime.enabled = false;
                }
            }
        }

        private void Reset()
        {
            enemy = GetComponent<SphereCollider>();
        }

        /// <summary>
        /// 敵の初期化
        /// </summary>
        /// <param name="shortcut">一部処理をパスするフラグ</param>
        public void EnemyInit()
        {
            step = 0;
            location = 0;
            finishOneLoop = false;

            if(enemy == null)
            {
                enemy = GetComponent<SphereCollider>();
            }

            // コライダーの設定
            enemy.radius = colliderSize;
            enemy.center = new Vector3(0, colliderSize, 0);

            // アニメーションの速度を取得
            if (enemyAnime != null) { animationSpeed = enemyAnime.GetCurrentAnimatorStateInfo(0).speed; }

            // 敵の開始時の向き
            transform.rotation = Quaternion.Euler(enemyStartRot);

            // 敵の開始時のサイズを設定
            transform.localScale = Vector3.one * enemySize;
        }

        /// <summary>
        /// 敵の移動処理
        /// </summary>
        /// <param name="fixedUpdate"></param>
        private void EnemyMove(bool fixedUpdate)
        {
            float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

            // 水中かチェックする
            InWater = StageWater != null && transform.position.y + enemy.radius < StageWater.max;

            if (IsMoveStop == false)
            {
                int nextLocation;
                if (finishOneLoop)
                {
                    nextLocation = location - 1 < 0 ? moveType == EnemyMoveType.Lap ? movePlan.Length - 1 : location + 1 : location - 1;
                }
                else
                {
                    nextLocation = location + 1 >= movePlan.Length ? moveType == EnemyMoveType.Lap ? 0 : location - 1 : location + 1;
                }
                Vector3 nowPos = movePlan[location];
                Vector3 nextPos = movePlan[nextLocation];
                Vector3 forward = (nextPos - nowPos).normalized;

                // プレイヤーと接触しているかをチェック
                IsHitPlayer = Physics.BoxCast(new Vector3(transform.position.x, transform.position.y - enemy.radius * enemySize, transform.position.z), new Vector3(enemy.radius * enemySize * 1.25f, enemy.radius * enemySize, enemy.radius * enemySize * 1.25f), Vector3.up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), 1.0f, playerLayer);
                if (IsHitPlayer)
                {
                    // プレイヤーと接触している場合はプレイヤーの方向を向く処理を実行
                    IsActonEnd = RotateAnimation(transform.gameObject, (new Vector3(hit.transform.position.x, 0, hit.transform.position.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized, rotatePower * delta * 2.5f, false);
                }
                else
                {
                    switch (step)
                    {
                        case 0:
                            transform.position = nowPos;
                            if (Vector3.Distance(transform.position, movePlan[nextLocation]) < 0.1f)
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
                                float speed = InWater ? enemyWaterSpeed : enemySpeed;
                                transform.position = Vector3.MoveTowards(transform.position, movePlan[nextLocation], speed * delta);
                                stepEnd = transform.position == movePlan[nextLocation];
                            }
                            else
                            {
                                step = 1;
                            }
                            break;
                        case 3:
                            if (finishOneLoop)
                            {
                                location--;
                                if (moveType == EnemyMoveType.Lap)
                                {
                                    if (location < 0) { location = movePlan.Length - 1; }
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
                                    if (location >= movePlan.Length) { location = 0; }
                                }
                                else
                                {
                                    if (location >= movePlan.Length - 1) { finishOneLoop = true; }
                                }
                            }
                            stepEnd = true;
                            break;
                        default:
                            step = 0;
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
        }
    }
}


