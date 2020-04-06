using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public Vector3 stageSize;
    public Vector3 startPos;
    public float waterStep;
    public GameObject stagePrefab;
    public float startAngle;//未設定
    public int AmehurashiQuantity;
}