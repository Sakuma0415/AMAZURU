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
    private GameObject playerObj;
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

    //private

    //水のオブジェ
    GameObject waterObj;

    //初期化処理
    private void StageLoad()
    {

        //ステージ生成
        Instantiate(StageMake.LoadStageData.stagePrefab);

        //水生成
        waterObj = Instantiate(water);
        waterObj.transform.localScale = new Vector3(StageMake.LoadStageData.stageSize.x,0.25f, StageMake.LoadStageData.stageSize.z) -new Vector3 (0.01f,0,0.01f);
        waterObj.transform.position += new Vector3(0.005f, 0, 0.005f);
        waterMaterial.SetFloat("_X", StageMake.LoadStageData.stageSize.x/5);
        waterMaterial.SetFloat("_Y", StageMake.LoadStageData.stageSize.z/6);

        //プレイヤー生成
        GameObject player = Instantiate(playerObj, StageMake.LoadStageData.startPos-new Vector3 (0.5f,0,0.5f),Quaternion.identity);
        player.GetComponent<PlayerType2>().StageWater = waterObj.GetComponent<WaterHi>();

        //カメラの設定
        cameraPos.lookPos = new Vector3(StageMake.LoadStageData.stageSize.x/2, StageMake.LoadStageData.stageSize.y / 2, StageMake.LoadStageData.stageSize.z/2);
        cameraPos.PlayerTransform = player.GetComponent<Transform >();

        //酸素管理の設定
        o2Controller.playerType2 = player.GetComponent<PlayerType2>();

        //アメフラシの設定
        amehurashiManager.waterHi = waterObj.GetComponent<WaterHi>();
        amehurashiManager.ManagerSet();
        amehurashiManager.waterStep = StageMake.LoadStageData.waterStep;
        amehurashiManager.AmehurashiQuantity  = StageMake.LoadStageData.AmehurashiQuantity;
    }


    //カメラの優先設定
    void FStateSet()
    {
        cameraPos.fAngle = StageMake.LoadStageData.startAngle;
        cameraPos.CameraDisP = StageMake.LoadStageData.CameraDisP;
        cameraPos.CameraDisS = StageMake.LoadStageData.CameraDisS;
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
