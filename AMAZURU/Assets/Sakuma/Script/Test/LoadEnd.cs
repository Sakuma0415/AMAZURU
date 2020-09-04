using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadEnd : MonoBehaviour
{

    public float Up=0;
    [SerializeField]
    Material material;

    private void Start()
    {
        Up = 1;
    }

    void Update()
    {
        material.SetFloat("_Up", Up);
    }
}
