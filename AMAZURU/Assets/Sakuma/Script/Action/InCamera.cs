using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InCamera : MonoBehaviour
{
    bool set=false;
    [SerializeField]
    GameObject maskObj;
    [SerializeField]
    Image image;
    float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        maskObj.SetActive(set);
        time = (set ? (time + Time.fixedDeltaTime > 0.1f ? 0.1f : time + Time.fixedDeltaTime) : (time - Time.fixedDeltaTime < 0f ? 0 : time - Time.fixedDeltaTime));
        image.color = new Color(image.color.r, image.color.g, image.color.b, time * 2f+0.1f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if(LayerMask.LayerToName(other.gameObject.layer) == "Mirror")
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
