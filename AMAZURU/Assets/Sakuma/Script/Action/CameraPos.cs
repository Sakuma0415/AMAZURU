using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPos : MonoBehaviour
{
    public float CameraDisS;
    public float CameraDisP;

    [SerializeField]
    float CameraDis;

    public Vector3 lookPos;
    public Transform PlayerTransform;


    private Vector3 lookObj;

    public float XZangle = 0;
    [Range(0,90)]
    [SerializeField]
    private float Yangle = 0;

    float targetAngle;
    float angleSpeed;
    [SerializeField]
    bool lookMode = false;
    float lookAnimeTime = 0;

    Vector3 animePos;
    Vector3 animeRef;
    [SerializeField]
    bool MouseHack = true;


    [SerializeField]
    [Range(-10, 10)]
    float lookHi;
    float newLookHi;

    [SerializeField]
    float changeTime;

    [SerializeField]
    public float fAngle;

    bool MouseCheck = true;
    [SerializeField]
    float endCameraPos;
    [SerializeField]
    LayerMask layerMask;
    [SerializeField]
    SphereCollider sphereCollider;

    bool startCameraFlg = true;
    bool startCameraAngleResetFlg = true;
    float startCameraAngleResetBf = 0;
    // Start is called before the first frame update
    void Start()
    {
        startCameraFlg = true;
        
        XZangle = fAngle;
        targetAngle = fAngle;
        CameraDis =lookMode ? CameraDisP: CameraDisS;
        MouseCheck = true;
    }

    private void LateUpdate()
    {
        if ((PlayState.playState.gameMode == PlayState.GameMode.Play|| PlayState.playState.gameMode == PlayState.GameMode.StartEf ) || rainPotChange)
        {
            if (lookAnimeTime > 0)
            {

                transform.position = Vector3.Lerp(animePos, (new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad) + newLookHi, Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * (!lookMode ? CameraDisS : CameraDisP)) + lookObj, 1 - (lookAnimeTime/ changeTime));
                
            }
            else
            {
                if (lookMode)
                {

                    if (Physics.OverlapSphere((new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad) + newLookHi, Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * CameraDis) + lookObj, sphereCollider.radius, layerMask).Length == 0)
                    {
                        endCameraPos = CameraDis;
                    }
                    else
                    {
                        Ray ray = new Ray(PlayerTransform.position, Vector3.Normalize(transform.position - PlayerTransform.position));
                        RaycastHit hit;
                        if (Physics.SphereCast (ray, sphereCollider.radius, out hit, CameraDis, layerMask))
                        {
                            Debug.Log(hit.collider.gameObject.name);
                            endCameraPos = Vector3.Distance(hit.point, PlayerTransform.position);
                        }
                        else
                        {
                            endCameraPos = CameraDis;
                        }
                    }

                }
                else
                {
                    endCameraPos = CameraDis;
                }

                transform.position = (new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad) + newLookHi, Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * endCameraPos) + lookObj;
            }

            transform.localEulerAngles = new Vector3(Yangle, -XZangle - 90, 0);
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (startCameraFlg)
        {
            lookObj = lookMode ? PlayerTransform.position : lookPos;
            XZangle += Time.deltaTime*3;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                startCameraAngleResetBf = XZangle;
                startCameraFlg = false;
                PlayState.playState.gameMode = PlayState.GameMode.Play;
                lookMode = !lookMode;
                lookAnimeTime = changeTime;
                animePos = transform.position;
            }
        }

        //Debug.Log(lookObj); 

        if (PlayState.playState.gameMode == PlayState.GameMode.Play)
        {
            Cursor.visible = !MouseHack;
            if (MouseHack)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                MouseHack = !MouseHack;
            }
            if (MouseHack)
            {

                if (MouseCheck)
                {
                        float mouse_x_delta = Input.GetAxis("Mouse X");
                        float mouse_y_delta = Input.GetAxis("Mouse Y");

                        XZangle -= mouse_x_delta * (lookMode ? CameraDisS : CameraDisP) / 10;
                        Yangle -= mouse_y_delta * (lookMode ? CameraDisS : CameraDisP) / 10;

                        if (Yangle > 90)
                        {
                            Yangle = 90;
                        }
                        if (Yangle < -90)
                        {
                            Yangle = -90;
                        }


                }


            }
            //else
            //{
            //    bool right = Input.GetKey(KeyCode.T);
            //    bool left = Input.GetKey(KeyCode.Y);
            //    if (right)
            //    {
            //        XZangle -= 2;
            //    }
            //    if (left)
            //    {
            //        XZangle += 2;
            //    }
            //}






            if (Input.GetKeyDown(KeyCode.Q) && lookAnimeTime == 0&&!startCameraFlg )
            {
                lookMode = !lookMode;
                lookAnimeTime = changeTime;
                animePos = transform.position;
            }

            if (lookAnimeTime > 0)
            {
                MouseCheck = false;
                lookAnimeTime -= Time.deltaTime;
                if (lookAnimeTime <= 0)
                {
                    lookAnimeTime = 0;
                    MouseCheck = true;
                }
                CameraDis = Mathf.Lerp(lookMode ? CameraDisS : CameraDisP, lookMode ? CameraDisP : CameraDisS, 1 - (lookAnimeTime/ changeTime));
                newLookHi = Mathf.Lerp(lookMode ? 0 : lookHi, lookMode ? lookHi  : 0, 1 - (lookAnimeTime / changeTime));
                if (startCameraAngleResetFlg)
                {
                    XZangle = Mathf.Lerp(startCameraAngleResetBf  , fAngle , 1 - (lookAnimeTime / changeTime));
                }
            }
            else
            {
                if(startCameraAngleResetFlg)
                {
                    startCameraAngleResetFlg = false;
                }
            }

            lookObj = lookMode ? PlayerTransform.position : lookPos;

        }
        else
        {
            Cursor.visible = true ;

                Cursor.lockState = CursorLockMode.None;
            
        }

        if (rainPotChange)
        {
            potAnimeTime += Time.deltaTime;

            if (outflg)
            {
                if (potAnimeTime < rainPotChangeAnimeTimeSpead)
                {
                    XZangle = Mathf.LerpAngle(lotAngle, beforeAngleXZ, potAnimeTime/ rainPotChangeAnimeTimeSpead);
                    Yangle = Mathf.LerpAngle(35, beforeAngleY, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    CameraDis = Mathf.Lerp(25, beforeDis, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookObj = Vector3.Lerp(lookPos, beforePos , potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookHi = Mathf.Lerp( -0.1f, beforeHi, potAnimeTime / rainPotChangeAnimeTimeSpead);
                }
                else
                {
                    XZangle = Mathf.LerpAngle(lotAngle, beforeAngleXZ, 1);
                    Yangle = Mathf.LerpAngle(35, beforeAngleY, 1);
                    CameraDis = Mathf.Lerp(25, beforeDis, 1);
                    lookObj = Vector3.Lerp(lookPos, beforePos, 1);
                    lookHi = Mathf.Lerp(-0.1f, beforeHi, 1);
                    rainPotChange = false;
                }

            }
            else
            {
                if (potAnimeTime < rainPotChangeAnimeTimeSpead)
                {
                    XZangle = Mathf.LerpAngle(beforeAngleXZ, lotAngle, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    Yangle = Mathf.LerpAngle(beforeAngleY, 35, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    CameraDis = Mathf.Lerp(beforeDis, 25, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookObj = Vector3.Lerp(beforePos , lookPos, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookHi = Mathf.Lerp(beforeHi, -0.1f, potAnimeTime / rainPotChangeAnimeTimeSpead);
                }
                else
                {
                    XZangle = Mathf.LerpAngle(beforeAngleXZ, lotAngle, 1);
                    Yangle = Mathf.LerpAngle(beforeAngleY, 35, 1);
                    CameraDis = Mathf.Lerp(beforeDis, 25, 1);
                    lookObj = Vector3.Lerp(beforePos, lookPos, 1);
                    lookHi = Mathf.Lerp(beforeHi, -0.1f, 1);
                }
            }

        }
    }


    //雨降らし選択時のカメラ遷移用
    [SerializeField]
    bool rainPotChange = false;
    float beforeAngleY = 0;
    float beforeAngleXZ = 0;
    float beforeDis = 0;
    float beforeHi = 0;
    Vector3 beforePos;
    float rainPotChangeAnimeTime;
    public float rainPotChangeAnimeTimeSpead;
    float potAnimeTime;
    bool outflg = false;
    float lotAngle = 0;

    public void RainPotChange()
    {

        beforeHi = lookHi;
        rainPotChange = true;
        outflg = false;
        rainPotChangeAnimeTime = 0;
        beforePos = lookObj;
        beforeAngleXZ = XZangle;
        beforeAngleY = Yangle;
        beforeDis = lookMode ? CameraDisP : CameraDisS;
        potAnimeTime = 0;

        float bfAngle = XZangle;
        while (bfAngle > 360)
        {
            bfAngle -= 360;
        }
        while (bfAngle < 0)
        {
            bfAngle += 360;
        }
        if(bfAngle < 90)
        {
            lotAngle = 45;
        }
        else if(bfAngle < 180)
        {
            lotAngle = 135;
        }
        else if (bfAngle < 270)
        {
            lotAngle = 225;
        }
        else
        {
            lotAngle = 315;
        }
    }
    public void RainPotChangeOut()
    {
        potAnimeTime = 0;
        outflg = true;
    }

}
