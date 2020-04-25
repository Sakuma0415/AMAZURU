using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 水中でカメラにフィルターをかけるためのクラス
/// </summary>
public class InCamera : MonoBehaviour
{

    [Header("設定項目")]

    //水中かどうかのフラグ
    public bool set=false;
    
    [Header("変更不要")]

    //水中でかけるフィルターのオブジェ
    [SerializeField]
    GameObject maskObj;
    

    private void FixedUpdate()
    {
        //マスク設定
        maskObj.SetActive(set);
    }

    //水と接触時
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Mirror")
        {
            set = true;
        }
    }

    //水との接触が離れた時
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Mirror")
        {
            set = false;
        }
    }
}
