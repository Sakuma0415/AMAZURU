using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "PRefabStageData", fileName = "s")]
public class PrefabStageData : ScriptableObject
{
    [Tooltip("頻繁に変更する変数群")]
    public StageEditor.ChangeFrequentlyLiterals cfl;
    [Tooltip("ステージプレファブ")]
    public GameObject stage;
    [Tooltip("生成するグリッドの数")]
    public Vector3Int gridCells;
}
