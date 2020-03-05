using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(menuName = "PRefabStageData", fileName = "s")]
public class PrefabStageData : ScriptableObject
{
    public string stageName;
    public GameObject stage;
    public Vector3Int gridCells;
}
