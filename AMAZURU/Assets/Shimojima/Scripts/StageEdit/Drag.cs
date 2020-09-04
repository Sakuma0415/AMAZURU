using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IDragHandler
{
    
    void Start()
    {
    }

    void Update()
    {
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 localPos = Vector3.zero;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), eventData.position, Camera.main, out localPos);
        transform.position = localPos;
    }
}
