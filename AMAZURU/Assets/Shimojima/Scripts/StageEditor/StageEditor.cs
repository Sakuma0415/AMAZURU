﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageEditor : MonoBehaviour
{
    [System.Serializable]
    public struct ChangeFrequentlyLiterals
    {
        /// <summary>
        /// 作成したステージのデータ
        /// </summary>
        public PrefabStageData Data { get; set; }

        [Tooltip("ステージ名")]
        public string stageName;
    }

    [Tooltip("頻繁に内容を変更する変数群(不可視変数有)")]
    public ChangeFrequentlyLiterals changeFrequentlyLiterals;

    //[HideInInspector]
    public bool loadStage;

    [Tooltip("グリッドの数　X * Y * Z")]
    public Vector3Int cells;
    private float posAdjust = 0.5f;

    public Vector3Int cellNum;
    private Vector3Int tempCnum = Vector3Int.zero;

    [Tooltip("Gridオブジェクトの参照管理")]
    public GameObject[,,] gridPos;
    private GameObject[,,] _StageObjects;
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
        if(changeFrequentlyLiterals.Data != null && changeFrequentlyLiterals.Data.cfl.stageName != changeFrequentlyLiterals.stageName)
        {
            StageDataIncetance();
        }
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
                if (loadStage) { goto CreatePrefab; }
                _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z].SetActive(true);
                changeFrequentlyLiterals.Data.cfl.stageName = changeFrequentlyLiterals.stageName;
                try
                {
                    AssetDatabase.CreateAsset(changeFrequentlyLiterals.Data, "Assets/Shimojima/StageData_" + changeFrequentlyLiterals.stageName + ".asset");
                }
                catch
                {
                    Debug.Log("同名ファイルが存在するため、上書きされました");
                }

                GameObject[,,] _objects = new GameObject[_StageObjects.GetLength(0), _StageObjects.GetLength(1), _StageObjects.GetLength(2)];
                GameObject _obj = new GameObject();
                _obj.name = "Stage";
                for (int i = 0; i < cells.x; i++)
                {
                    for (int j = 0; j < cells.y; j++)
                    {
                        for (int k = 0; k < cells.z; k++)
                        {
                            if(_StageObjects[i, j, k] == null) { continue; }
                            GameObject obj = Instantiate(_StageObjects[i, j, k]);
                            obj.transform.parent = _obj.transform;
                            _StageObjects[i, j, k] = obj;
                        }
                    }
                }
            
                changeFrequentlyLiterals.Data.stage = (GameObject)PrefabUtility.SaveAsPrefabAssetAndConnect(stageRoot, "Assets/Shimojima/Prefabs/" + changeFrequentlyLiterals.stageName + ".prefab", InteractionMode.UserAction);
                AssetDatabase.SaveAssets();

                Destroy(stageRoot);
                stageRoot = _obj;
                return;

            CreatePrefab:
                changeFrequentlyLiterals.Data.stage = (GameObject)PrefabUtility.SaveAsPrefabAssetAndConnect(stageRoot, "Assets/Shimojima/Prefabs/" + changeFrequentlyLiterals.stageName + ".prefab", InteractionMode.UserAction);
                AssetDatabase.SaveAssets();
            }
            return;
        }
