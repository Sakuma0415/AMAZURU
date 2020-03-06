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
    public void InitGuidObject(GameObject parent, GameObject refObj)
    {
        transform.parent = parent.transform;
        GetComponent<Renderer>().material.color = Color.yellow;
        transform.localPosition = Vector3.zero;
        transform.localRotation = refObj.transform.localRotation;
    }
}
