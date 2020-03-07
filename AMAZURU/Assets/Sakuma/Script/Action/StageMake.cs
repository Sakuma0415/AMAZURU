using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMake : MonoBehaviour
{
    public StageData LoadStageData;
    [SerializeField]
    private GameObject water;
    [SerializeField]
    private GameObject playerObj;
    [SerializeField]
    private CameraPos cameraPos;

    private void StageLoad()
    {
        Instantiate(LoadStageData.stagePrefab);
        GameObject waterObj= Instantiate(water);
        waterObj.transform.localScale = new Vector3(LoadStageData.stageSize.x,0.25f, LoadStageData.stageSize.y) -new Vector3 (0.01f,0,0.01f);
        waterObj.transform.position += new Vector3(0.005f, 0, 0.005f);

        GameObject player = Instantiate(playerObj,LoadStageData.startPos-new Vector3 (0.5f,0,0.5f),Quaternion.identity);
        cameraPos.lookPos = new Vector3(LoadStageData.stageSize.x/2, LoadStageData.stageSize.y / 2, LoadStageData.stageSize.z/2);
        cameraPos.PlayerTransform = player.GetComponent<Rigidbody >();
    }


    void Start()
    {
        StageLoad();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
