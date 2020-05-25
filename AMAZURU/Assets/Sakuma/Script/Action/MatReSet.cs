using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// ステージ選択の時にMaterialをリセットするためのクラス
/// </summary>
public class MatReSet : MonoBehaviour
{

    //初期化するマテリアル
    [SerializeField]
    Material[] materials;


    //起動時処理
    void Start()
    {
        for(int i=0;i< materials.Length; i++)
        {
            materials[i].SetFloat("_High", 0);
            materials[i].SetFloat("_Xside", 0);
            materials[i].SetFloat("_Zside", 0);
        }
    }

}
