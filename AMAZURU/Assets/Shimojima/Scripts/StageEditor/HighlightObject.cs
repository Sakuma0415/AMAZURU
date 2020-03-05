using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public bool IsSelect { get; set; } = false;

    [SerializeField]
    private Transform myTransform;

    public void OnDrawGizmos()
    {
        if (!IsSelect)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(myTransform.localPosition, myTransform.localScale);
        }
    }
}
