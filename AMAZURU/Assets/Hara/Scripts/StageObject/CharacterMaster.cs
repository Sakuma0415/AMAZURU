using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMaster : SingletonMonoBehaviour<CharacterMaster>
{
    [SerializeField, Tooltip("プレイヤーのPrefab")] private PlayerType2 playerPrefab = null;
    [SerializeField, Tooltip("雷オブジェクト")] private Thunder thunderPrefab = null;

    [SerializeField, Header("感電時の操作無効時間")] private float electricTimer = 1.0f;
    [SerializeField, Header("感電するインターバル")] private float electricInterval = 1.0f;

    private PlayState.GameMode gameMode = PlayState.GameMode.Play;

    /// <summary>
    /// プレイヤーの情報
    /// </summary>
    public PlayerType2 Player { private set; get; } = null;

    // エネミーの情報
    private EnemyMaster enemy = null;

    /// <summary>
    /// 酸素ゲージの管理スクリプト
    /// </summary>
    public O2Controller OxygenGauge { private set; get; } = null;

    /// <summary>
    /// 読み込まれたステージのデータ
    /// </summary>
    public StageData LoadStageData { private set; get; } = null;

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

    private Thunder thunder = null;

    /// <summary>
    /// プレイヤーをスポーンさせる（ステージの読み込みが完了した後に実行）
    /// </summary>
    /// <param name="spawnPosition">スポーン座標</param>
    /// <param name="water">ステージの水位情報</param>
    /// <param name="oxygen">酸素ゲージスクリプト</param>
    /// <param name="loadStage">読み込むステージの情報</param>
    /// <param name="startRotation">スポーン時の向き</param>
    public void SpawnPlayer(Vector3 spawnPosition, WaterHi water, O2Controller oxygen, StageData loadStage, Vector3? startRotation = null)
    {
        // 酸素ゲージを取得
        OxygenGauge = oxygen;

        // ステージデータを取得
        LoadStageData = loadStage;

        // プレイヤーをスポーン
        Player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity, transform);

        // プレイヤーの向きを設定
        Vector3 rot = startRotation ?? Vector3.zero;
        Player.transform.localRotation = Quaternion.Euler(rot);

        // プレイヤーに水位情報を渡す
        Player.StageWater = water;

        // 敵情報を取得
        GetEnemyInfo(water);

        if (loadStage.IsThunder)
        {
            thunder = Instantiate(thunderPrefab, transform.parent);
        }
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
            // ステージの中心座標を取得
            Vector3 stageCenter;
            if(LoadStageData != null)
            {
                stageCenter = LoadStageData.stageSize * 0.5f;
            }
            else
            {
                stageCenter = Vector3.zero;
            }

            enemy.StageCenter = stageCenter;

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
            // 酸素ゲージが0になったかをチェックする
            bool waterDead = OxygenGauge != null && OxygenGauge.WaterDeth;

            // ポーズ中は移動処理とアニメーションを停止させる
            Player.IsGameStop = gameMode == PlayState.GameMode.Pause;

            // 敵との接触フラグ
            Player.IsHitEnemy = enemy != null && enemy.IsHit;

            if(gameMode != PlayState.GameMode.Play)
            {
                // ステートがプレイ以外のときは入力を受け付けない
                Player.DontInput = true;
            }
            else
            {
                // ステートがプレイのときは敵と接触または感電状態時のみ入力不可にする
                Player.DontInput = Player.IsHitEnemy || isElectric;
            }

            // アメフラシ起動時
            Player.IsRain = gameMode == PlayState.GameMode.Rain;

            // ゲームクリア時
            Player.IsGameClear = gameMode == PlayState.GameMode.Clear;

            // ゲームオーバー時
            Player.IsGameOver = gameMode == PlayState.GameMode.GameOver && waterDead == false;

            // 溺死のフラグ
            Player.IsGameOverInWater = gameMode == PlayState.GameMode.GameOver && waterDead;

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
            // ポーズ中は移動処理とアニメーションを停止
            enemy.IsGameStop = gameMode == PlayState.GameMode.Pause;

            // プレイ中以外のときはスタンバイ状態にする
            enemy.IsStandby = gameMode != PlayState.GameMode.Play;

            // ステージが回転状態のフラグ
            bool isFall;
            try
            {
                isFall = PlayState.playState.IsFallBox;
            }
            catch
            {
                isFall = true;
            }

            enemy.IsStageRoation = gameMode == PlayState.GameMode.RotationPot && isFall == false;
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

        if(thunder != null)
        {
            thunder.PlayThunder(LightningStrikePoint.transform.position);
        }
    }

    /// <summary>
    /// 落雷の効果を無効にする際に呼び出す処理
    /// </summary>
    public void CancelLightningEffect()
    {
        if(IsCanLightningStrike == false) { return; }
        enemy.CancelElectricMode();
    }

    // Update is called once per frame
    void Update()
    {
        gameMode = GetGameMode();

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
