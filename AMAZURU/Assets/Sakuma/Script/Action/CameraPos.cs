using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public  float CameraDis;


    public Vector3 lookPos;

    private float XZangle = 0;
    [Range(0,90)]
    [SerializeField]
    private float Yangle = 0;

    float targetAngle;
    float angleSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void LateUpdate()
    {


        transform.position = (new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad), Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * CameraDis) + lookPos;
        transform.localEulerAngles = new Vector3(Yangle, -XZangle - 90, 0);
    }
    // Update is called once per frame
    void Update()
    {
        bool right = Input.GetKey(KeyCode.T);
        bool left = Input.GetKey(KeyCode.Y);


        if (right)
        {
            targetAngle = XZangle + 15;
        }
        if (left)
        {
            targetAngle = XZangle - 15;
        }

        XZangle = Mathf.SmoothDamp(XZangle, targetAngle, ref angleSpeed, 0.2f);
    }
}
