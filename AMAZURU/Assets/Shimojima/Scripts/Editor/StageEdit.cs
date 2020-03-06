//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//public class StageEdit : EditorWindow
//{
//    [MenuItem("AMAZURU/StageEdit")]
//    static void ShowWIndow()
//    {
//        EditorWindow.GetWindow<StageEdit>();
//    }

//    PrefabStageData pre;
//    StageEditor stageEditor;
//    string name;

//    private void OnGUI()
//    {
//        stageEditor.changeFrequentlyLiterals.stageName = EditorGUILayout.TextField("ステージ名", name);
//        stageEditor.cells = EditorGUILayout.Vector3IntField("ステージの範囲", stageEditor.cells);
//        pre = (PrefabStageData)EditorGUILayout.ObjectField("読み込むステージ", pre, typeof(ScriptableObject), false);
//        if (GUILayout.Button("NewStage"))
//        {
//            if (!EditorApplication.isPlaying) { return; }
//            stageEditor.CreateGrid();
//        }

//        if (GUILayout.Button("LoadStage"))
//        {
//            if (!EditorApplication.isPlaying) { return; }
//            LoadStage(stageEditor);
//        }
//    }

//    /// <summary>
//    /// ステージの読み込み
//    /// </summary>
//    /// <param name="stageEditor">ベースクラス</param>
//    private void LoadStage(StageEditor stageEditor)
//    {
//        stageEditor.loadStage = true;
//        stageEditor.changeFrequentlyLiterals = pre.cfl;
//        GameObject o = Instantiate(pre.stage);
//        stageEditor.gridPos = new GameObject[pre.gridCells.x, pre.gridCells.y, pre.gridCells.z];
//        stageEditor.CreateGrid();

//    ReStart:
//        foreach (Transform child in o.transform)
//        {
//            child.gameObject.transform.parent = stageEditor.stageRoot.transform;
//        }

//        if (o.transform.childCount != 0) { goto ReStart; }
//        Destroy(o);
//    }
//}
