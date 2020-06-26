using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Scriptable/Config")]
/// <summary>
/// ゲーム内の設定の保存先
/// </summary>
public class Config : ScriptableObject
{
    public CameraSpeed cameraSpeed;
    public int stageSrectStartTarget;
}
