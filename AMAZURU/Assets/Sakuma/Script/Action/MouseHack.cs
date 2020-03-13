using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseHack : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouse_x_delta = Input.GetAxis("Mouse X");
        float mouse_y_delta = Input.GetAxis("Mouse Y");

        Debug.Log(mouse_x_delta+" , "+mouse_x_delta);
    }
}
