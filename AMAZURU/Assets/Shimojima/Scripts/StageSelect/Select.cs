using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    private List<GameObject> stages = new List<GameObject>();
    [SerializeField]
    private GameObject[] viewStage = new GameObject[4];
    private int stageCount = 0;
    private int rotateAngle = 0;

    [SerializeField]
    private Vector3 pos;
    [SerializeField,Range(1, 10)]
    private float rotateSpeed = 1;
    private float sumRotateNum;
    [SerializeField]
    private float[] _RotateNums = new float[4] 
    {
        270,
        180,
        90,
        0
    };
    private bool IsRotate = false;
    private bool doOnce = false;

    void Start()
    {
        GameObject[] objs = Resources.LoadAll<GameObject>("Prefabs/Stage");
        foreach (GameObject obj in objs)
        {
            stages.Add(obj);
        }
        SelectStageInit();
        viewStage[1].transform.RotateAround(transform.position, Vector3.up, 270);
        viewStage[2].transform.RotateAround(transform.position, Vector3.up, 180);
        viewStage[3].transform.RotateAround(transform.position, Vector3.up, 90);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            rotateAngle = 0;
            IsRotate = true;
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            rotateAngle = 1;
            IsRotate = true;
        }
    }

    private void FixedUpdate()
    {
        if (!IsRotate) { return; }
        StageRotate(rotateAngle);
    }

    /// <summary>
    /// ステージセレクターの初期化
    /// </summary>
    private void SelectStageInit()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject obj;

            if(i > stages.Count) { obj = Instantiate(stages[stages.Count - 1]); }
            else { obj = Instantiate(stages[i]); }

            obj.transform.localPosition = pos;
            obj.transform.localEulerAngles = new Vector3(0, -20, 0);
            viewStage[i] = obj;
            stageCount++;
            if(stageCount > stages.Count) { stageCount = stages.Count - 1; }
        }
    }

    private void StageRotate(int angle)
    {
        for (int i = 0; i < 4; i++)
        {
            if(rotateAngle == 0) 
            {
                viewStage[i].transform.RotateAround(transform.position, Vector3.up, rotateSpeed);
                _RotateNums[i] += rotateSpeed; 
            }
            else if(rotateAngle == 1) 
            {
                viewStage[i].transform.RotateAround(transform.position, Vector3.up, -rotateSpeed);
                _RotateNums[i] -= rotateSpeed; 
            }
        }

        sumRotateNum += rotateSpeed;

        if (sumRotateNum >= 90)
        {
            for (int i = 0; i < 4; i++)
            {
                if(_RotateNums[i] >= 360)
                {
                    _RotateNums[i] = 0;
                    
                    Destroy(viewStage[i]);
                    viewStage[i] = Instantiate(stages[stageCount]);
                    viewStage[i].transform.localPosition = pos;
                    viewStage[i].transform.localEulerAngles = new Vector3(0, -20, 0);
                    viewStage[i].transform.RotateAround(transform.position, Vector3.up, 90);
                    viewStage[i].SetActive(false);
                    //if(rotateAngle == 1) { continue; }
                    stageCount++;
                    if(stageCount == stages.Count - 1) { stageCount = 0; }
                    else if(stageCount == 0) { stageCount = stages.Count - 1; }
                }
                else if(_RotateNums[i] >= 90)
                {
                    viewStage[i].SetActive(true);
                }
            }

            sumRotateNum = 0;
            IsRotate = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
