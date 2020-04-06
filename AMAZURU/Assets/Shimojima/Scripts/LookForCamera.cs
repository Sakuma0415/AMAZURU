using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForCamera : MonoBehaviour
{
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private GameObject selectUI;
    private bool lookCamera = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!lookCamera) { return; }
        LookCamera();
        
    }

    private void LookCamera()
    {
        transform.LookAt(camera.transform);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other.gameObject.layer == 9);
        if(other.gameObject.tag == "Player")
        {
            selectUI.SetActive(true);
            lookCamera = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            selectUI.SetActive(false);
            lookCamera = false;
        }
    }
}
