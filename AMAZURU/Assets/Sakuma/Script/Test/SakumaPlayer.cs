using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SakumaPlayer : MonoBehaviour
{
    float Yangle = 90;
    [SerializeField]
    float maxSpead = 5;
    float spead=0;
    [SerializeField]
    float maxSpeadTime = 0.5f;

    [SerializeField]
    float slopAngle = 90;
    [SerializeField]
    Vector3 footPos;

    float refAngle;

    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    float rayLeng=0.5f;

    [SerializeField]
    GameObject PlayerObject;

    [SerializeField]
    Animator animator;

    void Start()
    {
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {



        bool F = Input.GetKey(KeyCode.W);
        bool R = Input.GetKey(KeyCode.D);
        bool L = Input.GetKey(KeyCode.A);
        bool B = Input.GetKey(KeyCode.S);

        if (animator != null)
        {
            animator.SetBool("wate", F || R || L || B);
        }




        float cameraangle = -Camera.main.transform.localEulerAngles.y;

        Vector2 fsAngle =new Vector2  (0,0);
        if (F) { fsAngle += new Vector2(0, 1); }
        if (R) { fsAngle += new Vector2(1, 0) ; }
        if (L) { fsAngle += new Vector2(-1, 0) ; }
        if (B) { fsAngle += new Vector2(0, -1) ; }

        //Debug.Log(fsAngle);
        if (F || R || L || B)
        {
            Yangle = Mathf.SmoothDampAngle(Yangle, (Mathf.Atan2(fsAngle.y, fsAngle.x)*Mathf.Rad2Deg + cameraangle), ref refAngle, 0.05f);

            if (maxSpead > spead)
            {
                spead += Time.fixedDeltaTime * maxSpead / maxSpeadTime;
            }
            if (maxSpead < spead)
            {
                spead = maxSpead;
            }
        }
        else
        {
            if (0 < spead)
            {
                spead -= Time.fixedDeltaTime * maxSpead / maxSpeadTime;
            }
            if (0 > spead)
            {
                spead = 0;
            }
        }



        float angleLate = 1;
        float fangle = Yangle;
        Ray ray = new Ray(transform.position+ new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad), 0, Mathf.Sin(fangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime, Vector3.down);
        if(Physics.Raycast(ray, rayLeng, layerMask))
        {
            //
        }
        else
        {
            angleLate = 0;

            for (float i=0; i< 90; i += 10)
            {
                int ok = 0;
                Ray ray2 = new Ray(transform.position + new Vector3(Mathf.Cos((i+ Yangle) * Mathf.Deg2Rad), 0, Mathf.Sin((i + Yangle) * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime, Vector3.down);
                if (Physics.Raycast(ray2, rayLeng, layerMask))
                {

                    fangle = fangle + i;
                    angleLate = Mathf.Cos(i * Mathf.Deg2Rad);
                    ok += 1;
                }
                ray2 = new Ray(transform.position + new Vector3(Mathf.Cos((Yangle - i) * Mathf.Deg2Rad), 0, Mathf.Sin((Yangle - i) * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime, Vector3.down);
                if (Physics.Raycast(ray2, rayLeng, layerMask))
                {
                    fangle = fangle - i;
                    angleLate = Mathf.Cos(i * Mathf.Deg2Rad);
                    ok += 1;
                }
                if (ok > 0)
                {
                    break;
                }
            }

        }

        //Debug.Log(angleLate);
        float Xangle = 0;
        Ray fRay = new Ray(footPos+transform.position, new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad), 0, Mathf.Sin(fangle * Mathf.Deg2Rad)));
        if (Physics.Raycast(fRay, angleLate * spead, layerMask))
        {
            for (float i = 0; i < slopAngle; i += 5)
            {
                Ray fRay2 = new Ray(footPos + transform.position, new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad)));
                if (!Physics.Raycast(fRay2, angleLate * spead*Time.fixedDeltaTime, layerMask))
                {
                    Xangle = i;
                    break;
                }
            }
        }
        else
        {
            for (float i = 0; i > -slopAngle; i -= 5)
            {
                Ray fRay2 = new Ray(footPos + transform.position, new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad)));
                if (Physics.Raycast(fRay2, angleLate * spead * Time.fixedDeltaTime, layerMask))
                {
                    Xangle = i+5;
                    break;
                }
            }
        }


        Ray dray = new Ray(transform.position+new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime * angleLate,Vector3.down);
        if(Physics.Raycast (dray, Mathf.Sin(Xangle * Mathf.Deg2Rad) * spead * Time.fixedDeltaTime * angleLate, layerMask))
        {
            Debug.Log("wewe");
            rb.transform.position += new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), 0, Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime * angleLate;
        }
        else
        {
            rb.transform.position += new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime * angleLate;
        }

        

        //Debug.Log(Xangle);




        PlayerObject.transform.localEulerAngles = new Vector3(0,90- Yangle, 0);

    }




}
