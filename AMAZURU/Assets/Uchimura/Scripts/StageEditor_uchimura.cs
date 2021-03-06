﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageEditor_uchimura : MonoBehaviour
{
    /// <summary>
    /// 範囲選択モードの状態
    /// </summary>
    private enum RangeSelectionState
    {
        OFF = 0,
        ON,
        Stay
    }

    private RangeSelectionState rangeSelectionState = RangeSelectionState.OFF;

    public PrefabStageData Data { get; set; }

    [Tooltip("ステージ名")]
    public string stageName;

    //[HideInInspector]
    public bool loadStage;
    public bool isSave;
    [HideInInspector]
    public bool isCreateStage;

    [Tooltip("グリッドの数　X * Y * Z")]
    public Vector3Int cells;
    private float posAdjust = 0.5f;

    public Vector3Int cellNum;
    private Vector3Int tempCnum = Vector3Int.zero;

    [Tooltip("Gridオブジェクトの参照管理")]
    public GameObject[,,] gridPos;
    public GameObject[,,] _StageObjects;
    private Vector3Int _tempIndex;

    [Header("-以下変更禁止-")]

    [SerializeField, Tooltip("参照するGridObject")]
    private GameObject gridObj;
    [SerializeField,Tooltip("配置場所を視認し易くするためのオブジェクト")]
    private GameObject guideObj;
    [Tooltip("シーン内のオブジェクトを削除するためのルートオブジェクト")]
    private GameObject gridRoot;

    [HideInInspector, Tooltip("保存するステージのルートオブジェクト")]
    public GameObject stageRoot;
    
    [SerializeField,Tooltip("ステージに使う参照オブジェクト")]
    private GameObject[] referenceObject;
    private int refObjIndex = 0;
    [Tooltip("配置するオブジェクト")]
    private GameObject stageObj;
    
    private Vector3 objAngle;

    private bool IsInputAnyKey { get; set; } = false;
    private float horizontal, vertical = 0;

    void Update()
    {
        if(Data != null && Data.stageName != stageName)
        {
            StageDataIncetance();
        }

        if (!isCreateStage) { return; }
        CheakKeyDownForMoveKey();
        EditorInput();
    }

    /// <summary>
    /// ステージエディット時のキー入力処理
    /// </summary>
    private void EditorInput()
    {
        SetOrDeleteStageObject();
        RangeSelection();

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
    /// ステージオブジェクトの設置削除の入力が行われた時の処理
    /// </summary>
    private void SetOrDeleteStageObject()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Array3DForLoop(cells, 2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Array3DForLoop(cells, 3);
        }
    }

    /// <summary>
    /// 範囲選択モードの切り替え
    /// </summary>
    private void RangeSelection()
    {
        if (Input.GetKey(KeyCode.LeftShift)) { rangeSelectionState = RangeSelectionState.ON; }
        if (Input.GetKeyUp(KeyCode.LeftShift)) { rangeSelectionState = RangeSelectionState.Stay; }
    }

    /// <summary>
    /// 奥行の入力が行われた時の処理
    /// </summary>
    private void InputDepth()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            cellNum.z++;
            if(cellNum.z == cells.z) { cellNum.z = 0; }
            SelectGridObject(cellNum);
        }
        else if (Input.GetKeyDown(KeyCode.S))
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
        if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            cellNum.y++;
            if (cellNum.y == cells.y) { cellNum.y = 0; }
            SelectGridObject(cellNum);
        }
        else if (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            cellNum.y--;
            if (cellNum.y == -1) { cellNum.y = cells.y - 1; }
            SelectGridObject(cellNum);
        }
    }
    #endregion

    /// <summary>
    /// 編集するステージの初期化
    /// </summary>
    public void EditStageInit()
    {
        if (loadStage) { goto CreateGrid; }
        if (gridRoot != null) { Destroy(gridRoot); }
        StageDataIncetance();

    CreateGrid:

        stageRoot = new GameObject();
        stageRoot.name = "Stage";
        gridRoot = new GameObject();
        gridRoot.name = "GridRootObj";

        if (!loadStage) { gridPos = new GameObject[cells.x, cells.y, cells.z];
                          _StageObjects = new GameObject[cells.x, cells.y, cells.z];
                        }

        float s = gridObj.transform.localScale.x;
        Array3DForLoop(cells, 0, s);
        
        gridPos[0, 0, 0].GetComponent<HighlightObject>().IsSelect = true;

        stageObj = referenceObject[0];
        //GuidObjectの生成と初期化
        Instantiate(stageObj).AddComponent<GuidObjectInit>().InitGuidObject(guideObj, referenceObject[0], gridPos[cellNum.x,cellNum.y,cellNum.z]);
        guideObj.transform.localPosition = new Vector3(posAdjust, posAdjust, posAdjust);
    }

    /// <summary>
    /// ステージの保存
    /// </summary>
    public void StageSave()
    {
#if UNITY_EDITOR
        if (loadStage) { goto CreatePrefab; }
        _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z].SetActive(true);

        if(stageName == "") { stageName = "stageName"; }
        Data.stageName = stageName;

        if (isSave)
        {
            AssetDatabase.DeleteAsset("Assets/Shimojima/EditData_" + stageName + ".asset");
        }

        Data.stage = (GameObject)PrefabUtility.SaveAsPrefabAssetAndConnect(stageRoot, "Assets/Shimojima/Prefabs/" + stageName + ".prefab", InteractionMode.UserAction);

        AssetDatabase.CreateAsset(Data, "Assets/Shimojima/EditData_" + stageName + ".asset");

        Array3DForLoop(cells, 1);

        return;

    CreatePrefab:
        _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z].SetActive(true);
        Data.stage = (GameObject)PrefabUtility.SaveAsPrefabAssetAndConnect(stageRoot, "Assets/Shimojima/Prefabs/" + stageName + ".prefab", InteractionMode.UserAction);
        AssetDatabase.SaveAssets();
