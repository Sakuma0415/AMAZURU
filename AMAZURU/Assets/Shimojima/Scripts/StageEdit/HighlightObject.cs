using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    /// <summary>
    /// 選択されているか
    /// </summary>
    public bool IsSelect { get; set; } = false;

    /// <summary>
    /// ステージオブジェクトが設置されているか
    /// </summary>
    public bool IsAlreadyInstalled { get; set; } = false;

    public void OnDrawGizmos()
    {
        if (!IsSelect)
        {
            Gizmos.color = new Color(1,1,1,0.05f);
            Gizmos.DrawWireCube(transform.localPosition, transform.localScale);
        }
        else
        {
            Gizmos.color = Color.green;
            Vector3 scale = transform.localScale;
            Gizmos.DrawCube(transform.localPosition, new Vector3(scale.x / 2, scale.z / 2, scale.y / 2));
        }
    }
}
