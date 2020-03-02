using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageEditor : MonoBehaviour
{
    [Tooltip("グリッドの数　X * Y * Z")]
    public int cells;

    public Vector3Int cellNum;
    private Vector3Int tempCnum;

    [SerializeField,Tooltip("参照するGridObject")]
    private GameObject gridObj;

    [Tooltip("Gridオブジェクトの参照管理")]
    private GameObject[,,] gridPos;

    [Tooltip("シーン内のオブジェクトを削除するためのルートオブジェクト")]
    private GameObject gridRoot;

    void Start()
    {
        CreateGrid();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            ChangeSelectObj();
        }
    }

    /// <summary>
    /// Gridの作成
    /// </summary>
    public void CreateGrid()
    {
        if (gridRoot != null) { Destroy(gridRoot); }

        gridRoot = new GameObject();
        gridRoot.name = "GridRootObj";
        gridPos = new GameObject[cells, cells, cells];
        float s = gridObj.transform.localScale.x;

        for (int i = 0; i < cells; i++)
        {
            for (int j = 0; j < cells; j++)
            {
                for (int k = 0; k < cells; k++)
                {
                    GameObject obj = Instantiate(gridObj);
                    obj.transform.localPosition = new Vector3((i - 1) * s, j * s, k * s);
                    gridPos[i, j, k] = obj;
                    obj.transform.parent = gridRoot.transform;
                }
            }
        }
    }

    /// <summary>
    /// Gridの選択
    /// </summary>
    public void ChangeSelectObj()
    {
        Vector3Int cNum = new Vector3Int(cellNum.x - 1, cellNum.y - 1, cellNum.z - 1);
        if (cellNum.x <= 0 || cellNum.y <= 0 || cellNum.z <= 0) { return; }

        if(tempCnum != null) { gridPos[tempCnum.x, tempCnum.y, tempCnum.z].GetComponent<HighlightObject>().IsSelect = false; }
        gridPos[cNum.x, cNum.y, cNum.z].GetComponent<HighlightObject>().IsSelect = true;
        tempCnum = cNum;
    }
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(StageEditor))]
//public class StageEditorCustom : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        StageEditor stageEditor = target as StageEditor;
//        base.OnInspectorGUI();
//    }
//}
//#endif
