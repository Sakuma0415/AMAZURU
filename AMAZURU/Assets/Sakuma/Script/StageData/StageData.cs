using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public Vector3 stageSize;
    public Vector3 startPos;
    public float waterHeight;
    public GameObject stagePrefab;
}