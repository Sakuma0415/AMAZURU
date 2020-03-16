using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable/StageCardData")]
public class StageCardData : ScriptableObject
{
    public string stageName;
    public Sprite sprite;
    public int rank;
}
