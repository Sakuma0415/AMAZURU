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
    /// 水位情報を扱う変数
    /// </summary>
    public WaterHi StageWater { set; private get; } = null;

    /// <summary>
    /// 初期化、生成処理
    /// </summary>
    public void Init()
    {
        transform.position = Vector3.zero;

        foreach(var enemy in enemyData)
        {
            // 敵のインスタンスを作成
            var enemyObject = Instantiate(enemyPrefab, enemy.MovePlan[0], Quaternion.identity, gameObject.transform);
            Vector3 startRot;
            switch (enemy.StartRotate)
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
            enemyObject.EnemyStartRot = startRot;
            enemyObject.EnemySize = enemy.Size;
            enemyObject.MovePlan = enemy.MovePlan;
            enemyObject.MoveType = enemy.MoveType;
            if(enemy.UseDefaultSetting == false)
            {
                enemyObject.EnemySpeed = enemy.NomalSpeed;
                enemyObject.EnemyWaterSpeed = enemy.WaterSpeed;
            }
            enemyObject.StageWater = StageWater;
            enemyObject.EnemyInit();

            if(enemy.Type == EnemyData.EnemyType.Dry)
            {
                // 乾燥ブロックのインスタンスを作成
                var block = Instantiate(dryEnemyPrefab, enemy.MovePlan[0], Quaternion.identity, gameObject.transform);
                block.EnemyObject = enemyObject;
                block.BlockSize = enemy.Size;
                block.BlockCenterY = enemy.BlockSetPosY;
                block.ReturnDryMode = enemy.ReturnBlock;
                block.StageWater = StageWater;
                enemyObject.gameObject.SetActive(false);
                block.DryEnemyInit();
            }
        }
    }
}