#endif
    }

    /// <summary>
    /// Gridの選択
    /// </summary>
    public void SelectGridObject(Vector3Int cNum, bool isCtrlKeyDown = false)
    {
        if (rangeSelectionState == RangeSelectionState.Stay)
        {
            rangeSelectionState = RangeSelectionState.OFF;
            foreach(GameObject obj in gridPos)
            {
                obj.GetComponent<HighlightObject>().IsSelect = false;
            }

            gridPos[cellNum.x, cellNum.y, cellNum.z].GetComponent<HighlightObject>().IsSelect = true;
        }

        if(tempCnum != null && rangeSelectionState == RangeSelectionState.OFF) { gridPos[tempCnum.x, tempCnum.y, tempCnum.z].GetComponent<HighlightObject>().IsSelect = false; }

        //GuideObjectの設定
        GameObject hObject = gridPos[cNum.x, cNum.y, cNum.z];
        hObject.GetComponent<HighlightObject>().IsSelect = true;
        if (hObject.GetComponent<HighlightObject>().IsAlreadyInstalled) { guideObj.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.red; }
        else { guideObj.transform.GetChild(1).GetComponent<Renderer>().material.color = referenceObject[refObjIndex].GetComponent<Renderer>().sharedMaterial.color; }
        guideObj.transform.position = gridPos[cNum.x, cNum.y, cNum.z].transform.position;

        if(rangeSelectionState != RangeSelectionState.OFF) { goto Skip; }
        MakeObjectSkeleton();
        tempCnum = cNum;
     Skip:
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
        Destroy(guideObj.transform.GetChild(1).gameObject);
        Instantiate(stageObj).AddComponent<GuidObjectInit>().InitGuidObject(guideObj, referenceObject[refObjIndex], gridPos[cellNum.x, cellNum.y, cellNum.z]);
    }

    /// <summary>
    /// ステージオブジェクトの設置
    /// </summary>
    /// <param name="cNum">グリッドのセル番号</param>
    /// <param name="obj">設置するゲームオブジェクト</param>
    private void SetStageObject(GameObject obj, Vector3Int cellIndex)
    {
        if (_StageObjects[cellIndex.x, cellIndex.y, cellIndex.z] != null) { Debug.Log("既にオブジェクトが設置されています"); return; }
        GameObject o = Instantiate(obj);
        o.name = obj.name;
        o.transform.localPosition = gridPos[cellIndex.x,cellIndex.y,cellIndex.z].transform.localPosition;
        o.transform.localEulerAngles += objAngle;
        o.transform.parent = stageRoot.transform;
        o.AddComponent<MyCellIndex>().cellIndex = cellIndex;
        _StageObjects[cellIndex.x, cellIndex.y, cellIndex.z] = o;
        gridPos[cellIndex.x, cellIndex.y, cellIndex.z].GetComponent<HighlightObject>().IsAlreadyInstalled = true;
        guideObj.transform.GetChild(1).GetComponent<Renderer>().material.color = Color.red;
        if(rangeSelectionState == RangeSelectionState.Stay) { return; }
        MakeObjectSkeleton();
    }

    /// <summary>
    /// ステージオブジェクトの削除
    /// </summary>
    private void DeleteStageObject(Vector3Int cellIndex)
    {
        if (_StageObjects[cellIndex.x, cellIndex.y, cellIndex.z] == null) { Debug.Log("削除できるオブジェクトがありません"); return; }
        Debug.Log(_StageObjects[cellIndex.x, cellIndex.y, cellIndex.z].name + "を削除しました");
        Destroy(_StageObjects[cellIndex.x, cellIndex.y, cellIndex.z]);
        gridPos[cellIndex.x, cellIndex.y, cellIndex.z].GetComponent<HighlightObject>().IsAlreadyInstalled = false;
        guideObj.transform.GetChild(1).GetComponent<Renderer>().material.color = referenceObject[refObjIndex].GetComponent<Renderer>().sharedMaterial.color;
    }

    /// <summary>
    /// <para>ステージオブジェクトを非表示にする</para>
    /// <para>または表示する</para>
    /// </summary>
    private void MakeObjectSkeleton()
    {
        if(rangeSelectionState == RangeSelectionState.ON) { goto Skip; }
        else if(rangeSelectionState == RangeSelectionState.Stay)
        {
            foreach (GameObject obj in _StageObjects)
            {
                if(_StageObjects[cellNum.x, cellNum.y, cellNum.z] != null) { obj.SetActive(true); }
            }

            _StageObjects[cellNum.x, cellNum.y, cellNum.z].SetActive(false);
        }
        if (_tempIndex != cellNum && _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z] != null) 
        { _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z].SetActive(true); }

    Skip:
        if(_StageObjects[cellNum.x, cellNum.y, cellNum.z] != null) 
        {
            _StageObjects[cellNum.x, cellNum.y, cellNum.z].SetActive(false);
            _tempIndex = cellNum;
        }
    }

    private void StageDataIncetance()
    {
        Data = (PrefabStageData)ScriptableObject.CreateInstance("PrefabStageData");
        Data.gridCells = cells;
    }

    /// <summary>
    /// 3次元配列の処理
    /// <para>processingIndexに入る値によって処理が変わります</para>
    /// <para>0 = Gridの生成　1 = セーブ処理　2 = 設置処理　3 = 削除処理</para>
    /// </summary>
    /// <param name="tArray">ループ処理の回数</param>
    /// <param name="processingIndex">関数の指定</param>
    /// <param name="size">Grid生成時のグリッドの１辺の長さ</param>
    private void Array3DForLoop(Vector3Int tArray, int processingIndex, float size = 1)
    {
        if(processingIndex < 0 || processingIndex > 3) { Debug.Log("0 ～ 3の間で処理を決定してください"); return; }

        GameObject _obj = new GameObject();
        _obj.name = "Stage";
        if(processingIndex != 1) { Destroy(_obj); }

        for (int i = 0; i < tArray.x; i++)
        {
            for (int j = 0; j < tArray.y; j++)
            {
                for (int k = 0; k < tArray.z; k++)
                {
                    switch (processingIndex)
                    {
                        case 0:
                            GridInit(i, j, k, size); 
                            break;
                        case 1:
                            AdminStageObjectArrayReInstantiate(i, j, k, _obj);
                            break;
                        case 2:
                            if (gridPos[i, j, k].GetComponent<HighlightObject>().IsSelect)
                            {
                                SetStageObject(stageObj, new Vector3Int(i,j,k));
                            }

                            if(new Vector3Int(i,j,k) != cellNum) { gridPos[i, j, k].GetComponent<HighlightObject>().IsSelect = false; }
                            else if(new Vector3Int(i, j, k) == cellNum) { tempCnum = new Vector3Int(i, j, k); }

                            break;
                        case 3:
                            if (gridPos[i, j, k].GetComponent<HighlightObject>().IsSelect)
                            {
                                DeleteStageObject(new Vector3Int(i, j, k));
                            }

                            if (new Vector3Int(i, j, k) != cellNum) { gridPos[i, j, k].GetComponent<HighlightObject>().IsSelect = false; }
                            else if (new Vector3Int(i, j, k) == cellNum) { tempCnum = new Vector3Int(i, j, k); }

                            break;
                    }
                }
            }
        }

        if (processingIndex == 2 || processingIndex == 3) { rangeSelectionState = RangeSelectionState.OFF; }
        if (processingIndex != 1) { return; }
        //AssetDatabase.SaveAssets();
        StageDataIncetance();

        Destroy(stageRoot);
        stageRoot = _obj;
        isSave = true;
    }

    /// <summary>
    /// グリッドの生成
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="k"></param>
    /// <param name="s"></param>
    private void GridInit(int i, int j, int k, float s)
    {
        GameObject obj = Instantiate(gridObj);
        obj.transform.localPosition = new Vector3((i + posAdjust) * s, (j + posAdjust) * s, (k + posAdjust) * s);
        gridPos[i, j, k] = obj;
        obj.transform.parent = gridRoot.transform;
    }

    /// <summary>
    /// セーブ時にプレファブアセット化したオブジェクトを削除して、同配置のステージを生成
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="k"></param>
    /// <param name="_obj"></param>
    private void AdminStageObjectArrayReInstantiate(int i, int j, int k, GameObject _obj)
    {
        if (_StageObjects[i, j, k] == null) { return; }
        GameObject obj = Instantiate(_StageObjects[i, j, k]);
        obj.transform.parent = _obj.transform;
        _StageObjects[i, j, k] = obj;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(StageEditor_uchimura))]
