﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHi : MonoBehaviour
{
    public float max=0.25f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        transform.localScale = new Vector3(transform.localScale.x, max, transform.localScale.z);
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.U)) { max += Time.deltaTime; }
        if (Input.GetKey(KeyCode.I)) { max -= Time.deltaTime; }

            
        
    }
}