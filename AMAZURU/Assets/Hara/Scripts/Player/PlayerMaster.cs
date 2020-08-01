﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMaster : MonoBehaviour
{
    [SerializeField, Tooltip("プレイヤーのPrefab")] private PlayerType2 playerPrefab = null;

    /// <summary>
    /// プレイヤーの情報
    /// </summary>
    public PlayerType2 Player { private set; get; } = null;

    // エネミーの情報
    private EnemyMaster enemy = null;

    // ステージの水位情報
    private WaterHi stageWater = null;

    /// <summary>
    /// ゲームオーバーフラグ
    /// </summary>
    public bool IsGameOver { private set; get; } = false;

    private void Update()
    {
        CheckEnemyState();
    }

    /// <summary>
    /// プレイヤーをスポーンさせる（ステージの読み込みが完了した後に実行）
    /// </summary>
    /// <param name="spawnPosition">スポーン座標</param>
    /// <param name="water">ステージの水位情報</param>
    public void SpawnPlayer(Vector3 spawnPosition, WaterHi water)
    {
        // プレイヤーをスポーン
        Player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, transform);

        // ステージの水位情報を取得
        stageWater = water;

        // プレイヤーに水位情報を渡す
        Player.StageWater = stageWater;

        // 敵情報を取得
        GetEnemyInfo();
    }

    /// <summary>
    /// エネミーの情報を取得する
    /// </summary>
    private void GetEnemyInfo()
    {
        enemy = FindObjectOfType<EnemyMaster>();

        // 敵情報の取得に成功した場合 (敵が配置されているステージで正常に取得出来たときのみ)
        if(enemy != null)
        {
            // 敵のスポーン処理を開始
            enemy.StageWater = stageWater;
            enemy.Init();
        }
    }

    /// <summary>
    /// エネミー情報を確認する
    /// </summary>
    private void CheckEnemyState()
    {
        if(enemy != null && Player != null)
        {
            // プレイヤーとエネミーが接触しているなら入力を無効
            if (enemy.IsHit)
            {
                Player.DontInput = true;
            }
            else
            {
                Player.DontInput = false;
            }

            // ゲームオーバーフラグをチェック
            IsGameOver = enemy.IsEnd;
        }
    }
}
