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
        name = refObj.name;
        transform.parent = parent.transform;
        transform.localPosition = refObj.transform.localPosition;
        transform.localRotation = refObj.transform.localRotation;
    }
}
