﻿using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    [SerializeField, Tooltip("エネミーのPrefab")] private EnemyController enemyPrefab = null;
    [SerializeField, Tooltip("乾燥ナマコブロックのPrefab")] private DryEnemy dryEnemyPrefab = null;

    [SerializeField, Header("エネミーデータ")] private EnemyData[] enemyData = null;


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    /// <summary>
    /// 初期化、生成処理
    /// </summary>
    private void Init()
    {
        transform.position = Vector3.zero;

        foreach(var enemy in enemyData)
        {
            // 敵のインスタンスを作成
            var enemyObject = Instantiate(enemyPrefab, transform.parent);
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
            enemyObject.EnemyInit();

            if(enemy.Type == EnemyData.EnemyType.Dry)
            {
                // 乾燥ブロックのインスタンスを作成
                var block = Instantiate(dryEnemyPrefab, enemy.MovePlan[0] + Vector3.up * enemy.BlockSetPosY, Quaternion.identity, transform.parent);
                block.EnemyObject = enemyObject;
                block.BlockSize = enemy.Size;
                enemyObject.gameObject.SetActive(false);
                block.DryEnemyInit();
            }
        }
    }
}
