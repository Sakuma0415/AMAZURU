using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "PRefabStageData", fileName = "s")]
public class PrefabStageData : ScriptableObject
{
    public string stageName;
    [Tooltip("ステージプレファブ")]
    public GameObject stage;
    [Tooltip("ステージセレクト用プレファブ")]
    public GameObject viewStage;
    [Tooltip("生成するグリッドの数")]
    public Vector3Int gridCells;
}
