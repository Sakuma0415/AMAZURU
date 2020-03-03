﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable/StageData")]
public class StageData : ScriptableObject
{
    public string stageName;
    public float stageLength;
    public float stageWidth;
    public float waterHeight;
    public GameObject stagePrefab;
}