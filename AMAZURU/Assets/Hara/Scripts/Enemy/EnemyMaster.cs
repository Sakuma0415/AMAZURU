using Enemy;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;

public class EnemyMaster : MonoBehaviour
{
    [SerializeField, Tooltip("エネミーのPrefab")] private EnemyController enemyPrefab = null;
    [SerializeField, Tooltip("乾燥ナマコブロックのPrefab")] private DryEnemy dryEnemyPrefab = null;
    [SerializeField, Tooltip("帯電ナマコのPrefab")] private ElectricEnemy electricEnemyPrefab = null;

    [SerializeField, Header("エネミーデータ")] private EnemyData[] enemyData = null;
    public EnemyData[] EnemyDataArray { set { enemyData = value; } get { return enemyData; } }

    private bool startOperation = false;

    // 敵を格納するオブジェクトのリスト
    private List<GameObject> parentObjects = null;

    /// <summary>
    /// ステージの中心座標
    /// </summary>
    public Vector3 StageCenter { set; private get; } = Vector3.zero;

    /// <summary>
    /// ステージ上のナマコ(エネミー)の情報を格納する
    /// </summary>
    public EnemyController[] Enemies { private set; get; } = null;

    // エネミーの種類情報を格納する
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
    /// ステージが回転している状態のフラグ
    /// </summary>
    public bool IsStageRoation { set; private get; } = false;

    /// <summary>
    /// 初期化、生成処理
    /// </summary>
    public void Init()
    {
        if (enemyData == null && enemyData.Length < 1) { return; }
        transform.position = Vector3.zero;
        Enemies = new EnemyController[enemyData.Length];
        enemyTypes = new EnemyType[enemyData.Length];
        DryEnemies = new List<DryEnemy>();
        ElectricEnemies = new List<ElectricEnemy>();
        parentObjects = new List<GameObject>();

        int count = 0;
        foreach (var data in enemyData)
        {
            // 敵のインスタンスを作成
            Transform parent = GetParent(data.EnemyUpDirection, parentObjects);

            Vector3 startPos = data.UseStartPosSetting ? data.StartPosition : data.MovePlan[0];
            startPos = parent.InverseTransformPoint(startPos);

            Vector3[] movePlanLocal = new Vector3[data.MovePlan.Length];
            for(int i = 0; i < movePlanLocal.Length; i++)
            {
                Vector3 before = data.MovePlan[i];
                Vector3 after = parent.InverseTransformPoint(before);
                movePlanLocal[i] = after;
            }

            Enemies[count] = Instantiate(enemyPrefab, parent);
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
            Enemies[count].MovePlan = movePlanLocal;
            Enemies[count].MoveType = data.MoveType;
            if(data.UseDefaultSetting == false)
            {
                Enemies[count].EnemySpeed = data.NomalSpeed;
                Enemies[count].EnemyWaterSpeed = data.WaterSpeed;
            }
            Enemies[count].StartPosFlag = data.UseStartPosSetting;
            Enemies[count].StageWater = StageWater;
            Enemies[count].StartPosition = startPos;
            Enemies[count].EnemyInit();

            if(enemyTypes[count] == EnemyType.Dry)
            {
                // 乾燥ブロックのインスタンスを作成
                var block = Instantiate(dryEnemyPrefab, parent);
                block.EnemyObject = Enemies[count];
                block.BlockSize = data.Size;
                block.BlockCenterY = data.BlockSetPosY;
                block.ReturnDryMode = data.ReturnBlock;
                block.StageWater = StageWater;
                block.StartPosition = startPos;
                block.EnemyObject.gameObject.SetActive(false);
                block.DryEnemyInit();

                // 管理用のリストに追加
                DryEnemies.Add(block);
            }

            if(enemyTypes[count] == EnemyType.Electric)
            {
                // 帯電ナマコの情報を設定
                var electric = Instantiate(electricEnemyPrefab, Enemies[count].transform);
                electric.gameObject.SetActive(false);
                electric.EnemyObject = Enemies[count];
                int length = Enemies[count].transform.childCount;
                GameObject[] models = new GameObject[length];
                Animator[] animators = new Animator[length];
                for(int i = 0; i < length; i++)
                {
                    Transform objTransform = Enemies[count].transform.GetChild(i);
                    models[i] = objTransform.gameObject;
                    animators[i] = objTransform.GetComponent<Animator>();
                }

                electric.EnemyModels = models;
                electric.EnemyAnimators = animators;

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
                // エネミーの種類が乾燥タイプ以外ならばゲームステートがプレイ以外は移動処理を停止
                enemy.IsMoveStop = IsStandby;
            }
            count++;
        }

        // エネミーの種類が乾燥タイプのとき
        foreach(var dry in DryEnemies)
        {
            // ポーズ中は処理を停止
            dry.IsStop = IsGameStop;

            // ゲームステートがプレイ以外またはアニメーションが実行中の場合は移動処理を停止
            dry.EnemyObject.IsMoveStop = IsStandby || dry.IsDoingAnimation;

            // ステージが回転状態かどうかのフラグ
            dry.IsRotate = IsStageRoation;
        }

        // 帯電ナマコのフラグ管理
        foreach(var electric in ElectricEnemies)
        {
            // ポーズ中はエフェクトを停止
            electric.IsGameStop = IsGameStop;
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

    /// <summary>
    /// 帯電ナマコの帯電化を解除する
    /// </summary>
    public void CancelElectricMode()
    {
        if(ElectricEnemies == null) { return; }

        // 帯電状態のナマコ(敵)を取得して帯電状態を解除する
        foreach(var electric in ElectricEnemies)
        {
            if (electric.IsElectric)
            {
                electric.ElectricMode(false);
            }
        }
    }

    /// <summary>
    /// 敵を格納する親オブジェクトのTransformを取得
    /// </summary>
    /// <param name="up"></param>
    /// <param name="parentList"></param>
    /// <returns></returns>
    private Transform GetParent(EnemyData.UpDirection up, List<GameObject> parentList)
    {
        Vector3 upDirection;
        string objName;
        switch (up)
        {
            case EnemyData.UpDirection.Forward:
                upDirection = Vector3.forward;
                objName = "Forward";
                break;
            case EnemyData.UpDirection.Back:
                upDirection = Vector3.back;
                objName = "Back";
                break;
            case EnemyData.UpDirection.Up:
                upDirection = Vector3.up;
                objName = "Up";
                break;
            case EnemyData.UpDirection.Down:
                upDirection = Vector3.down;
                objName = "Down";
                break;
            case EnemyData.UpDirection.Right:
                upDirection = Vector3.right;
                objName = "Right";
                break;
            default:
                upDirection = Vector3.left;
                objName = "Left";
                break;
        }

        int index = -1;
        for(int i = 0; i < parentList.Count; i++)
        {
            if(parentList[i].name == objName)
            {
                index = i;
                break;
            }
        }

        GameObject parent;
        if(index < 0)
        {
            parent = new GameObject();
            parent.transform.SetParent(gameObject.transform);
            parent.transform.up = upDirection;
            parent.name = objName;
            parentList.Add(parent);
        }
        else
        {
            parent = parentList[index];
        }

        return parent.transform;
    }

    private void Update()
    {
        if(startOperation == false) { return; }

        SetState();

        CheckEnemyFlag();

        GetTargetElectricEnemy();
    }
}
