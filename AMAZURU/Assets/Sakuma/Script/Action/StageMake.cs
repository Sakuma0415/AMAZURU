using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMake : MonoBehaviour
{
    public StageData LoadStageData;
    [SerializeField]
    private GameObject water;
    

    private void StageLoad()
    {
        Instantiate(LoadStageData.stagePrefab);
        GameObject waterObj= Instantiate(water);
        waterObj.transform.localScale = new Vector3(LoadStageData.stageLength,0, LoadStageData.stageWidth)-new Vector3 (0.01f,0,0.01f);
        waterObj.transform.position += new Vector3(0.005f, 0, 0.005f);
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
