using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHi : MonoBehaviour
{
    public float max;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (max >= transform.localScale.y)
        {
            transform.localScale += new Vector3(0, 0.01f, 0);
        }
    }
}
