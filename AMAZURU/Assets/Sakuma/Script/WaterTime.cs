using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTime : MonoBehaviour
{
    [SerializeField]
    Material[] material;
    float time = 0;
    [SerializeField]
    float spead = 0.1f;
    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        for(int i=0; i < material.Length; i++)
        {
            material[i].SetFloat("BackTime", time * 0.1f);

        }
    }
}
