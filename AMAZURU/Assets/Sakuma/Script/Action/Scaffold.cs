using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scaffold : MonoBehaviour
{
    [SerializeField]
    WaterHi waterHi;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    Rigidbody rb;
    
    // Start is called before the first frame update
    void Start()
    {
        Ray ray = new Ray(transform.position ,transform.TransformDirection(Vector3.down));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,200, layerMask))
        {
            waterHi=hit.collider.gameObject.GetComponent<WaterHi>();
        }
        
    }

    private void FixedUpdate()
    {
        if (waterHi != null)
        {
            rb.transform.position = new Vector3(transform.position.x, waterHi.max, transform.position.z);
        }
    }

}
