using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputAction : MonoBehaviour
{
    [SerializeField]
    private Transform target;
    [Range(3, 10)]
    public float rotateSpeed;

    [Range(20, 50)]
    public float moveSpeed;

    private bool mouseLeftButtonDown = false;

    //private float dis;

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            mouseLeftButtonDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            mouseLeftButtonDown = false;
        }

        PitchChange();
    }

    private void FixedUpdate()
    {
        if (!mouseLeftButtonDown) { return; }
        RotateCamera();
    }

    private void RotateCamera()
    {
        Vector3 angle = new Vector3(Input.GetAxis("Mouse X") * rotateSpeed, Input.GetAxis("Mouse Y") * rotateSpeed, 0);

        transform.RotateAround(target.position, Vector3.up, angle.x);
        transform.RotateAround(target.position, transform.right, -angle.y);
    }

    private void PitchChange()
    {
        float dis = Vector3.Distance(target.position, transform.position);

        if (dis <= 10f) { moveSpeed = 5f; }
        else if(dis > 10) { moveSpeed = 20; }

        float moveLength = moveSpeed * Input.GetAxis("Mouse ScrollWheel");
        
        if (dis <= 5f && moveLength > 0) { return; }
        else if (dis >= 30f && moveLength < 0) { return; }

        transform.position += transform.forward * moveLength;
    }
}
