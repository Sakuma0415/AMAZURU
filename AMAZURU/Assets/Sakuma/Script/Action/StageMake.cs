using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMake : MonoBehaviour
{
    static public StageData LoadStageData;
    public StageData SLoadStageData;
    [SerializeField]
    private GameObject water;
    [SerializeField]
    private GameObject playerObj;
    [SerializeField]
    private CameraPos cameraPos;
    GameObject waterObj;

    [SerializeField]
    Material waterMaterial;
    [SerializeField]
    O2Controller o2Controller;
    //[SerializeField]
    //DigitalRuby.RainMaker.RainScript[] rainScript;
    [SerializeField]
    AmehurashiManager amehurashiManager;



    private void StageLoad()
    {
        Instantiate(StageMake.LoadStageData.stagePrefab);
        waterObj = Instantiate(water);
        waterObj.transform.localScale = new Vector3(StageMake.LoadStageData.stageSize.x,0.25f, StageMake.LoadStageData.stageSize.z) -new Vector3 (0.01f,0,0.01f);
        waterObj.transform.position += new Vector3(0.005f, 0, 0.005f);
        //waterObj.GetComponent<WaterHi>().rainScript = rainScript;
        waterMaterial.SetFloat("_X", StageMake.LoadStageData.stageSize.x/5);
        waterMaterial.SetFloat("_Y", StageMake.LoadStageData.stageSize.z/6);

        GameObject player = Instantiate(playerObj, StageMake.LoadStageData.startPos-new Vector3 (0.5f,0,0.5f),Quaternion.identity);
        player.GetComponent<PlayerType2>().StageWater = waterObj.GetComponent<WaterHi>(); ;
        //player.GetComponent<PlayerType2>().PlayerInit();
        cameraPos.lookPos = new Vector3(StageMake.LoadStageData.stageSize.x/2, StageMake.LoadStageData.stageSize.y / 2, StageMake.LoadStageData.stageSize.z/2);
        cameraPos.PlayerTransform = player.GetComponent<Transform >();

        o2Controller.playerType2 = player.GetComponent<PlayerType2>();

        amehurashiManager.waterHi = waterObj.GetComponent<WaterHi>();
        amehurashiManager.ManagerSet();
        amehurashiManager.waterStep = SLoadStageData.waterStep;
        amehurashiManager.AmehurashiQuantity  = SLoadStageData.AmehurashiQuantity;
    }

    void FStateSet()
    {
        cameraPos.fAngle = StageMake.LoadStageData.startAngle;
        cameraPos.CameraDisP = StageMake.LoadStageData.CameraDisP;
        cameraPos.CameraDisS = StageMake.LoadStageData.CameraDisS;
    }

    private void Awake()
    {
        if (LoadStageData == null)
        {
            StageMake.LoadStageData = SLoadStageData;
        }
        FStateSet();
    }
    private void Start()
    {

        StageLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void RainSet(float hi)
    {
        waterObj.GetComponent<WaterHi>().HiChange(hi);
    }

}
