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

    private void StageLoad()
    {
        Instantiate(StageMake.LoadStageData.stagePrefab);
        GameObject waterObj= Instantiate(water);
        waterObj.transform.localScale = new Vector3(StageMake.LoadStageData.stageSize.x,0.25f, StageMake.LoadStageData.stageSize.z) -new Vector3 (0.01f,0,0.01f);
        waterObj.transform.position += new Vector3(0.005f, 0, 0.005f);



        GameObject player = Instantiate(playerObj, StageMake.LoadStageData.startPos-new Vector3 (0.5f,0,0.5f),Quaternion.identity);
        cameraPos.lookPos = new Vector3(StageMake.LoadStageData.stageSize.x/2, StageMake.LoadStageData.stageSize.y / 2, StageMake.LoadStageData.stageSize.z/2);
        cameraPos.PlayerTransform = player.GetComponent<Rigidbody >();

    }


    void Start()
    {
        if (LoadStageData == null)
        {
            StageMake.LoadStageData = SLoadStageData;
        }
        StageLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
