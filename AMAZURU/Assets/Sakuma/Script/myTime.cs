using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class myTime : MonoBehaviour
{
    Material material;
    void Start()
    {
        material=GetComponent<MeshRenderer>().material ;
    }

    void Update()
    {
        material.SetFloat("BackTime", WaterTime.time  * 0.1f);
    }
}
