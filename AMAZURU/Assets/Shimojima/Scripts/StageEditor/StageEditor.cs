using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageEditor : MonoBehaviour
{
    //[HideInInspector]
    public bool loadStage;
    [HideInInspector]
    public PrefabStageData data;

    [Tooltip("グリッドの数　X * Y * Z")]
    public Vector3Int cells;
    private float posAdjust = 0.5f;

    public Vector3Int cellNum;
    private Vector3Int tempCnum = Vector3Int.zero;

    [SerializeField,Tooltip("参照するGridObject")]
    private GameObject gridObj;

    [HideInInspector,Tooltip("Gridオブジェクトの参照管理")]
    public GameObject[,,] gridPos;

    [Tooltip("シーン内のオブジェクトを削除するためのルートオブジェクト")]
    private GameObject gridRoot;

    [SerializeField,Tooltip("ステージに使う参照オブジェクト")]
    private GameObject[] referenceObject;
    private int refObjIndex = 0;
    
    public string stageName;
    [HideInInspector,Tooltip("保存するステージのルートオブジェクト")]
    public GameObject stageRoot;
    private GameObject stageObj;
    [SerializeField]
    private GameObject guideObj;
    [SerializeField]
    private Vector3 objAngle;

    private bool IsInputAnyKey { get; set; } = false;
    private float horizontal, vertical = 0;
    void Start()
    {
    }

    void Update()
    {
        CheakKeyDownForMoveKey();
        EditorInput();
    }

    /// <summary>
    /// ステージエディット時のキー入力処理
    /// </summary>
    private void EditorInput()
    {

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftShift)) 
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (loadStage)
                {
                    data.stage = (GameObject)PrefabUtility.SaveAsPrefabAssetAndConnect(stageRoot, "Assets/Shimojima/Prefabs/" + stageName + ".prefab", InteractionMode.UserAction);
                    return;
                }
                AssetDatabase.CreateAsset(data, "Assets/Shimojima/PrefabStageData.asset");
                data.gridCells = cells;
                data.stageName = stageName;
                data.stage = (GameObject)PrefabUtility.SaveAsPrefabAssetAndConnect(stageRoot, "Assets/Shimojima/Prefabs/" + stageName + ".prefab", InteractionMode.UserAction);
                AssetDatabase.SaveAssets();
            }
            return;
        }
