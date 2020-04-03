using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainPot : MonoBehaviour
{

    [SerializeField]
    WaterHi waterHi;
    [SerializeField]
    LayerMask layerMask;


    public CameraPos cameraPos;
    public bool sw=false;



    // Start is called before the first frame update
    void Start()
    {
        cameraPos = Camera.main.gameObject.GetComponent<CameraPos>();
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, layerMask))
        {
            waterHi = hit.collider.gameObject.GetComponent<WaterHi>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //try
        //{
        //    if (PlayState.playState.gameMode == PlayState.GameMode.RainSelect)
        //    {

        //        if (Input.GetKeyDown(KeyCode.Alpha1))
        //        {
        //            waterHi.HiChange(1);
        //        }
        //        if (Input.GetKeyDown(KeyCode.Alpha2))
        //        {
        //            waterHi.HiChange(2);
        //        }
        //        if (Input.GetKeyDown(KeyCode.Alpha3))
        //        {
        //            waterHi.HiChange(3);
        //        }
        //        if (Input.GetKeyDown(KeyCode.Alpha4))
        //        {
        //            waterHi.HiChange(4);
        //        }
        //        if (Input.GetKeyDown(KeyCode.Alpha5))
        //        {
        //            waterHi.HiChange(5);
        //        }




        //    }
        //}
        //catch
        //{

        //}




    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Space) && PlayState.playState.gameMode == PlayState.GameMode.Play)
            {
                PlayState.playState.gameMode = PlayState.GameMode.RainSelect;
                Camera.main.gameObject.GetComponent<CameraPos>().RainPotChange();
                AmehurashiManager.amehurashi.rainPot = this;
                AmehurashiManager.amehurashi.hi = transform.position.y - 0.5f;
            }
        }
    }



}
