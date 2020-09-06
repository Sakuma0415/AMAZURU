using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

public class DryEnemy : MonoBehaviour
{
    public bool ReturnDryMode { set; private get; } = true;

    public float BlockSize { set; private get; } = 1.0f;
    public float BlockCenterY { set; private get; } = 0;
    private Vector3 spawnPos = Vector3.zero;  // スポーン地点はこのスクリプト上で設定する

    /// <summary>
    /// 水位情報を扱う変数
    /// </summary>
    public WaterHi StageWater { set; private get; } = null;

    /// <summary>
    /// アニメーション実行中のフラグ
    /// </summary>
    public bool IsDoingAnimation { private set; get; } = false;

    /// <summary>
    /// ゲーム停止中のフラグ
    /// </summary>
    public bool IsStop { set; private get; } = false;

    public Vector3 StartPosition { set; private get; } = Vector3.zero;

    // このオブジェクトに必要なデータ
    public EnemyController EnemyObject { set; get; } = null;
    private BoxCollider box = null;
    private bool spawnFlag = false;
    private GameObject blockObject = null;
    private Coroutine coroutine = null;
    private Vector3 blockPos = Vector3.zero;
    private bool firstTime = true;

    // 処理開始のフラグ
    private bool isStartAction = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isStartAction == false) { return; }

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
        spawnPos = StartPosition;

        // ブロックの座標データ及び、スケールデータを更新
        transform.localScale = Vector3.one * BlockSize;
        blockPos = FixedPosition(StartPosition + transform.up * box.center.y);
        transform.localPosition =  blockPos + transform.up * BlockCenterY;

        isStartAction = true;
    }

    /// <summary>
    /// 水面の高さを取得し、一定値を超えたら敵をスポーンさせる
    /// </summary>
    private void CheckWaterHeight()
    {
        if (EnemyObject == null || StageWater == null || box == null) { return; }

        if(StageWater.max > (firstTime ? transform.position.y + box.center.y : blockPos.y))
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
            // 水位が下回り、ブロックに戻すフラグがtrueのときのみ実行する
            if (spawnFlag && ReturnDryMode)
            {
                spawnFlag = false;
                ReturnBlock();
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

        EnemyObject.transform.localPosition = transform.localPosition;
        EnemyObject.transform.localScale = Vector3.zero;
        IsDoingAnimation = true;

        // 1フレーム遅延させてから敵を表示させる
        yield return null;
        EnemyObject.gameObject.SetActive(true);

        while (time < duration)
        {
            // ポーズ中は待機処理をループ
            while (IsStop)
            {
                yield return null;
            }

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
        while (EnemyObject.transform.localPosition != spawnPos)
        {
            // ポーズ中は待機処理をループ
            while (IsStop)
            {
                yield return null;
            }

            EnemyObject.transform.localPosition = Vector3.MoveTowards(EnemyObject.transform.localPosition, spawnPos, 2.0f * Time.deltaTime);
            yield return null;
        }
        IsDoingAnimation = false;

        // ブロックの判定を無効にする
        transform.localPosition = transform.localPosition - transform.up * blockPos.y;
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

        blockPos = FixedPosition(EnemyObject.transform.localPosition);
        transform.localPosition = blockPos;
        transform.localScale = Vector3.zero;
        IsDoingAnimation = true;
        blockObject.SetActive(true);
        spawnPos = new Vector3(blockPos.x, EnemyObject.transform.localPosition.y, blockPos.z);

        while (time < duration)
        {
            // ポーズ中は待機処理をループ
            while (IsStop)
            {
                yield return null;
            }

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
        EnemyObject.transform.localPosition = blockPos - transform.up * box.size.y * 0.5f * BlockSize;

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
