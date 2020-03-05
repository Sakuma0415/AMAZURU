using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public  float CameraDis;
    public Vector3 lookPos;
    public Rigidbody PlayerTransform;


    private Vector3 lookObj;

    public float XZangle = 0;
    [Range(0,90)]
    [SerializeField]
    private float Yangle = 0;

    float targetAngle;
    float angleSpeed;

    bool lookMode = false;
    float lookAnimeTime = 0;
    // Start is called before the first frame update
    void Start()
    {
        XZangle = 270;
        targetAngle = 270;
    }

    private void LateUpdate()
    {
        transform.position = (new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad), Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * CameraDis) + lookObj;
        transform.localEulerAngles = new Vector3(Yangle, -XZangle - 90, 0);
    }
    // Update is called once per frame
    void Update()
    {
        bool right = Input.GetKey(KeyCode.T);
        bool left = Input.GetKey(KeyCode.Y);

        if (right)
        {
            XZangle -= 2;
        }
        if (left)
        {
            XZangle += 2;
        }

        

        if(Input.GetKeyDown(KeyCode.Q)&& lookAnimeTime==0)
        {
            lookMode = !lookMode;
            lookAnimeTime = 1;
        }

        if(lookAnimeTime > 0)
        {
            lookAnimeTime -= Time.deltaTime;
            if (lookAnimeTime <= 0)
            {
                lookAnimeTime = 0;
            }
        }

        if (lookAnimeTime > 0)
        {
            lookObj = new Vector3(0, 0, 0);
        }
        else
        {
            if (!lookMode)
            {
                lookObj = lookPos;
            }
            else
            {
                lookObj = PlayerTransform.position;
            }
        }
            

    }
}
