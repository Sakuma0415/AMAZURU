using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class DryEnemy : MonoBehaviour
{
    [SerializeField, Tooltip("地面を取得する用のレイヤーマスク")] private LayerMask groundLayer;
    [SerializeField, Tooltip("水面を取得する用のレイヤーマスク")] private LayerMask waterLayer;
    public bool ReturnDryMode { set; private get; } = true;

    public float BlockSize { set; private get; } = 1.0f;
    public float BlockCenterY { set; private get; } = 0;
    private Vector3 spawnPos = Vector3.zero;  // スポーン地点はこのスクリプト上で設定する

    // このオブジェクトに必要なデータ
    private WaterHi stageWater = null;
    public EnemyController EnemyObject { set; private get; } = null;
    private BoxCollider box = null;
    private bool spawnFlag = false;
    private GameObject blockObject = null;
    private Coroutine coroutine = null;
    private Vector3 blockPos = Vector3.zero;
    private bool firstTime = true;

    // Start is called before the first frame update
    void Start()
    {
        
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
    public void DryEnemyInit()
    {
        spawnFlag = false;
        firstTime = true;
        

        // 水面の情報を取得する
        if (stageWater == null)
        {
            try
            {
                stageWater = Progress.progress.waterHi;
            }
            catch
            {
                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 200, waterLayer) && stageWater == null)
                {
                    stageWater = hit.transform.gameObject.GetComponent<WaterHi>();
                }
            }
        }

        // このオブジェクトに必要なデータを取得する
        if(box == null)
        {
            box = GetComponent<BoxCollider>();
        }
        if(blockObject == null)
        {
            blockObject = transform.GetChild(0).gameObject;
        }

        // 敵のスポーン地点を設定
        spawnPos = transform.position;

        // ブロックの座標データ及び、スケールデータを更新
        transform.localScale = Vector3.one * BlockSize;
        blockPos = FixedPosition(transform.position + Vector3.up * box.center.y);
        transform.position = blockPos + Vector3.up * BlockCenterY;
    }

    /// <summary>
    /// 水面の高さを取得し、一定値を超えたら敵をスポーンさせる
    /// </summary>
    private void CheckWaterHeight()
    {
        if (EnemyObject == null || stageWater == null || box == null) { return; }

        if(stageWater.max > (firstTime ? transform.position.y + box.center.y : blockPos.y))
        {
            if(spawnFlag == false)
            {
                spawnFlag = true;

                firstTime = false;

                // 敵をスポーンさせる
                SpawnEnemy();
            }
        }
        else
        {
            if (spawnFlag)
            {
                spawnFlag = false;

                // 敵をブロックの状態に戻す
                if (ReturnDryMode) { ReturnBlock(); }
            }
        }
    }

    /// <summary>
    /// 敵をスポーンさせる
    /// </summary>
    private void SpawnEnemy()
    {
        if(EnemyObject == null || blockObject == null || box == null) { return; }

        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(SpawnAnimation());
    }

    /// <summary>
    /// 敵を乾燥状態(ブロック)に戻す
    /// </summary>
    private void ReturnBlock()
    {
        if (EnemyObject == null || blockObject == null || box == null) { return; }

        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(ReturnBlockAnimation());
    }

    /// <summary>
    /// スポーン時のアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator SpawnAnimation()
    {
        float time = 0;
        float duration = 1.0f;

        EnemyObject.transform.position = transform.position;
        EnemyObject.gameObject.SetActive(true);
        EnemyObject.transform.localScale = Vector3.zero;
        EnemyObject.DontMove = true;

        while(time < duration)
        {
            float diff = time / duration;
            float sub = 1.0f - diff;

            // 敵のサイズを徐々に大きくしていく
            EnemyObject.transform.localScale = Vector3.one * BlockSize * diff;

            // ブロックを徐々に小さくしていく
            transform.localScale *= sub;

            time += Time.deltaTime;
            yield return null;
        }

        // 値の誤差を修正と、このオブジェクトを非表示にする
        EnemyObject.transform.localScale = Vector3.one * BlockSize;
        blockObject.SetActive(false);

        // 生成された敵をゆっくり地面に降下させる
        while (EnemyObject.transform.position != spawnPos)
        {
            EnemyObject.transform.position = Vector3.MoveTowards(EnemyObject.transform.position, spawnPos, 2.0f * Time.deltaTime);
            yield return null;
        }
        EnemyObject.DontMove = false;

        // ブロックの判定を無効にする
        transform.position = transform.position + Vector3.down * blockPos.y;
        box.enabled = false;

        // 処理完了
        coroutine = null;
    }

    /// <summary>
    /// ブロック状態にするアニメーション
    /// </summary>
    /// <returns></returns>
    private IEnumerator ReturnBlockAnimation()
    {
        float time = 0;
        float duration = 1.0f;

        blockPos = FixedPosition(EnemyObject.transform.position);
        transform.position = blockPos;
        transform.localScale = Vector3.zero;
        EnemyObject.DontMove = true;
        blockObject.SetActive(true);
        spawnPos = new Vector3(blockPos.x, EnemyObject.transform.position.y, blockPos.z);

        while (time < duration)
        {
            float diff = time / duration;
            float sub = 1.0f - diff;

            // 敵のサイズを徐々に小さくしていく
            EnemyObject.transform.localScale *= sub;

            // ブロックのサイズを徐々に大きくしていく
            transform.localScale = Vector3.one * BlockSize * diff;

            time += Time.deltaTime;
            yield return null;
        }

        // 誤差を補正
        transform.localScale = Vector3.one * BlockSize;

        // 表示・非表示の管理
        box.enabled = true;
        EnemyObject.gameObject.SetActive(false);
        EnemyObject.transform.position = blockPos + Vector3.down * box.size.y * 0.5f * BlockSize;

        // 処理完了
        coroutine = null;
    }

    /// <summary>
    /// ブロックが埋まらないように座標を修正する
    /// </summary>
    /// <param name="pos">修正前の座標</param>
    /// <returns></returns>
    private Vector3 FixedPosition(Vector3 pos)
    {
        return pos + Vector3.up * 0.5f * BlockSize;
    }
}
