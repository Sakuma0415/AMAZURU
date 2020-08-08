using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaster : MonoBehaviour
{
    [SerializeField, Tooltip("プレイヤーのPrefab")] private PlayerType2 playerPrefab = null;

    [SerializeField, Header("感電時の操作無効時間")] private float electricTimer = 1.0f;
    [SerializeField, Header("感電するインターバル")] private float electricInterval = 1.0f;

    /// <summary>
    /// プレイヤーの情報
    /// </summary>
    public PlayerType2 Player { private set; get; } = null;

    // エネミーの情報
    private EnemyMaster enemy = null;

    /// <summary>
    /// ゲームオーバーフラグ
    /// </summary>
    public bool IsGameOver { private set; get; } = false;

    /// <summary>
    /// 落雷を発生できる状態か
    /// </summary>
    public bool IsCanLightningStrike { private set; get; } = false;

    /// <summary>
    /// 落雷地点オブジェクト
    /// </summary>
    public GameObject LightningStrikePoint { private set; get; } = null;

    /// <summary>
    /// ステージの感電状態フラグ
    /// </summary>
    public bool IsStageElectric { private set; get; } = false;

    // 感電状態のフラグ
    private bool isElectric = false;
    private float time = 0;
    private bool isElectricInterval = false;

    /// <summary>
    /// プレイヤーをスポーンさせる（ステージの読み込みが完了した後に実行）
    /// </summary>
    /// <param name="spawnPosition">スポーン座標</param>
    /// <param name="water">ステージの水位情報</param>
    /// <param name="startRotation">スポーン時の向き</param>
    public void SpawnPlayer(Vector3 spawnPosition, WaterHi water, Vector3? startRotation = null)
    {
        // プレイヤーをスポーン
        Player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, transform);

        // プレイヤーの向きを設定
        Vector3 rot = startRotation ?? Vector3.zero;
        Player.transform.localRotation = Quaternion.Euler(rot);

        // プレイヤーに水位情報を渡す
        Player.StageWater = water;

        // 敵情報を取得
        GetEnemyInfo(water);
    }

    /// <summary>
    /// エネミーの情報を取得する
    /// </summary>
    /// <param name="water">ステージの水位情報</param>
    private void GetEnemyInfo(WaterHi water)
    {
        enemy = FindObjectOfType<EnemyMaster>();

        // 敵情報の取得に成功した場合 (敵が配置されているステージで正常に取得出来たときのみ)
        if (enemy != null)
        {
            // 敵のスポーン処理を開始
            enemy.StageWater = water;
            enemy.Init();
        }
    }

    /// <summary>
    /// エネミー情報を確認する
    /// </summary>
    private void CheckEnemyState()
    {
        if (enemy != null && Player != null)
        {
            // エネミー側のゲームオーバーフラグを取得する
            IsGameOver = enemy.IsGameOver;
        }
        else
        {
            IsGameOver = false;
        }
    }

    /// <summary>
    /// ゲームステートを取得
    /// </summary>
    /// <returns></returns>
    private PlayState.GameMode GetGameMode()
    {
        PlayState.GameMode mode;
        try
        {
            mode = PlayState.playState.gameMode;
        }
        catch
        {
            mode = PlayState.GameMode.Play;
        }
        return mode;
    }

    /// <summary>
    /// プレイヤーにゲームステートの情報を渡す
    /// </summary>
    private void SetPlayerGameState()
    {
        if(Player != null)
        {
            PlayState.GameMode mode = GetGameMode();

            // ポーズ中は移動処理とアニメーションを停止させる
            Player.IsGameStop = mode == PlayState.GameMode.Pause;

            if(mode != PlayState.GameMode.Play)
            {
                // ステートがプレイ以外のときは入力を受け付けない
                Player.DontInput = true;
            }
            else
            {
                // ステートがプレイのときは敵と接触または感電状態のときのみ入力不可にする
                if(enemy != null)
                {
                    Player.DontInput = enemy.IsHit || isElectric;
                }
                else
                {
                    // 敵がいなければ入力無効にしない
                    Player.DontInput = false;
                }
            }

            // アメフラシ起動時
            Player.IsRain = mode == PlayState.GameMode.Rain;

            // ゲームクリア時
            Player.IsGameClear = mode == PlayState.GameMode.Clear;

            // ゲームオーバー時
            Player.IsGameOver = mode == PlayState.GameMode.GameOver;

            // 感電時
            Player.IsElectric = isElectric;
        }
    }

    /// <summary>
    /// エネミーにゲームステートの情報を渡す
    /// </summary>
    private void SetEnemyGameState()
    {
        if(enemy != null)
        {
            PlayState.GameMode mode = GetGameMode();

            // ポーズ中は移動処理とアニメーションを停止
            enemy.IsGameStop = mode == PlayState.GameMode.Pause;

            // プレイ中とアメフラシ起動時以外のときはスタンバイ状態にする
            enemy.IsStandby = mode != PlayState.GameMode.Play && mode != PlayState.GameMode.Rain;
        }
    }

    /// <summary>
    /// 感電したかをチェックする
    /// </summary>
    private void CheckElectricDamage()
    {
        // タイマー設定が0以下にならないように修正
        electricTimer = Mathf.Max(0, electricTimer);
        electricInterval = Mathf.Max(0, electricInterval);

        if(IsCanLightningStrike && Player != null && GetGameMode() == PlayState.GameMode.Play)
        {
            IsStageElectric = enemy.IsStageElectric;

            if (Player.InWater && isElectric == false && isElectricInterval == false && IsStageElectric)
            {
                // プレイヤーが水中かつ感電状態でないとき感電状態にする
                isElectric = true;
                time = 0;
            }
            else if(isElectric && isElectricInterval == false)
            {
                // 指定の時間が経過したら感電状態を解除
                if(time < electricTimer)
                {
                    time += Time.deltaTime;
                }
                else
                {
                    time = 0;
                    isElectric = false;
                    isElectricInterval = true;
                }
            }
            else
            {
                // 感電状態解除後、指定の時間が経過するまで感電状態にならない
                if(time < electricInterval)
                {
                    time += Time.deltaTime;
                }
                else
                {
                    time = 0;
                    isElectricInterval = false;
                }
            }
        }
        else
        {
            isElectric = false;
            IsStageElectric = false;
        }
    }

    /// <summary>
    /// 落雷地点を取得
    /// </summary>
    private void GetLightningStrikePoint()
    {
        if (IsCanLightningStrike == false || enemy.TargetElectricEnemy == null)
        {
            LightningStrikePoint = null;
        }
        else
        {
            LightningStrikePoint = enemy.TargetElectricEnemy.gameObject;
        }
    }

    /// <summary>
    /// 落雷時に呼び出す処理
    /// </summary>
    public void LightningStrikeAction()
    {
        if(IsCanLightningStrike == false || LightningStrikePoint == null) { return; }
        enemy.ChangeElectricMode();
    }

    // Update is called once per frame
    void Update()
    {
        SetPlayerGameState();

        SetEnemyGameState();

        CheckEnemyState();

        // 落雷できるかチェック
        if(enemy != null)
        {
            IsCanLightningStrike = enemy.ElectricEnemies != null && enemy.ElectricEnemies.Count > 0;
        }
        else
        {
            IsCanLightningStrike = false;
        }

        GetLightningStrikePoint();

        CheckElectricDamage();
    }
}
