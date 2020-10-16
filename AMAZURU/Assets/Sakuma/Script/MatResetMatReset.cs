using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatResetMatReset : MonoBehaviour
{
    //SkyBoxのマテリアル
    [SerializeField]
    Material material;
    //アメフラシの起動数0の時の空の明るさ
    [SerializeField]
    float baseSkyBrightness = 1;
    void Start()
    {
        material.SetColor("_Tint", new Color(baseSkyBrightness, baseSkyBrightness, baseSkyBrightness, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
