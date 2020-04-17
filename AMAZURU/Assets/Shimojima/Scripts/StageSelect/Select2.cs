using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select2 : MonoBehaviour
{
    [SerializeField]
    private GameObject senterPivot;
    [SerializeField]
    private Vector3 pivotCubeSize;

    private List<GameObject> stages = new List<GameObject>();
    private PrefabStageData[] psd;
    [SerializeField]
    private Vector3 pos;
    [SerializeField]
    private Vector2 scaleAdjust;
    [SerializeField,Range(1,10)]
    private float speed = 1;
    private float angle = 0;
    private bool isRotation = false;

    [System.Serializable]
    public struct ViewStage
    {
        public string name;
        public GameObject stage;
        public bool isSelect;
        public bool isInvisible;
    }

    public ViewStage[] viewStage = new ViewStage[10];

    void Start()
    {
        StageSelectorInit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isRotation = true;
        }
    }

    void FixedUpdate()
    {
        if (!isRotation) { return; }
        StageRotation(1);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void StageSelectorInit()
    {
        psd = Resources.LoadAll<PrefabStageData>("EditData/");
        float angle = 360 / 10;
        int overCount = 1;
        //int i = 0;
        for (int i = 0; i < 10; i++)
        {
            if(i > (psd.Length - 1) * overCount)
            {
                int _i = i - psd.Length;
                stages.Add(psd[_i].stage);
                viewStage[i].stage = Instantiate(stages[i]);
                viewStage[i].name = stages[i].name;
                if (i < 9)
                {
                    viewStage[i].stage.transform.localScale = Vector3.zero;
                    viewStage[i].isInvisible = true;
                    goto Skip;
                }
                StageReSize(psd[_i], i);
                
            }
            else
            {
                stages.Add(psd[i].stage);
                viewStage[i].stage = Instantiate(stages[i]);
                viewStage[i].name = stages[i].name;
                if (i == psd.Length - 1)
                {
                    viewStage[i].stage.transform.localScale = Vector3.zero;
                    viewStage[i].isInvisible = true;
                    goto Skip;
                }
                StageReSize(psd[i], i);
            }

        Skip:
            viewStage[i].stage.transform.position = pos;

            
            viewStage[i].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, angle * (i + 1));
            if(angle * (i + 1) == 72) { viewStage[i].stage.transform.localScale = Vector3.one * 0.8f; }

            if(i > (psd.Length - 1) * (overCount + 1)) { overCount++; }
        }

        //foreach (PrefabStageData obj in psd)
        //{
        //    stages.Add(Instantiate(obj.stage));
        //    stages[i].transform.position = pos;

        //    StageReSize(obj, i);

        //    //stages[i].transform.localScale = defSize;
        //    stages[i].transform.RotateAround(senterPivot.transform.position, Vector3.up, angle * (i + 1));
        //    i++;
        //}
    }

    /// <summary>
    /// ステージのリサイズ
    /// </summary>
    /// <param name="obj">ステージデータアセット</param>
    /// <param name="i">インデックス</param>
    private void StageReSize(PrefabStageData obj, int i)
    {
        if (scaleAdjust.x  <= 0 || scaleAdjust.y <= 0) { return; }
        Vector2 scale = new Vector2(obj.gridCells.x, obj.gridCells.z);
        if (scale.x > scaleAdjust.x || scale.y > scaleAdjust.y)
        {
            int a, b, r;
            if (scale.x > scale.y)
            {
                a = (int)scale.x;
                b = (int)scale.y;
            }
            else
            {
                a = (int)scale.y;
                b = (int)scale.x;
            }

            r = a % b;
            while (r != 0)
            {
                a = b;
                b = r;
                r = a % b;
            }

            float x = scale.x / b;
            float z = scale.y / b;
            float magni;
            if (x > z) { magni = scaleAdjust.x / x; }
            else { magni = scaleAdjust.y / z; }

            Vector2 reSize = new Vector2((x * magni) / scale.x, (z * magni) / scale.y);
            viewStage[i].stage.transform.localScale = new Vector3(reSize.x, 1, reSize.y);
        }
    }

    private void StageRotation(int i)
    {
        for (int j = 0; j < viewStage.Length; j++)
        {
            viewStage[j].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, speed);
        }

        angle += speed;

        if(angle >= 36)
        {
            angle = 0;
            isRotation = false;
        }
    }

    private void CheakAngle()
    {
        for (int i = 0; i < viewStage.Length; i++)
        {
            if(36 + viewStage[i].stage.transform.localEulerAngles.y == 72)
            {
                viewStage[i].isSelect = true;
            }
            else
            {
                viewStage[i].isSelect = false;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(senterPivot.transform.position, pivotCubeSize);
    }
}
