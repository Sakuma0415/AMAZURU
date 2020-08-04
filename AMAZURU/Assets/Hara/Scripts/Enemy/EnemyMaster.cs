using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    [SerializeField, Tooltip("エネミーのPrefab")] private EnemyController enemyPrefab = null;
    [SerializeField, Tooltip("乾燥ナマコブロックのPrefab")] private DryEnemy dryEnemyPrefab = null;
    [SerializeField, Tooltip("帯電ナマコのモデルPrefab")] private GameObject electricEnemyPrefab = null;

    [SerializeField, Header("エネミーデータ")] private EnemyData[] enemyData = null;

    private bool startOperation = false;

    /// <summary>
    /// ステージ上のナマコ(エネミー)の情報を格納する
    /// </summary>
    public EnemyController[] Enemies { private set; get; } = null;
    private EnemyType[] enemyTypes = null;

    /// <summary>
    /// ステージ上の乾燥ナマコの情報を格納する
    /// </summary>
    public List<DryEnemy> DryEnemies { private set; get; } = null;

    /// <summary>
    /// ステージ上の帯電ナマコの情報を格納する
    /// </summary>
    public List<ElectricEnemy> ElectricEnemies { private set; get; } = null;

    /// <summary>
    /// 水位情報を扱う変数
    /// </summary>
    public WaterHi StageWater { set; private get; } = null;

    /// <summary>
    /// ゲームオーバー用のフラグ
    /// </summary>
    public bool IsGameOver { private set; get; } = false;

    /// <summary>
    /// プレイヤーとの接触フラグ
    /// </summary>
    public bool IsHit { private set; get; } = false;

    /// <summary>
    /// ゲーム停止中のフラグ
    /// </summary>
    public bool IsGameStop { set; private get; } = false;

    /// <summary>
    /// ゲーム待機中のフラグ
    /// </summary>
    public bool IsStandby { set; private get; } = false;

    /// <summary>
    /// ステージが感電状態になったフラグ
    /// </summary>
    public bool IsStageElectric { private set; get; } = false;

    /// <summary>
    /// 落雷地点となる帯電ナマコの情報
    /// </summary>
    public ElectricEnemy TargetElectricEnemy { private set; get; } = null;

    /// <summary>
    /// 初期化、生成処理
    /// </summary>
    public void Init()
    {
        transform.position = Vector3.zero;
        Enemies = new EnemyController[enemyData.Length];
        enemyTypes = new EnemyType[enemyData.Length];
        DryEnemies = new List<DryEnemy>();
        ElectricEnemies = new List<ElectricEnemy>();

        int count = 0;
        foreach(var data in enemyData)
        {
            // 敵のインスタンスを作成
            Vector3 startPos = data.UseStartPosSetting ? data.StartPosition : data.MovePlan[0];
            Enemies[count] = Instantiate(enemyPrefab, startPos, Quaternion.identity, gameObject.transform);
            Vector3 startRot;
            switch (data.StartRotate)
            {
                case EnemyData.RotateDirection.Forward:
                    startRot = Vector3.zero;
                    break;
                case EnemyData.RotateDirection.Back:
                    startRot = new Vector3(0, 180, 0);
                    break;
                case EnemyData.RotateDirection.Right:
                    startRot = new Vector3(0, 90, 0);
                    break;
                case EnemyData.RotateDirection.Left:
                    startRot = new Vector3(0, -90, 0);
                    break;
                default:
                    return;
            }
            enemyTypes[count] = data.Type;
            Enemies[count].EnemyStartRot = startRot;
            Enemies[count].EnemySize = data.Size;
            Enemies[count].MovePlan = data.MovePlan;
            Enemies[count].MoveType = data.MoveType;
            if(data.UseDefaultSetting == false)
            {
                Enemies[count].EnemySpeed = data.NomalSpeed;
                Enemies[count].EnemyWaterSpeed = data.WaterSpeed;
            }
            Enemies[count].StartPosFlag = data.UseStartPosSetting;
            Enemies[count].StageWater = StageWater;
            Enemies[count].EnemyInit();

            if(enemyTypes[count] == EnemyType.Dry)
            {
                // 乾燥ブロックのインスタンスを作成
                var block = Instantiate(dryEnemyPrefab, startPos, Quaternion.identity, gameObject.transform);
                block.EnemyObject = Enemies[count];
                block.BlockSize = data.Size;
                block.BlockCenterY = data.BlockSetPosY;
                block.ReturnDryMode = data.ReturnBlock;
                block.StageWater = StageWater;
                block.EnemyObject.gameObject.SetActive(false);
                block.DryEnemyInit();

                // 管理用のリストに追加
                DryEnemies.Add(block);
            }

            if(enemyTypes[count] == EnemyType.Electric)
            {
                // 帯電ナマコの情報を設定
                var electric = Enemies[count].gameObject.AddComponent<ElectricEnemy>();
                if(electricEnemyPrefab != null)
                {
                    var obj = Instantiate(electricEnemyPrefab, Enemies[count].transform);
                    obj.SetActive(false);
                }
                electric.EnemyObject = Enemies[count];
                electric.Init();

                // 管理用のリストに追加
                ElectricEnemies.Add(electric);
            }

            count++;
        }

        startOperation = true;
    }

    /// <summary>
    /// エネミーのフラグをチェックする
    /// </summary>
    private void CheckEnemyFlag()
    {
        bool isHit = false;
        bool isGameOver = false;
        foreach(var enemy in Enemies)
        {
            // プレイヤーとの接触をチェック
            if (enemy.IsHitPlayer && isHit == false)
            {
                isHit = true;
            }

            // ゲームオーバーチェック
            if (enemy.IsActonEnd && isGameOver == false)
            {
                isGameOver = true;
            }
        }

        // 帯電状態のナマコが水中にいるかチェック
        bool isStageElectric = false;
        foreach(var electric in ElectricEnemies)
        {
            if(electric.IsElectric && electric.EnemyObject.InWater)
            {
                isStageElectric = true;
                break;
            }
        }

        IsHit = isHit;
        IsGameOver = isGameOver;
        IsStageElectric = isStageElectric;
    }

    /// <summary>
    /// エネミーにゲームステートに応じたフラグをセットする
    /// </summary>
    private void SetState()
    {
        int count = 0;
        foreach(var enemy in Enemies)
        {
            // ポーズ中は処理を停止
            enemy.IsAllStop = IsGameStop;

            if (enemyTypes[count] != EnemyType.Dry)
            {
                // エネミーの種類が乾燥タイプ以外ならばゲームステートがプレイ及びアメフラシ起動時以外は移動処理を停止
                enemy.IsMoveStop = IsStandby;
            }
            count++;
        }

        // エネミーの種類が乾燥タイプのとき
        foreach(var dry in DryEnemies)
        {
            // ポーズ中は処理を停止
            dry.IsStop = IsGameStop;

            // ゲームステートがプレイ及びアメフラシ起動時以外またはアニメーションが実行中の場合は移動処理を停止
            dry.EnemyObject.IsMoveStop = IsStandby || dry.IsDoingAnimation;
        }
    }

    /// <summary>
    /// 落雷地点となる帯電ナマコをランダムに取得
    /// </summary>
    private void GetTargetElectricEnemy()
    {
        if(TargetElectricEnemy != null) { return; }
        // 帯電ナマコの中から帯電状態でないナマコのIDを取得
        List<int> index = new List<int>();
        int count = 0;
        foreach(var electric in ElectricEnemies)
        {
            if(electric.IsElectric == false)
            {
                index.Add(count);
            }
            count++;
        }

        // 全ての帯電ナマコが帯電化しているなら処理を終了
        if(index.Count < 1)
        {
            TargetElectricEnemy = null;
            return;
        }

        // 帯電化していないナマコをランダムに選定
        int rand = Random.Range(0, index.Count);
        TargetElectricEnemy = ElectricEnemies[index[rand]];
    }

    /// <summary>
    /// 帯電ナマコを帯電化させる
    /// </summary>
    public void ChangeElectricMode()
    {
        if(TargetElectricEnemy == null) { return; }
        TargetElectricEnemy.ElectricMode(true);
        TargetElectricEnemy = null;
    }

    private void Update()
    {
        if(startOperation == false) { return; }

        SetState();

        CheckEnemyFlag();

        GetTargetElectricEnemy();
    }
}
