using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクションシーン開始時の初期設定用のクラス
/// </summary>
public class StageMake : MonoBehaviour
{
    //読み込むステージ
    static public StageData LoadStageData;
    
    [Header("設定項目")]
    //シーンのテスト再生時に読み込むステージ
    [SerializeField]
    private StageData SLoadStageData;
    //水のオブジェのプレハブ
    [SerializeField]
    private GameObject water;
    //プレイヤーのプレハブ
    [SerializeField]
    private CharacterMaster master;
    //カメラのクラス
    [SerializeField]
    private CameraPos cameraPos;
    //水のマテリアル
    [SerializeField]
    Material waterMaterial;
    //酸素管理のクラス
    [SerializeField]
    O2Controller o2Controller;
    //アメフラシ管理のクラス
    [SerializeField]
    AmehurashiManager amehurashiManager;
    [SerializeField]
    GameObject RainEf;
    [SerializeField]
    RainEfController RainEfController;
    [SerializeField]
    VewPos vewPos;
    [SerializeField]
    Progress progress;


    //private

    //水のオブジェ
    GameObject waterObj;

    //初期化処理
    private void StageLoad()
    {

        //ステージ生成
        GameObject StageObj = Instantiate(StageMake.LoadStageData.stagePrefab);
        GameObject StageCenter = new GameObject();
        StageCenter.transform.position = StageMake.LoadStageData.stageSize/2;
        StageCenter.name = "StageCenter";
        StageObj.transform.parent= StageCenter.transform;
        master.gameObject.transform.parent = StageCenter.transform;
        PlayState.playState.stageObj = StageCenter;
        waterObj.transform.parent = StageCenter.transform;
        //プレイヤー生成
        //GameObject player = Instantiate(playerObj, StageMake.LoadStageData.startPos-new Vector3 (0.5f,0,0.5f),Quaternion.identity);
        master.SpawnPlayer(StageMake.LoadStageData.startPos - new Vector3(0.5f, 0, 0.5f), waterObj.GetComponent<WaterHi>(), o2Controller, LoadStageData);
        Progress.progress.animator  = master.Player.transform.GetChild(1) .GetComponent<Animator >();
        PlayState.playState.character = master.Player.GetComponent<CharacterController>();

        //カメラの設定
        cameraPos.lookPos = new Vector3(StageMake.LoadStageData.stageSize.x/2, StageMake.LoadStageData.stageSize.y / 2, StageMake.LoadStageData.stageSize.z/2);
        cameraPos.PlayerTransform = master.Player.transform;
        vewPos.PlayerTransform = master.Player.transform;

        //酸素管理の設定
        o2Controller.master = master;

        //アメフラシの設定
        amehurashiManager.waterHi = waterObj.GetComponent<WaterHi>();
        amehurashiManager.ManagerSet();
        amehurashiManager.waterStep = StageMake.LoadStageData.waterStep;
        amehurashiManager.AmehurashiQuantity  = StageMake.LoadStageData.AmehurashiQuantity;

        //雨のエフェクト生成
        GameObject rainef =Instantiate(RainEf);
        rainef.transform.position = new Vector3(StageMake.LoadStageData.stageSize.x / 2, 20, StageMake.LoadStageData.stageSize.y / 2);
        ParticleSystem ps= rainef.GetComponent<ParticleSystem>();
        var sh = ps.shape;
        sh.scale =new Vector3 (StageMake.LoadStageData.stageSize.x, StageMake.LoadStageData.stageSize.y,0);
        var em=ps.emission;
        em.rateOverTime = 0;
        RainEfController.ps = ps;
        RainEfController.bas = StageMake.LoadStageData.stageSize.x * StageMake.LoadStageData.stageSize.y;
    }


    //カメラの優先設定
    void FStateSet()
    {
        cameraPos.fAngle = StageMake.LoadStageData.startAngle;
        cameraPos.CameraDisP = StageMake.LoadStageData.CameraDisP;
        cameraPos.CameraDisS = StageMake.LoadStageData.CameraDisS;


        //水生成
        waterObj = Instantiate(water);
        waterObj.transform.localScale = new Vector3(StageMake.LoadStageData.stageSize.x, 0.25f, StageMake.LoadStageData.stageSize.z) - new Vector3(0.02f, 0, 0.02f);
        waterObj.transform.position += new Vector3(0.01f, 0, 0.01f);
        waterMaterial.SetFloat("_X", StageMake.LoadStageData.stageSize.x / 5);
        waterMaterial.SetFloat("_Y", StageMake.LoadStageData.stageSize.z / 6);
        progress.waterHi = waterObj.GetComponent<WaterHi >();
    }

    //開始時の優先処理
    private void Awake()
    {
        if (LoadStageData == null)
        {
            StageMake.LoadStageData = SLoadStageData;
        }
        FStateSet();
    }

    //開始時処理
    private void Start()
    {
        StageLoad();
    }
}