#endif

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SetStageObject(stageObj);
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeStageObject();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            objAngle.y += -90;
            guideObj.transform.localEulerAngles = objAngle;
        }
        if (IsInputAnyKey) { return; }
        InputHorizontal();
        InputVertical();
        InputDepth();
    }
    #region 入力関連の関数

    /// <summary>
    /// 移動に関するキー入力が行われているか
    /// </summary>
    private void CheakKeyDownForMoveKey()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        if (horizontal <= 0.5f && vertical <= 0.5f && 
            !Input.GetKey(KeyCode.Z) && !Input.GetKey(KeyCode.X)) { IsInputAnyKey = false; }
    }

    /// <summary>
    /// 奥行んお入力が行われた時の処理
    /// </summary>
    private void InputDepth()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            cellNum.z++;
            if(cellNum.z == cells.z) { cellNum.z = 0; }
            SelectGridObject(cellNum);
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            cellNum.z--;
            if (cellNum.z == -1) { cellNum.z = cells.z - 1; }
            SelectGridObject(cellNum);
        }
    }

    /// <summary>
    /// 横方向の入力が行われた時の処理
    /// </summary>
    private void InputHorizontal()
    {
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            cellNum.x++;
            if (cellNum.x == cells.x) { cellNum.x = 0; }
            SelectGridObject(cellNum);
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            cellNum.x--;
            if (cellNum.x == -1) { cellNum.x = cells.x - 1; }
            SelectGridObject(cellNum);
        }
    }

    /// <summary>
    /// 縦方向の入力が行われた時の処理
    /// </summary>
    private void InputVertical()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            cellNum.y++;
            if (cellNum.y == cells.y) { cellNum.y = 0; }
            SelectGridObject(cellNum);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            cellNum.y--;
            if (cellNum.y == -1) { cellNum.y = cells.y - 1; }
            SelectGridObject(cellNum);
        }
    }
    #endregion

    /// <summary>
    /// Gridの作成
    /// </summary>
    public void CreateGrid()
    {
        if (loadStage) { goto CreateGrid; }
        if (gridRoot != null) { Destroy(gridRoot); }
        data = (PrefabStageData)ScriptableObject.CreateInstance("PrefabStageData");
        data.gridCells = cells;

    CreateGrid:

        stageRoot = new GameObject();
        stageRoot.transform.position = Vector3.zero;
        stageRoot.name = "Stage";
        gridRoot = new GameObject();
        gridRoot.name = "GridRootObj";

        if (!loadStage) { gridPos = new GameObject[cells.x, cells.y, cells.z]; }

        float s = gridObj.transform.localScale.x;

        for (int i = 0; i < cells.x; i++)
        {
            for (int j = 0; j < cells.y; j++)
            {
                for (int k = 0; k < cells.z; k++)
                {
                    GameObject obj = Instantiate(gridObj);
                    obj.transform.localPosition = new Vector3((i + posAdjust) * s, (j + posAdjust) * s, (k + posAdjust) * s);
                    gridPos[i, j, k] = obj;
                    obj.transform.parent = gridRoot.transform;
                }
            }
        }
        gridPos[0, 0, 0].GetComponent<HighlightObject>().IsSelect = true;

        stageObj = referenceObject[0];
        GameObject o = Instantiate(stageObj);
        o.transform.parent = guideObj.transform;
        o.GetComponent<Renderer>().material.color = Color.yellow;
        guideObj.transform.localPosition = new Vector3(posAdjust, posAdjust, posAdjust);
    }

    /// <summary>
    /// Gridの選択
    /// </summary>
    public void SelectGridObject(Vector3Int cNum, bool isCtrlKeyDown = false)
    {
        if (isCtrlKeyDown) { gridPos[cNum.x, cNum.y, cNum.z].GetComponent<HighlightObject>().IsSelect = true; goto Compleat; }
        if(tempCnum != null) { gridPos[tempCnum.x, tempCnum.y, tempCnum.z].GetComponent<HighlightObject>().IsSelect = false; }
        gridPos[cNum.x, cNum.y, cNum.z].GetComponent<HighlightObject>().IsSelect = true;
        guideObj.transform.position = gridPos[cNum.x, cNum.y, cNum.z].transform.position;
        tempCnum = cNum;
        Compleat:
        IsInputAnyKey = true;
    }

    /// <summary>
    /// 設置するステージオブジェクトの変更
    /// </summary>
    private void ChangeStageObject()
    {
        refObjIndex++;
        if(refObjIndex == referenceObject.Length) { refObjIndex = 0; }

        stageObj = referenceObject[refObjIndex];
        Destroy(guideObj.transform.GetChild(0).gameObject);
        GameObject o = Instantiate(stageObj);
        o.transform.parent = guideObj.transform;
        o.GetComponent<Renderer>().material.color = Color.yellow;
        o.transform.localPosition = Vector3.zero;
        o.transform.localRotation = referenceObject[refObjIndex].transform.localRotation;
    }

    /// <summary>
    /// ステージオブジェクトの設置
    /// </summary>
    /// <param name="cNum">グリッドのセル番号</param>
    /// <param name="obj">設置するゲームオブジェクト</param>
    private void SetStageObject(GameObject obj)
    {
        GameObject o = Instantiate(obj);
        o.name = obj.name;
        o.transform.localPosition = guideObj.transform.localPosition;
        o.transform.localEulerAngles += objAngle;
        o.transform.parent = stageRoot.transform;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(StageEditor))]
public class StageEditorCustom : Editor
{
    PrefabStageData pre;
    public override void OnInspectorGUI()
    {
        StageEditor stageEditor = target as StageEditor;
        base.OnInspectorGUI();
        pre = (PrefabStageData)EditorGUILayout.ObjectField("読み込むステージ",pre,typeof(ScriptableObject),false);
        if (GUILayout.Button("NewStage"))
        {
            if (!EditorApplication.isPlaying) { return; }
            stageEditor.CreateGrid();
        }

        if (GUILayout.Button("LoadStage"))
        {
            if (!EditorApplication.isPlaying) { return; }
            LoadStage(stageEditor);
        }
    }

    /// <summary>
    /// ステージの読み込み
    /// </summary>
    /// <param name="stageEditor">ベースクラス</param>
    private void LoadStage(StageEditor stageEditor)
    {
        stageEditor.loadStage = true;
        stageEditor.stageName = pre.stageName;
        GameObject o = Instantiate(pre.stage);
        stageEditor.gridPos = new GameObject[pre.gridCells.x, pre.gridCells.y, pre.gridCells.z];
        stageEditor.data = pre;
        stageEditor.CreateGrid();

    ReStart:
        foreach (Transform child in o.transform)
        {
            child.gameObject.transform.parent = stageEditor.stageRoot.transform;
        }

        if (o.transform.childCount != 0) { goto ReStart; }
        Destroy(o);
    }
}
#endif
