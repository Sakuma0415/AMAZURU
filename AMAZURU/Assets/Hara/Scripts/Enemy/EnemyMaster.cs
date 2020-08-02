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

    private EnemyController[] enemies = null;
    private EnemyType[] enemyTypes = null;
    private List<DryEnemy> dryEnemies = null;

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
        enemies = new EnemyController[enemyData.Length];
        enemyTypes = new EnemyType[enemyData.Length];
        dryEnemies = new List<DryEnemy>();

        int count = 0;
        foreach(var data in enemyData)
        {
            // 敵のインスタンスを作成
            enemies[count] = Instantiate(enemyPrefab, data.MovePlan[0], Quaternion.identity, gameObject.transform);
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
            enemies[count].EnemyStartRot = startRot;
            enemies[count].EnemySize = data.Size;
            enemies[count].MovePlan = data.MovePlan;
            enemies[count].MoveType = data.MoveType;
            if(data.UseDefaultSetting == false)
            {
                enemies[count].EnemySpeed = data.NomalSpeed;
                enemies[count].EnemyWaterSpeed = data.WaterSpeed;
            }
            enemies[count].StageWater = StageWater;
            enemies[count].EnemyInit();

            if(enemyTypes[count] == EnemyType.Dry)
            {
                // 乾燥ブロックのインスタンスを作成
                var block = Instantiate(dryEnemyPrefab, data.MovePlan[0], Quaternion.identity, gameObject.transform);
                block.EnemyObject = enemies[count];
                block.BlockSize = data.Size;
                block.BlockCenterY = data.BlockSetPosY;
                block.ReturnDryMode = data.ReturnBlock;
                block.StageWater = StageWater;
                block.EnemyObject.gameObject.SetActive(false);
                block.DryEnemyInit();

                // 管理用のリストに追加
                dryEnemies.Add(block);
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
        foreach(var enemy in enemies)
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
        for(int i = 0; i < enemies.Length; i++)
        {
            // ポーズ中は処理を停止
            enemies[i].IsAllStop = IsGameStop;

            if(enemyTypes[i] == EnemyType.Normal)
            {
                // エネミーの種類がノーマルならばスタンバイフラグを通常で設定
                enemies[i].IsMoveStop = IsStandby;
            }
        }

        foreach(var dry in dryEnemies)
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