public class StageEditor_uchimuraCustom : Editor
{
    PrefabStageData pre;
    public override void OnInspectorGUI()
    {
        StageEditor_uchimura stageEditor = target as StageEditor_uchimura;
        base.OnInspectorGUI();
        GUILayout.Label("-以下変更可-");
        pre = (PrefabStageData)EditorGUILayout.ObjectField("読み込むステージ", pre, typeof(ScriptableObject), false);
        if (GUILayout.Button("NewStage"))
        {
            if (!EditorApplication.isPlaying) { return; }
            stageEditor.EditStageInit();
            stageEditor.isCreateStage = true;
        }

        if (GUILayout.Button("LoadStage"))
        {
            if (!EditorApplication.isPlaying) { return; }
            LoadStage(stageEditor);
            stageEditor.isCreateStage = true;
        }
    }

    /// <summary>
    /// ステージの読み込み
    /// </summary>
    /// <param name="stageEditor">ベースクラス</param>
    private void LoadStage(StageEditor_uchimura stageEditor)
    {
        stageEditor.loadStage = true;
        stageEditor.Data = pre;
        stageEditor.stageName = stageEditor.Data.stageName;
        GameObject o = Instantiate(pre.stage);
        stageEditor.gridPos = new GameObject[pre.gridCells.x, pre.gridCells.y, pre.gridCells.z];
        stageEditor._StageObjects = new GameObject[pre.gridCells.x, pre.gridCells.y, pre.gridCells.z];
        stageEditor.EditStageInit();

    ReStart:
        foreach (Transform child in o.transform)
        {
            child.gameObject.transform.parent = stageEditor.stageRoot.transform;
            Vector3Int v = child.GetComponent<MyCellIndex>().cellIndex;
            stageEditor.gridPos[v.x, v.y, v.z].GetComponent<HighlightObject>().IsAlreadyInstalled = true ;
            stageEditor._StageObjects[v.x, v.y, v.z] = child.gameObject;
        }

        if (o.transform.childCount != 0) { goto ReStart; }
        Destroy(o);
    }
}
#endif
