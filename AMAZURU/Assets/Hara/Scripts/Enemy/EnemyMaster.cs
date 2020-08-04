using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    [SerializeField, Tooltip("エネミーのPrefab")] private EnemyController enemyPrefab = null;
    [SerializeField, Tooltip("乾燥ナマコブロックのPrefab")] private DryEnemy dryEnemyPrefab = null;

    [SerializeField, Header("エネミーデータ")] private EnemyData[] enemyData = null;

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
            Enemies[count] = Instantiate(enemyPrefab, data.MovePlan[0], Quaternion.identity, gameObject.transform);
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
            Enemies[count].StageWater = StageWater;
            Enemies[count].EnemyInit();

            if(enemyTypes[count] == EnemyType.Dry)
            {
                // 乾燥ブロックのインスタンスを作成
                var block = Instantiate(dryEnemyPrefab, data.MovePlan[0], Quaternion.identity, gameObject.transform);
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
                electric.EnemyObject = Enemies[count];

                // 管理用のリストに追加
                ElectricEnemies.Add(electric);
            }

            count++;
        }
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

        IsHit = isHit;
        IsGameOver = isGameOver;
    }

    /// <summary>
    /// エネミーにゲームステートに応じたフラグをセットする
    /// </summary>
    private void SetState()
    {
        for(int i = 0; i < Enemies.Length; i++)
        {
            // ポーズ中は処理を停止
            Enemies[i].IsAllStop = IsGameStop;

            if(enemyTypes[i] != EnemyType.Dry)
            {
                // エネミーの種類が乾燥タイプ以外ならばゲームステートがプレイ及びアメフラシ起動時以外は移動処理を停止
                Enemies[i].IsMoveStop = IsStandby;
            }
        }

        foreach(var dry in DryEnemies)
        {
            // ポーズ中は処理を停止
            dry.IsStop = IsGameStop;

            // ゲームステートがプレイ及びアメフラシ起動時以外またはアニメーションが実行中の場合は移動処理を停止
            dry.EnemyObject.IsMoveStop = IsStandby || dry.IsDoingAnimation;
        }
    }

    private void Update()
    {
        SetState();

        CheckEnemyFlag();
    }
}
