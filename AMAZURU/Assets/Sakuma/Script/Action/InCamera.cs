using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InCamera : MonoBehaviour
{
    [SerializeField]
    bool set=false;


    [SerializeField]
    GameObject maskObj;
    [SerializeField]
    Image image;
    float time = 0;
    [SerializeField]
    LayerMask inMask;
    [SerializeField]
    LayerMask outMask;
    [SerializeField]
    Camera camera;



    // Start is called before the first frame update
    void Start()
    {
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        camera.cullingMask = !set ? outMask : inMask;
        maskObj.SetActive(set);
        time = (set ? 0.1f : (time - Time.fixedDeltaTime < 0f ? 0 : time - Time.fixedDeltaTime));
        image.color = new Color(image.color.r, image.color.g, image.color.b, (time==0)?0:time *2+0.3f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Mirror")
        {
            set = true;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Mirror")
        {
            set = false;
        }

    }

}
