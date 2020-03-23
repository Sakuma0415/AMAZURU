using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sakumaPlayer2 : MonoBehaviour
{

    float Yangle = 90;
    [SerializeField]
    float maxSpead = 5;
    
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
    float rayLeng = 0.5f;

    [SerializeField]
    GameObject PlayerObject;

    [SerializeField]
    Animator animator;



    [SerializeField]
    CapsuleCollider capsuleCollider;
    [SerializeField]
    PhysicMaterial physicMaterial;
    [SerializeField]
    float fangle;
    [SerializeField]
    float angleLate;
    [SerializeField]
    float spead = 0;
    [SerializeField]
    float Xangle = 0;


    [SerializeField]
    float debug;
    void Start()
    {
        footPos.y = (-((capsuleCollider.height) / 2))+0.01f;

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

        Vector2 fsAngle = new Vector2(0, 0);
        if (F) { fsAngle += new Vector2(0, 1); }
        if (R) { fsAngle += new Vector2(1, 0); }
        if (L) { fsAngle += new Vector2(-1, 0); }
        if (B) { fsAngle += new Vector2(0, -1); }

        //Debug.Log(fsAngle);
        if (F || R || L || B)
        {
            //Debug.Log("移動");
            Yangle = Mathf.SmoothDampAngle(Yangle, (Mathf.Atan2(fsAngle.y, fsAngle.x) * Mathf.Rad2Deg + cameraangle), ref refAngle, 0.1f);

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


        //rb.transform.position += new Vector3(Mathf.Cos(Yangle * Mathf.Deg2Rad), 0, Mathf.Sin(Yangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime;

        angleLate = 1;
        fangle = Yangle;
        Ray ray = new Ray(footPos+transform.position + new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad), 0, Mathf.Sin(fangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime, Vector3.down);
        if (Physics.Raycast(ray, rayLeng, layerMask))
        {
            //
        }
        else
        {
            Debug.Log("分解");
            angleLate = 0;

            for (float i = 0; i < 90; i += 10)
            {
                int ok = 0;
                Ray ray2 = new Ray(footPos + transform.position + new Vector3(Mathf.Cos((i + Yangle) * Mathf.Deg2Rad), 0, Mathf.Sin((i + Yangle) * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime, Vector3.down);
                if (Physics.Raycast(ray2, rayLeng, layerMask))
                {

                    fangle = fangle + i;
                    angleLate = Mathf.Cos(i * Mathf.Deg2Rad);
                    ok += 1;
                }
                ray2 = new Ray(footPos + transform.position + new Vector3(Mathf.Cos((Yangle - i) * Mathf.Deg2Rad), 0, Mathf.Sin((Yangle - i) * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime, Vector3.down);
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


#if true
        Xangle = 0;
        Vector3 vec =Vector3.Normalize( new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad), 0, Mathf.Sin(fangle * Mathf.Deg2Rad)));
        //Debug.Log(vec);
        if (
            Physics.CapsuleCast(
            transform.position - new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0),
            transform.position + new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0),
            capsuleCollider.radius,
            vec,
            angleLate * spead * Time.fixedDeltaTime,
            layerMask)
            )
        {
            Debug.Log("ぶつかった");
            for (float i = 0; i < slopAngle; i += 1)
            {
                vec = Vector3.Normalize(new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad)));
                if (!Physics.CapsuleCast(
                    transform.position - new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0),
                    transform.position + new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0),
                    capsuleCollider.radius,
                    vec,
                    angleLate * spead * Time.fixedDeltaTime,
                    layerMask))
                { 
                    Xangle = i;
                    break;
                }

            }

        }
        else
        {
            
            for (float i = 0; i > -slopAngle; i -= 1)
            {
                
                vec = Vector3.Normalize(new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad), Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(i * Mathf.Deg2Rad)));
                if (Physics.CapsuleCast(
                    transform.position - new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0),
                    transform.position + new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0),
                    capsuleCollider.radius,
                    vec,
                    angleLate * spead * Time.fixedDeltaTime,
                    layerMask))
                {
                    Xangle = i + 1;
                    break;
                }
            }
            //Debug.Log(Xangle);
        }
        //Debug.Log(Mathf.Sin(Xangle * Mathf.Deg2Rad));


#endif


        ray = new Ray(footPos + transform.position + new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime * angleLate, Vector3.down);

        if (Physics.Raycast(ray, rayLeng, layerMask))
        {

            Ray dray = new Ray(-new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2 /*+ 0.01f*/, 0)+transform.position + new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime * angleLate, Vector3.down);
            if (Physics.SphereCast(dray,capsuleCollider.radius-0.01f, Mathf.Sin(Xangle * Mathf.Deg2Rad) * spead * Time.fixedDeltaTime * angleLate, layerMask))
            {
                Debug.Log("wewe "+Xangle);

                rb.transform.position += new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad),0, Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime * angleLate;
            }
            else
            {
                rb.transform.position += new Vector3(Mathf.Cos(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * spead * Time.fixedDeltaTime * angleLate;
            }




            





            physicMaterial.staticFriction = 3;
        }
        else
        {
            physicMaterial.staticFriction = 0;
            Debug.Log("せーふてぃ");
        }



        debug = Mathf.Sin(fangle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad) * spead * Time.fixedDeltaTime * angleLate;




    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0,  (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0), capsuleCollider.radius);
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, (capsuleCollider.height - capsuleCollider.radius * 2) / 2, 0), capsuleCollider.radius);
    }
}
