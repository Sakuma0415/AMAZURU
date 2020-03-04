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
    private bool IsInputAnyKey { get; set; } = false;
    private float horizontal, vertical = 0;
    void Start()
    {
        CreateGrid();
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        EditorInput();
    }

    /// <summary>
    /// ステージエディット時のキー入力処理
    /// </summary>
    private void EditorInput()
    {
        if (Input.GetKey(KeyCode.LeftShift)) { InputDepth();}
        //InputHorizontal();
        //InputVertical();
    }

    private void InputDepth()
    {
        if (vertical > 0)
        {
            cellNum.z++;
            if(cellNum.z == cells) { cellNum.z = 0; }
            ChangeSelectObj(cellNum);
        }
        else if (vertical < 0)
        {
            cellNum.z--;
            if (cellNum.z == -1) { cellNum.z = cells - 1; }
            ChangeSelectObj(cellNum);
        }
    }

    /// <summary>
    /// 横方向の入力が行われた時の処理
    /// </summary>
    private void InputHorizontal()
    {
        if (horizontal > 0)
        {
            cellNum.x++;
            if (cellNum.x == cells) { cellNum.x = 0; }
            ChangeSelectObj(cellNum);
        }
        else if (horizontal < 0)
        {
            cellNum.x--;
            if (cellNum.x == -1) { cellNum.x = cells - 1; }
            ChangeSelectObj(cellNum);
        }
    }

    /// <summary>
    /// 縦方向の入力が行われた時の処理
    /// </summary>
    private void InputVertical()
    {
        if (vertical > 0)
        {
            cellNum.y++;
            if (cellNum.y == cells) { cellNum.y = 0; }
            ChangeSelectObj(cellNum);
        }
        else if (vertical < 0)
        {
            cellNum.y--;
            if (cellNum.y == -1) { cellNum.y = cells - 1; }
            ChangeSelectObj(cellNum);
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
    public void ChangeSelectObj(Vector3Int cNum)
    {
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
