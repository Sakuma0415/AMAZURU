using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class a : MonoBehaviour
{
    

    [MenuItem("Amazuru/create")]
    public static void CreateRe()
    {
        RenderTexture tex = new RenderTexture(1920, 1080, 16);
        AssetDatabase.CreateAsset(tex, "Assets/Shimojima/RenderTexture.renderTexture");
    }
}
