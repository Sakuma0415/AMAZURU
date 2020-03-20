using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainPot : MonoBehaviour
{

    [SerializeField]
    WaterHi waterHi;
    [SerializeField]
    LayerMask layerMask;
    // Start is called before the first frame update
    void Start()
    {
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
        if(PlayState.playState.gameMode == PlayState.GameMode.RainSelect)
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                waterHi.HiChange(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                waterHi.HiChange(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                waterHi.HiChange(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                waterHi.HiChange(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                waterHi.HiChange(5);
            }




        }



    }

    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("other");
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Space) && PlayState.playState.gameMode == PlayState.GameMode.Play)
            {
                PlayState.playState.gameMode = PlayState.GameMode.RainSelect;
            }
        }
    }



}
