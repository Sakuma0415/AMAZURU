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

    /// <summary>
    /// エネミーの情報
    /// </summary>
    public EnemyMaster Enemy { private set; get; } = null;

    // ステージの水位情報
    private WaterHi stageWater = null;

    /// <summary>
    /// ゲームオーバーフラグ
    /// </summary>
    public bool IsGameOver { private set; get; } = false;

    // 感電状態のフラグ
    private bool isElectric = false;
    [SerializeField, Header("デバッグ用のフラグ(感電モード)")] private bool debugFlag = false;
    private float time = 0;
    private bool flag = false;

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
        Enemy = FindObjectOfType<EnemyMaster>();

        // 敵情報の取得に成功した場合 (敵が配置されているステージで正常に取得出来たときのみ)
        if (Enemy != null)
        {
            // 敵のスポーン処理を開始
            Enemy.StageWater = stageWater;
            Enemy.Init();
        }
    }

    /// <summary>
    /// エネミー情報を確認する
    /// </summary>
    private void CheckEnemyState()
    {
        if (Enemy != null && Player != null)
        {
            // エネミー側のゲームオーバーフラグを取得する
            IsGameOver = Enemy.IsGameOver;
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
                if(Enemy != null)
                {
                    Player.DontInput = Enemy.IsHit || isElectric;
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
        if(Enemy != null)
        {
            PlayState.GameMode mode = GetGameMode();

            // ポーズ中は移動処理とアニメーションを停止
            Enemy.IsGameStop = mode == PlayState.GameMode.Pause;

            // プレイ中とアメフラシ起動時以外のときはスタンバイ状態にする
            Enemy.IsStandby = mode != PlayState.GameMode.Play && mode != PlayState.GameMode.Rain;
        }
    }

    /// <summary>
    /// 感電したかをチェックする
    /// </summary>
    private void CheckElectricDamage()
    {
        if(debugFlag == false) { return; }

        if(Enemy != null && Player != null && GetGameMode() == PlayState.GameMode.Play)
        {
            if (Player.InWater && isElectric == false && flag == false)
            {
                // プレイヤーが水中かつ感電状態でないとき感電状態にする
                isElectric = true;
                time = 0;
            }
            else if(isElectric && flag == false)
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
                    flag = true;
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
                    flag = false;
                }
            }
        }
        else
        {
            isElectric = false;
        }
    }

    private void Start()
    {
        // タイマーが0以下に設定されていた場合は修正
        electricTimer = Mathf.Max(0, electricTimer);
        electricInterval = Mathf.Max(0, electricInterval);
    }

    // Update is called once per frame
    void Update()
    {
        SetPlayerGameState();

        SetEnemyGameState();

        CheckEnemyState();

        CheckElectricDamage();
    }
}
