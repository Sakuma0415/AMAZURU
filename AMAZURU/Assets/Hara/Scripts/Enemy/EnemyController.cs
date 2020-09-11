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
        [SerializeField, Tooltip("敵のAnimator")] private Animator enemyAnime = null;
        public Animator EnemyAnime { set { enemyAnime = value; } }

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

        /// <summary>
        /// ローカル座標に変換するための基点
        /// </summary>
        public Transform MasterTransform { set; private get; } = null;

        /// <summary>
        /// 開始時の座標
        /// </summary>
        public Vector3 StartPosition { set; private get; } = Vector3.zero;

        private int location = 0;
        private int step = 0;
        private bool stepEnd = false;
        private bool finishOneLoop = false;

        // 処理開始のフラグ
        private bool isStartAction = false;

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

        /// <summary>
        /// スタート地点を別の座標にするフラグ
        /// </summary>
        public bool StartPosFlag { set; private get; } = false;

        private void FixedUpdate()
        {
            if(IsAllStop == false)
            {
                if(isStartAction == false) { return; }
                // エネミーの移動処理
                EnemyMove(true);
            }

            // アニメーションの停止管理
            if (enemyAnime != null)
            {
                enemyAnime.enabled = !IsAllStop;
            }
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

            // アニメーションの速度を取得
            if (enemyAnime != null) { animationSpeed = enemyAnime.GetCurrentAnimatorStateInfo(0).speed; }

            // 敵を開始時の座標に配置
            transform.localPosition = StartPosition;

            // 敵の開始時の向き
            transform.localRotation = Quaternion.Euler(enemyStartRot);

            // 敵の開始時のサイズを設定
            transform.localScale = Vector3.one * enemySize;

            isStartAction = true;
        }

        /// <summary>
        /// 敵の移動処理
        /// </summary>
        /// <param name="fixedUpdate"></param>
        private void EnemyMove(bool fixedUpdate)
        {
            float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

            // 水中かチェックする
            InWater = StageWater != null && transform.position.y < StageWater.max;

            if (IsMoveStop == false)
            {
                if(IsHitPlayer == false)
                {
                    // プレイヤーと接触しているかをチェック
                    Vector3 up = transform.up;
                    if (Physics.BoxCast(transform.position - up * 0.5f * enemySize, new Vector3(up.x == 0 ? enemySize * 1.25f : enemySize, up.y == 0 ? enemySize * 1.25f : enemySize, up.z == 0 ? enemySize * 1.25f : enemySize) * 0.5f, up, out RaycastHit hit, Quaternion.Euler(Vector3.zero), 0.25f, playerLayer))
                    {
                        IsHitPlayer = true;
                        StartCoroutine(HitAnimation(hit.transform.position));
                        return;
                    }

                    int nextLocation;
                    if (StartPosFlag)
                    {
                        nextLocation = location;
                    }
                    else
                    {
                        if (finishOneLoop)
                        {
                            nextLocation = location - 1 < 0 ? moveType == EnemyMoveType.Lap ? movePlan.Length - 1 : location + 1 : location - 1;
                        }
                        else
                        {
                            nextLocation = location + 1 >= movePlan.Length ? moveType == EnemyMoveType.Lap ? 0 : location - 1 : location + 1;
                        }
                    }

                    Vector3 nextPos = movePlan[nextLocation];
                    Vector3 forward = (nextPos - transform.localPosition).normalized;

                    switch (step)
                    {
                        case 0:
                            if (Vector3.Distance(transform.localPosition, nextPos) < 0.1f)
                            {
                                step = -5;
                            }
                            stepEnd = true;
                            break;
                        case 1:
                            stepEnd = RotateAnimation(transform.gameObject, forward, rotatePower * delta, true);
                            break;
                        case 2:
                            if (transform.localRotation == Quaternion.LookRotation(forward))
                            {
                                float speed = InWater ? enemyWaterSpeed : enemySpeed;
                                transform.localPosition = Vector3.MoveTowards(transform.localPosition, nextPos, speed * delta);
                                if (Vector3.Distance(transform.localPosition, nextPos) < 0.1f)
                                {
                                    transform.localPosition = nextPos;
                                    stepEnd = true;
                                }
                            }
                            else
                            {
                                step = 1;
                            }
                            break;
                        case 3:
                            if (StartPosFlag == false)
                            {
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
                            }
                            stepEnd = true;
                            break;
                        default:
                            StartPosFlag = false;
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
                animationTime += delta;
                if (animationTime >= animationSpeed)
                {
                    animationTime = 0;
                    SoundManager.soundManager.PlaySe3D("EnemyMove", transform.position, 0.3f);
                }
            }
        }

        /// <summary>
        /// プレイヤーにヒットした時に実行するアニメーション
        /// </summary>
        /// <param name="playerPos">プレイヤーの座標</param>
        /// <returns></returns>
        private IEnumerator HitAnimation(Vector3 playerPos)
        {
            // プレイヤーの座標をローカル座標に変換
            Vector3 target = transform.parent.InverseTransformPoint(playerPos);
            target.y = 0;

            // プレイヤーの方向を向く
            while (RotateAnimation(transform.gameObject, (target - new Vector3(transform.localPosition.x, 0, transform.localPosition.z)).normalized, rotatePower * Time.deltaTime * 2.5f, true) == false)
            {
                while(IsMoveStop || IsAllStop)
                {
                    yield return null;
                }
                yield return null;
            }

            // 処理の完了フラグ
            IsActonEnd = true;
        }
    }
}


