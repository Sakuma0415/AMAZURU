﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageEditor : MonoBehaviour
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

    [HideInInspector]
    public bool loadStage;
    [HideInInspector]
    public bool isSave;
    [HideInInspector]
    public bool isCreateStage;

    [HideInInspector]
    public bool isOnMenu = true;

    [SerializeField]
    private InputField changeObjectName;
    [SerializeField]
    private Dropdown biomeSelect, stageSelect_d;
    [SerializeField]
    private Button nStageB, lStageB;

    [HideInInspector,Tooltip("グリッドの数　X * Y * Z")]
    public Vector3Int cells;
    private float posAdjust = 0.5f;
    [HideInInspector]
    public Vector3Int cellNum;
    private Vector3Int tempCnum = Vector3Int.zero;
    private string biome = "";

    [Tooltip("Gridオブジェクトの参照管理")]
    public GameObject[,,] gridPos;
    public GameObject[,,] _StageObjects;
    private Vector3Int _tempIndex;

    public GameObject menuCanvas;
    public GameObject editCanvas;
    public GameObject enemyEditCanvas;
    public InputField[] stageSizeInputField;
    public InputField stageNameInputField;
    public Toggle isGenerateFloor;
    public Toggle enemyEdit;
    [SerializeField, Tooltip("参照するGridObject")]
    private GameObject gridObj;
    [SerializeField,Tooltip("配置場所を視認し易くするためのオブジェクト")]
    private GameObject guideObj;
    [Tooltip("シーン内のオブジェクトを削除するためのルートオブジェクト")]
    private GameObject gridRoot;

    [HideInInspector, Tooltip("保存するステージのルートオブジェクト")]
    public GameObject stageRoot;
    
    [SerializeField,Tooltip("ステージに使う参照オブジェクト")]
    private GameObject[] referenceObject, floorRefObj, prismRefObj;
    private int refObjIndex = 0;
    [Tooltip("配置するオブジェクト")]
    private GameObject stageObj;

    private Vector3 objAngle;

    private bool IsInputAnyKey { get; set; } = false;
    private float horizontal, vertical = 0;
    private bool deleteComponent = false;

    private void Start()
    {
        //Warningつぶし
        if (!gridObj) { gridObj = new GameObject(); }
        if (!guideObj) { guideObj = new GameObject(); }
        if(referenceObject.Length == 0) { referenceObject = new GameObject[1]; }
        if(floorRefObj.Length == 0) { floorRefObj = new GameObject[1]; }
        if(prismRefObj.Length == 0) { prismRefObj = new GameObject[1]; }
        GetBiomeName();
    }

    void Update()
    {
        if (isCreateStage)
        {
            if (stageSizeInputField[0].interactable)
            {
                for (int i = 0; i < stageSizeInputField.Length; i++)
                {
                    stageSizeInputField[i].interactable = false;
                }

                isGenerateFloor.interactable = false;
            }
        }


        if(Data != null && Data.editName != stageNameInputField.text)
        {
            StageDataIncetance();
        }

        EditorInput();
    }

    /// <summary>
    /// ステージエディット時のキー入力処理
    /// </summary>
    private void EditorInput()
    {
        if (!isCreateStage) { return; }

        if (Input.GetKeyDown(KeyCode.I))
        {
            isOnMenu = !isOnMenu;
            menuCanvas.SetActive(!menuCanvas.activeSelf);
            editCanvas.SetActive(!editCanvas.activeSelf);
        }
        if (isOnMenu) { return; }

        CheakKeyDownForMoveKey();
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ChangeToggleForEnemyEdit();
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            SelectObjectAllChange();
        }

        SetOrDeleteStageObject();

        RangeSelection();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeStageObject();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameObject obj = guideObj.transform.GetChild(1).gameObject;
            Camera.main.transform.parent = null;
            objAngle.y += -90;
            guideObj.transform.localEulerAngles = objAngle;
            obj.transform.parent = null;
            Camera.main.transform.parent = guideObj.transform;
            obj.transform.parent = guideObj.transform;
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
        if (enemyEditCanvas.activeSelf) { return; }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Array3DForLoop(Vector3Int.zero, cells, 2);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Array3DForLoop(Vector3Int.zero, cells, 3);
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
            if (cellNum.y == cells.y) { cellNum.y = 1; }
            SelectGridObject(cellNum);
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            cellNum.y--;
            if (cellNum.y == 0) { cellNum.y = cells.y - 1; }
            SelectGridObject(cellNum);
        }
    }
    #endregion

    #region UI関連

    public void BiomeSelectOnMove()
    {
        biome = biomeSelect.captionText.text;
        GetStageName();
    }

    private void GetBiomeName()
    {
        List<string> bName = new List<string>();
        string[] s = System.IO.Directory.GetDirectories(UnityEngine.Application.dataPath + "/Shimojima/Resources/EditData", "*", System.IO.SearchOption.AllDirectories);
        foreach (string path in s)
        {
            FileInfo f = new FileInfo(path);
            bName.Add(f.Name);
        }
        biomeSelect.AddOptions(bName);
        BiomeSelectOnMove();
    }

    private void GetStageName()
    {
        stageSelect_d.ClearOptions();
        List<string> sName = new List<string>();
        sName.Add("None");
        Object[] o = Resources.LoadAll("EditData/" + biome + "/", typeof(PrefabStageData));
        foreach(Object _o in o)
        {
            sName.Add(_o.name);
        }

        stageSelect_d.AddOptions(sName);
    }

    private void ChangeToggleForEnemyEdit()
    {
        enemyEdit.isOn = !enemyEdit.isOn;
    }

    public void ShowEnemyEditor()
    {
        enemyEditCanvas.SetActive(!enemyEditCanvas.activeSelf);
    }

    public void ChangeName()
    {
        enemyEditCanvas.GetComponent<EnemyDataSet>().sName = stageNameInputField.text;
    }

    public void SetStageName()
    {
        if(stageSelect_d.captionText.text == "None") { stageNameInputField.text = ""; return; }
        stageNameInputField.text = stageSelect_d.captionText.text;
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

        stageObj = referenceObject[0];
        stageRoot = new GameObject();
        stageRoot.name = "Stage";
        enemyEditCanvas.GetComponent<EnemyDataSet>().stageRoot = stageRoot;
        gridRoot = new GameObject();
        gridRoot.name = "GridRootObj";
        cells = new Vector3Int(int.Parse(stageSizeInputField[0].text), int.Parse(stageSizeInputField[1].text) + 1, int.Parse(stageSizeInputField[2].text));
        cellNum = new Vector3Int(0, 1, 0);
        tempCnum = new Vector3Int(0, 1, 0);

        if (!loadStage) { gridPos = new GameObject[cells.x, cells.y, cells.z];
                          _StageObjects = new GameObject[cells.x, cells.y, cells.z];
                        }

        float s = gridObj.transform.localScale.x;
        Array3DForLoop(Vector3Int.zero, cells, 0, s);
        
        gridPos[0, 1, 0].GetComponent<HighlightObject>().IsSelect = true;
        
        //GuidObjectの生成と初期化
        Instantiate(stageObj).AddComponent<GuidObjectInit>().InitGuidObject(guideObj, referenceObject[0], gridPos[cellNum.x,cellNum.y,cellNum.z]);
        guideObj.transform.localPosition = new Vector3(posAdjust, posAdjust, posAdjust);

        if (!isGenerateFloor.isOn) { return; }
        for (int i = 0; i < cells.x; i++)
        {
            for (int j = 0; j < cells.z; j++)
            {
                int x = Random.Range(0, 6);
                GameObject s_obj = Instantiate(floorRefObj[x], gridPos[i,0,j].transform.position, Quaternion.identity);
                s_obj.name = referenceObject[0].name;
                s_obj.AddComponent<MyCellIndex>().cellIndex = new Vector3Int(i, 0, j);
                Destroy(s_obj.GetComponent<BoxCollider>());
                s_obj.transform.parent = stageRoot.transform;
                _StageObjects[i, 0, j] = s_obj;
            }
        }
    }

    /// <summary>
    /// ステージの保存
    /// </summary>
    public void StageSave()
    {
#if UNITY_EDITOR
        if (_StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z] != null)
        {
            _StageObjects[_tempIndex.x, _tempIndex.y, _tempIndex.z].SetActive(true);
        }

        if(stageNameInputField.text == "") { stageNameInputField.text = "stageName"; }
        Data.editName = stageNameInputField.text;
        if (isSave || loadStage)
        {
            AssetDatabase.DeleteAsset("Assets/Shimojima/Resources/EditData/" + biome + "/EditData_" + stageNameInputField.text + ".asset");
            AssetDatabase.SaveAssets();
        }
        Data.stage = (GameObject)PrefabUtility.SaveAsPrefabAssetAndConnect(stageRoot, "Assets/Shimojima/Resources/Prefabs/Stage/" + biome + "/" + stageNameInputField.text + ".prefab", InteractionMode.UserAction);
        AssetDatabase.CreateAsset(Data, "Assets/Shimojima/Resources/EditData/" + biome + "/EditData_" + stageNameInputField.text + ".asset");
        Array3DForLoop(Vector3Int.zero, cells, 1);
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
        guideObj.transform.position = gridPos[cNum.x, cNum.y, cNum.z].transform.position;

        MakeObjectSkeleton();
        tempCnum = cNum;
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
        GameObject o;
        int x = Random.Range(0, 6);
        if (referenceObject[refObjIndex].name == "SandFloor")
        {
            o = Instantiate(floorRefObj[x]);
        }
        else if(referenceObject[refObjIndex].name == "Prism")
        {
            o = Instantiate(prismRefObj[x]);
        }
        else { o = Instantiate(obj); }
        o.name = obj.name;
        o.transform.localPosition = gridPos[cellIndex.x,cellIndex.y,cellIndex.z].transform.localPosition;
        o.transform.localEulerAngles += objAngle;
        o.transform.parent = stageRoot.transform;
        o.AddComponent<MyCellIndex>().cellIndex = cellIndex;
        if (deleteComponent && o.name == "SandFloor") { Destroy(o.GetComponent<BoxCollider>()); }
        
        _StageObjects[cellIndex.x, cellIndex.y, cellIndex.z] = o;
        gridPos[cellIndex.x, cellIndex.y, cellIndex.z].GetComponent<HighlightObject>().IsAlreadyInstalled = true;
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
    }

    /// <summary>
    /// 設置されているオブジェクトをすべて、現在選択しているオブジェクトに差し替える
    /// </summary>
    public void SelectObjectAllChange()
    {
        foreach (GameObject obj in _StageObjects)
        {
            if(obj == null) { continue; }
            if (obj.name == changeObjectName.text)
            {
                GameObject o;
                int x = Random.Range(0, 6);
                if (referenceObject[refObjIndex].name == "SandFloor")
                {
                    o = Instantiate(floorRefObj[x]);
                }
                else if (referenceObject[refObjIndex].name == "prism")
                {
                    o = Instantiate(prismRefObj[x]);
                }
                else { o = Instantiate(referenceObject[refObjIndex]); }
                
                o.name = referenceObject[refObjIndex].name;
                o.transform.localPosition = obj.transform.localPosition;
                o.transform.localEulerAngles += obj.transform.localEulerAngles;
                o.transform.parent = stageRoot.transform;
                o.AddComponent<MyCellIndex>().cellIndex = obj.GetComponent<MyCellIndex>().cellIndex;
                Vector3Int cellIndex = o.GetComponent<MyCellIndex>().cellIndex;
                _StageObjects[cellIndex.x, cellIndex.y, cellIndex.z] = o;
                Destroy(obj);
            }
        }
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
        Data.gridCells = new Vector3Int(cells.x, cells.y - 1, cells.z);
    }

    /// <summary>
    /// 3次元配列の処理
    /// <para>processingIndexに入る値によって処理が変わります</para>
    /// <para>0 = Gridの生成　1 = セーブ処理　2 = 設置処理　3 = 削除処理</para>
    /// </summary>
    /// <param name="tArray">ループ処理の回数</param>
    /// <param name="processingIndex">関数の指定</param>
    /// <param name="size">Grid生成時のグリッドの１辺の長さ</param>
    private void Array3DForLoop(Vector3Int tArray1, Vector3Int tArray2, int processingIndex, float size = 1)
    {
        if(processingIndex < 0 || processingIndex > 4) { Debug.Log("0 ～ 4の間で処理を決定してください"); return; }

        GameObject _obj = new GameObject();
        _obj.name = "Stage";
        if(processingIndex != 1) { Destroy(_obj); }

        for (int i = tArray1.x; i < tArray2.x; i++)
        {
            for (int j = tArray1.y; j < tArray2.y; j++)
            {
                for (int k = tArray1.z; k < tArray2.z; k++)
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
                        case 4:
                            gridPos[i, j, k].GetComponent<HighlightObject>().IsSelect = true;
                            break;
                    }
                }
            }
        }

        if (processingIndex == 2 || processingIndex == 3) { rangeSelectionState = RangeSelectionState.OFF; }
        if (processingIndex != 1) { return; }
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
        obj.transform.localPosition = new Vector3((i + posAdjust) * s, (j + posAdjust - 1) * s, (k + posAdjust) * s);
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
        obj.name = _StageObjects[i, j, k].name;
        obj.transform.parent = _obj.transform;
        _StageObjects[i, j, k] = obj;
    }

    public void SwitchComponent()
    {
        deleteComponent = !deleteComponent;
    }

    /// <summary>
    /// ステージの初期化とデータのセット
    /// </summary>
    public void NewStageCreate()
    {
        EditStageInit();
        isCreateStage = true;
        
        biomeSelect.interactable = false;
        stageSelect_d.interactable = false;
        nStageB.interactable = false;
        lStageB.interactable = false;
    }

    /// <summary>
    /// 既存ステージの読み込み
    /// </summary>
    public void LoadStage()
    {
        if (stageSelect_d.captionText.text == "None") { Debug.Log("⚠ステージが選択されていません！"); return; }
        loadStage = true;
        biomeSelect.interactable = false;
        stageSelect_d.interactable = false;
        nStageB.interactable = false;
        lStageB.interactable = false;

        Data = Resources.Load<PrefabStageData>("EditData/" + biome + "/" + stageNameInputField.text);
        stageNameInputField.text = Data.editName;
        enemyEditCanvas.GetComponent<EnemyDataSet>().sName = Data.editName;
        GameObject o = Instantiate(Data.stage);
        stageSizeInputField[0].text = Data.gridCells.x.ToString();
        stageSizeInputField[1].text = Data.gridCells.y.ToString();
        stageSizeInputField[2].text = Data.gridCells.z.ToString();
        cells = new Vector3Int(Data.gridCells.x, Data.gridCells.y, Data.gridCells.z);
        gridPos = new GameObject[Data.gridCells.x, Data.gridCells.y + 1, Data.gridCells.z];
        _StageObjects = new GameObject[Data.gridCells.x, Data.gridCells.y + 1, Data.gridCells.z];
        EditStageInit();
        isCreateStage = true;

    ReStart:
        foreach (Transform child in o.transform)
        {
            child.gameObject.transform.SetParent(stageRoot.transform);
            Vector3Int v = Vector3Int.zero;
            if (child.GetComponent<MyCellIndex>())
            {
                v = child.GetComponent<MyCellIndex>().cellIndex;
                if(v.y != cells.y && isGenerateFloor.isOn)
                {
                    v = new Vector3Int(v.x, v.y + 1, v.z);
                }
                child.GetComponent<MyCellIndex>().cellIndex = v;

                gridPos[v.x, v.y, v.z].GetComponent<HighlightObject>().IsAlreadyInstalled = true;
                _StageObjects[v.x, v.y, v.z] = child.gameObject;
            }
            else if (child.gameObject.name == "EnemyMaster")
            {
                enemyEditCanvas.GetComponent<EnemyDataSet>().createEnemy = true;
                enemyEditCanvas.GetComponent<EnemyDataSet>().LoadEnemyData(child.gameObject);
            }
        }
        if (o.transform.childCount != 0) { goto ReStart; }
        Destroy(o);
    }
}