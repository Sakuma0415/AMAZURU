using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "PRefabStageData", fileName = "s")]
public class PrefabStageData : ScriptableObject
{
    public string stageName;
    public string editName;
    [Tooltip("難易度")]
    public int diificulty;
    [Tooltip("アメフラシの数")]
    public int amehurashiNum;
    [Tooltip("水位が上がる量")]
    public int increasedWaterVolume;
    [Tooltip("クリア率")]
    public int clearPercentage;
    [Tooltip("ステージプレファブ")]
    public GameObject stage;
    [Tooltip("ステージセレクト用プレファブ")]
    public GameObject viewStage;
    [Tooltip("ステージデータ")]
    public StageData sData;
    [Tooltip("生成するグリッドの数")]
    public Vector3Int gridCells;
    [Tooltip("ステージセレクトで生成されるときのデフォルトポジション")]
    public Vector3 defPos;
}
