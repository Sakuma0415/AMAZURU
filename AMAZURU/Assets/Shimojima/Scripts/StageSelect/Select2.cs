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
    [SerializeField]
    private Vector3 pos;
    [SerializeField]
    private Vector2 scaleAdjust;
    [SerializeField]
    private GameObject[] viewStage = new GameObject[4];

    void Start()
    {
        StageSelectorInit();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void StageSelectorInit()
    {
        PrefabStageData[] psd = Resources.LoadAll<PrefabStageData>("EditData/");
        float angle = 360 / psd.Length;
        int i = 0;
        foreach (PrefabStageData obj in psd)
        {
            stages.Add(Instantiate(obj.stage));
            stages[i].transform.position = pos;

            StageReSize(obj, i);

            //stages[i].transform.localScale = defSize;
            stages[i].transform.RotateAround(senterPivot.transform.position, Vector3.up, angle * (i + 1));
            i++;
        }
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
            stages[i].transform.localScale = new Vector3(reSize.x, 1, reSize.y);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(senterPivot.transform.position, pivotCubeSize);
    }
}