#endif
        SetOrDeleteStageObject();
        
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
            SetStageObject(stageObj);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            DeleteStageObject();
        }
    }

    /// <summary>
    /// 奥行の入力が行われた時の処理
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
        StageDataIncetance();

    CreateGrid:

        stageRoot = new GameObject();
        stageRoot.transform.position = Vector3.zero;
        stageRoot.name = "Stage";
        gridRoot = new GameObject();
        gridRoot.name = "GridRootObj";

        if (!loadStage) { gridPos = new GameObject[cells.x, cells.y, cells.z];
                          _StageObjects = new GameObject[cells.x, cells.y, cells.z];
                        }

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
        //GuidObjectの生成と初期化
        Instantiate(stageObj).AddComponent<GuidObjectInit>().InitGuidObject(guideObj, referenceObject[0], gridPos[cellNum.x,cellNum.y,cellNum.z]);
        guideObj.transform.localPosition = new Vector3(posAdjust, posAdjust, posAdjust);
    }

    /// <summary>
    /// Gridの選択
    /// </summary>
    public void SelectGridObject(Vector3Int cNum, bool isCtrlKeyDown = false)
    {
        if (isCtrlKeyDown) { gridPos[cNum.x, cNum.y, cNum.z].GetComponent<HighlightObject>().IsSelect = true; goto Compleat; }
        if(tempCnum != null) { gridPos[tempCnum.x, tempCnum.y, tempCnum.z].GetComponent<HighlightObject>().IsSelect = false; }

        //GuideObjectの設定
        GameObject hObject = gridPos[cNum.x, cNum.y, cNum.z];
        hObject.GetComponent<HighlightObject>().IsSelect = true;
        if (hObject.GetComponent<HighlightObject>().IsAlreadyInstalled) { guideObj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red; }
        else { guideObj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow; }
        guideObj.transform.position = gridPos[cNum.x, cNum.y, cNum.z].transform.position;
        MakeObjectSkeleton();

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
        Instantiate(stageObj).AddComponent<GuidObjectInit>().InitGuidObject(guideObj, referenceObject[refObjIndex], gridPos[cellNum.x, cellNum.y, cellNum.z]);
    }

    /// <summary>
    /// ステージオブジェクトの設置
    /// </summary>
    /// <param name="cNum">グリッドのセル番号</param>
    /// <param name="obj">設置するゲームオブジェクト</param>
    private void SetStageObject(GameObject obj)
    {
        if (_StageObjects[cellNum.x, cellNum.y, cellNum.z] != null) { Debug.Log("既にオブジェクトが設置されています"); return; }
        GameObject o = Instantiate(obj);
        o.name = obj.name;
        o.transform.localPosition = guideObj.transform.localPosition;
        o.transform.localEulerAngles += objAngle;
        o.transform.parent = stageRoot.transform;
        _StageObjects[cellNum.x, cellNum.y, cellNum.z] = o;
        gridPos[cellNum.x, cellNum.y, cellNum.z].GetComponent<HighlightObject>().IsAlreadyInstalled = true;
        guideObj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
        MakeObjectSkeleton();
    }

    /// <summary>
    /// ステージオブジェクトの削除
    /// </summary>
    private void DeleteStageObject()
    {
        if (_StageObjects[cellNum.x, cellNum.y, cellNum.z] == null) { Debug.Log("削除できるオブジェクトがありません"); return; }
        Debug.Log(_StageObjects[cellNum.x, cellNum.y, cellNum.z].name + "を削除しました");
        Destroy(_StageObjects[cellNum.x, cellNum.y, cellNum.z]);
        gridPos[cellNum.x, cellNum.y, cellNum.z].GetComponent<HighlightObject>().IsAlreadyInstalled = false;
        guideObj.transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
    }

    /// <summary>
    /// <para>ステージオブジェクトを非表示にする</para>
    /// <para>または表示する</para>
    /// </summary>
    private void MakeObjectSkeleton()
    {
        Debug.Log(_StageObjects[cellNum.x, cellNum.y, cellNum.z]);
        if (_tempIndex != cellNum && _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z] != null) 
        { _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z].SetActive(true); }

        if(_StageObjects[cellNum.x, cellNum.y, cellNum.z] != null) 
        {
            _StageObjects[cellNum.x, cellNum.y, cellNum.z].SetActive(false);
            _tempIndex = cellNum;
        }
    }

    private void StageDataIncetance()
    {
        changeFrequentlyLiterals.Data = (PrefabStageData)ScriptableObject.CreateInstance("PrefabStageData");
        changeFrequentlyLiterals.Data.gridCells = cells;
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
        GUILayout.Label("-以下変更可-");
        pre = (PrefabStageData)EditorGUILayout.ObjectField("読み込むステージ", pre, typeof(ScriptableObject), false);
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
        stageEditor.changeFrequentlyLiterals = pre.cfl;
        GameObject o = Instantiate(pre.stage);
        stageEditor.gridPos = new GameObject[pre.gridCells.x, pre.gridCells.y, pre.gridCells.z];
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