using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class DryEnemy : MonoBehaviour
{
    [SerializeField, Tooltip("ナマコッコのPrafab")] private EnemyController enemyPrefab = null;
    [SerializeField, Tooltip("地面を取得する用のレイヤーマスク")] private LayerMask groundLayer;
    [SerializeField, Tooltip("水面を取得する用のレイヤーマスク")] private LayerMask waterLayer;

    // ナマコッコ(敵)に渡すデータ
    [SerializeField, Header("スポーン後の行動プラン")] private Vector2[] plan = null;
    [SerializeField, Header("スポーン後の行動パターン")] private EnemyMoveType type = EnemyMoveType.Lap;
    [SerializeField, Header("スポーン時の向き")] private Vector3 spawnRot = Vector3.zero;
    [SerializeField, Header("スポーン時のサイズ"), Range(1.0f, 5.0f)] private float spawnSize = 1.0f;
    private Vector3 spawnPos = Vector3.zero;  // スポーン地点はこのスクリプト上で設定する

    // このオブジェクトに必要なデータ
    private WaterHi stageWater = null;
    private EnemyController enemyInstance = null;
    private BoxCollider box = null;
    private bool spawnFlag = false;
    private MeshRenderer meshRenderer = null;
    private Coroutine coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        // 水面の高さをチェックする
        CheckWaterHeight();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        if(enemyPrefab == null)
        {
            Debug.LogError("インスタンス元の情報がありません");
            return;
        }

        spawnFlag = false;

        // 水面の情報を取得する
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hit, 200, waterLayer) && stageWater == null)
        {
            stageWater = hit.transform.gameObject.GetComponent<WaterHi>();
        }

        // このオブジェクトに必要なデータを取得する
        if(box == null)
        {
            box = GetComponent<BoxCollider>();
        }
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }
        box.enabled = true;
        meshRenderer.enabled = true;
        float hitY = Physics.Raycast(ray, out hit, 200, groundLayer) ? hit.point.y : (transform.position.y + box.center.y) - box.size.y * 0.5f;

        // 予め、敵のインスタンスを作成しておく
        spawnPos = new Vector3(transform.position.x, hitY, transform.position.z);
        enemyInstance = Instantiate(enemyPrefab, transform.position + Vector3.up * box.center.y, Quaternion.identity, transform.parent);
        enemyInstance.gameObject.SetActive(false);

        // 敵に必要な情報を渡す
        enemyInstance.SpecialControl = true;
        if(stageWater != null) { enemyInstance.StageWater = stageWater; }
        enemyInstance.MovePlan = plan;
        enemyInstance.MoveType = type;
        enemyInstance.EnemyStartPos = spawnPos;
        enemyInstance.EnemyStartRot = spawnRot;
        enemyInstance.EnemySize = spawnSize;
    }

    /// <summary>
    /// 水面の高さを取得し、一定値を超えたら敵をスポーンさせる
    /// </summary>
    private void CheckWaterHeight()
    {
        // 一度スポーンが完了したら、以降は呼び出さない
        if (spawnFlag || stageWater == null || box == null) { return; }

        if(stageWater.max > (transform.position.y + box.center.y) + box.size.y * 0.5f)
        {
            spawnFlag = true;

            // 敵をスポーンさせる
            SpawnEnemy();
        }
    }

    /// <summary>
    /// 敵をスポーンさせる
    /// </summary>
    private void SpawnEnemy()
    {
        if(enemyInstance == null || meshRenderer == null) { return; }

        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(SpawnAnimation());
    }

    /// <summary>
    /// スポーン時のアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnAnimation()
    {
        float time = 0;
        float duration = 1.0f;
        
        enemyInstance.gameObject.SetActive(true);
        enemyInstance.transform.localScale = Vector3.zero;

        while(time < duration)
        {
            float diff = time / duration;
            float sub = 1.0f - diff;

            // 敵のサイズを徐々に大きくしていく
            enemyInstance.transform.localScale = Vector3.one * spawnSize * diff;

            // ブロックを徐々に小さくしていく
            transform.localScale = Vector3.one * sub;

            time += Time.deltaTime;
            yield return null;
        }

        // 値の誤差を修正と、このオブジェクトを非表示にする
        enemyInstance.transform.localScale = Vector3.one * spawnSize;
        meshRenderer.enabled = false;
        box.enabled = false;
        transform.localScale = Vector3.one;

        // 生成された敵をゆっくり地面に降下させる
        while(enemyInstance.transform.position != spawnPos)
        {
            enemyInstance.transform.position = Vector3.MoveTowards(enemyInstance.transform.position, spawnPos, 2.0f * Time.deltaTime);
            yield return null;
        }

        // スポーンした敵の初期化処理を実行
        enemyInstance.SpecialControl = false;
        enemyInstance.EnemyInit();

        // 処理完了
        coroutine = null;
    }
}
