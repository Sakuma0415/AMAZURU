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
    /// 初期化、生成処理
    /// </summary>
    public void Init()
    {
        transform.position = Vector3.zero;
        enemies = new EnemyController[enemyData.Length];

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

            if(data.Type == EnemyData.EnemyType.Dry)
            {
                // 乾燥ブロックのインスタンスを作成
                var block = Instantiate(dryEnemyPrefab, data.MovePlan[0], Quaternion.identity, gameObject.transform);
                block.EnemyObject = enemies[count];
                block.BlockSize = data.Size;
                block.BlockCenterY = data.BlockSetPosY;
                block.ReturnDryMode = data.ReturnBlock;
                block.StageWater = StageWater;
                enemies[count].gameObject.SetActive(false);
                block.DryEnemyInit();
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

    private void Update()
    {
        CheckEnemyFlag();
    }
}
