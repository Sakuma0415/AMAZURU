using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDep : MonoBehaviour
{
    [SerializeField]
    bool stageSet = false;
    [SerializeField]
    Camera camera;
    [SerializeField]
    CameraPos cameraPos;
    [SerializeField]
    SphereCollider sphereCollider;
    [SerializeField]
    LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.OverlapSphere(transform.position, sphereCollider.radius, layerMask).Length == 0)
        {
            camera.nearClipPlane = 0.01f;
        }
        else
        {
            Vector3 playerVec = cameraPos.PlayerTransform.position - transform.position;
            float dis = Vector3.Distance(cameraPos.PlayerTransform.position, transform.position);
            int step = 10;
            Vector3 stepPos = Vector3.zero;
            for (int i = 0; i < step - 1; i++)
            {
                stepPos = Vector3.Lerp(transform.position, cameraPos.PlayerTransform.position, (float)i / (float)step);
                if (Physics.OverlapSphere(stepPos, 0).Length == 0)
                {
                    break;
                }
            }

            camera.nearClipPlane = Vector3.Distance(stepPos, transform.position);


            stageSet = true;
        }

    }
    //private void OnTriggerStay(Collider other)
    //{
    //    if (LayerMask.LayerToName(other.gameObject.layer) == "Default")
    //    {
    //        Vector3 playerVec = cameraPos.PlayerTransform.position - transform.position;
    //        float dis = Vector3.Distance(cameraPos.PlayerTransform.position, transform.position);
    //        int step = 10;
    //        Vector3 stepPos=Vector3.zero;
    //        for(int i = 0; i < step-1; i++)
    //        {
    //            stepPos = Vector3.Lerp(transform.position, cameraPos.PlayerTransform.position,(float) i / (float)step);
    //            if (Physics.OverlapSphere(stepPos, 0).Length ==0)
    //            {
    //                break;
    //            }
    //        }

    //        camera.nearClipPlane = Vector3.Distance (stepPos,transform.position);


    //        stageSet = true;
    //    }
    //}
}
