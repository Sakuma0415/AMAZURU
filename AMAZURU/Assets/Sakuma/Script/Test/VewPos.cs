using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VewPos : MonoBehaviour
{
    public Transform PlayerTransform;
    [SerializeField]
    float headPos = 0;
    [SerializeField]
    Material material;
    void Start()
    {
        
    }
    
    void Update()
    {
        Debug.Log( RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position));
        Vector2  centerPos =( RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position) + RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position + new Vector3(0, headPos, 0)))/ 2;
        float r = Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position), RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position + new Vector3(0, headPos, 0)))+0.2f;
        Vector4 Ass = new Vector4(centerPos.x/1920, centerPos.y/1080,0,0);
        material.SetVector("_Point", Ass);
        material.SetFloat("_R", r/1080);
    }



}
