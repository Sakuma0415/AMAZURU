using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMake : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public GameObject testPlane;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
        {
            //Instantiate(gameObject );
            testPlane.GetComponent<MeshRenderer>().material = gameObject.GetComponent<MeshRenderer>().material;
        }
    }
}
