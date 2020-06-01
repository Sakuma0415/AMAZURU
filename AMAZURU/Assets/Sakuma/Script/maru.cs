using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maru : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3 (0, Camera.main.transform.eulerAngles.y,0);
    }
}
