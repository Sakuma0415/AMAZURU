using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 空の表現用のクラス
/// </summary>
public class SkyFall : MonoBehaviour
{
    [Header("設定項目")]
    //SkyBoxのマテリアル
    [SerializeField]
    Material material;
    //アメフラシの起動数0の時の空の明るさ
    [SerializeField]
    float baseSkyBrightness = 1;
    //アメフラシすべて起動した時の空の明るさ
    [SerializeField]
    float maxSkyBrightness = 0;
    //空の明るさを変更する時間
    [SerializeField]
    float BrightnessTime = 0;

    //private 

    //空の明るさを変更する時間
    float changeTime=0;
    //アメフラシの起動数の変化を検知するための変数
    int amehurashiNumber = 0;
    //明るさ変更中かどうかのフラグ
    bool ChangeFlg = false;
    //変更開始時の空の明るさ
    float startBrightness = 0;
    //変更開始後の空の明るさ
    float endBrightness = 0;

    void Start()
    {
        amehurashiNumber = 0;
        ChangeFlg = false;
        material.SetColor("_Tint", new Color(baseSkyBrightness, baseSkyBrightness, baseSkyBrightness, 1));
    }
    
    void Update()
    {
        if (ChangeFlg)
        {
            changeTime = (changeTime + Time.deltaTime > BrightnessTime) ? BrightnessTime : changeTime + Time.deltaTime;
            float data = Mathf.Lerp(startBrightness, endBrightness, changeTime/ BrightnessTime);
            material.SetColor("_Tint", new Color(data, data, data, 1));
            if(changeTime== BrightnessTime)
            {
                ChangeFlg = false;
            }
        }
        else
        {
            if (amehurashiNumber != AmehurashiManager.amehurashi.amehurashiTrueCont)
            {
                changeTime = 0;
                ChangeFlg = true;
                amehurashiNumber = AmehurashiManager.amehurashi.amehurashiTrueCont;
                startBrightness = material.GetColor("_Tint").r;
                endBrightness = baseSkyBrightness-(((float)AmehurashiManager.amehurashi.amehurashiTrueCont / (float)AmehurashiManager.amehurashi.AmehurashiQuantity) * (baseSkyBrightness - maxSkyBrightness));
            }
        }

    }
}
