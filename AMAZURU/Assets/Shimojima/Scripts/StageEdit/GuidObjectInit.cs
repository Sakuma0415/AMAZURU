using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuidObjectInit : MonoBehaviour
{
    /// <summary>
    /// GuidObjectの初期化
    /// </summary>
    /// <param name="parent">親オブジェクト</param>
    /// <param name="refObj">インスタンス元のプレファブ</param>
    public void InitGuidObject(GameObject parent, GameObject refObj, GameObject gridObj = null)
    {
        transform.parent = parent.transform;
        if(gridObj.GetComponent<HighlightObject>().IsAlreadyInstalled) 
        {
            GetComponent<Renderer>().material.color = Color.red; 
            goto PosSetting; 
        }

    PosSetting:
        transform.localPosition = refObj.transform.localPosition;
        transform.localRotation = refObj.transform.localRotation;
    }
}
