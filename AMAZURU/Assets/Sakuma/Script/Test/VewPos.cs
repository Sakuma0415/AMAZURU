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
    [SerializeField]
    float rad = 0.5f;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    CameraPos cameraPos;
    [SerializeField]
    float radLate = 1;

    public struct VewColPos
    {
        public Vector3 endPt1, endPt2;
        public float rad;
        public void Init(float data)
        {
            endPt1 = Vector3.zero;
            endPt2 = Vector3.zero;
            rad = data;
        }
    }

    static public VewColPos vewcolPos;
    
    private void Awake()
    {
        vewcolPos.Init(rad);
    }
    
    void Update()
    {
        //Debug.Log( RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position));
        Vector2  centerPos =( RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position) + RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position + new Vector3(0, headPos, 0)))/ 2;
        float r =( Vector2.Distance(RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position), RectTransformUtility.WorldToScreenPoint(Camera.main, PlayerTransform.position + new Vector3(0, headPos, 0)))/1f)+100;
        Vector4 Ass = new Vector4(centerPos.x/1920, centerPos.y/1080,0,0);
        material.SetVector("_Point", Ass);
        float rr = r / 1080;

        float dist = Vector3.Distance(Camera.main.gameObject.transform.position, PlayerTransform.position);
        //material.SetFloat("_R",rr<0.05f?0.05f:rr);
        material.SetFloat("_R",radLate/ dist );
        Vector3 Pt1 = PlayerTransform.position + new Vector3(0, headPos/2, 0);
        Vector3 Pt2 = Camera.main.transform.position;
        float dis = Vector3.Distance(Pt1, Pt2);
        if (dis < rad * 2)
        {
            //capsuleが成り立たない場合の処理
        }
        else
        {
            float hi = rad / dis;
            Vector3 loPt1 = Pt2 - Pt1;
            vewcolPos.endPt1 = Pt1 + (loPt1 * hi);

            Vector3 loPt2 = Pt1 - Pt2;
            vewcolPos.endPt2 = Pt2 + (loPt2 * hi);



        }

    }
}
