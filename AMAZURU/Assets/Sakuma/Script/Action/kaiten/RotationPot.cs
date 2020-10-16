using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPot : MonoBehaviour
{
    [SerializeField]
    float goAngle = 0;
    Vector3 lotAngle = Vector3.zero;
    Vector3 WavelotAngle = Vector3.zero;


    public enum DirectionRot
    {
        N=0,
        S,
        E,
        W,
    }

    [SerializeField]
    DirectionRot directionRot;

    // Start is called before the first frame update
    void Start()
    {
        switch ((int)directionRot)
        {
            case 0:
                lotAngle = new Vector3(90, 0, 0);
                WavelotAngle = new Vector3(0, -90, 90);
                return;
            case 1:
                lotAngle = new Vector3(-90, 0, 0);
                WavelotAngle = new Vector3(0, 90, 90);
                return;
            case 2:
                lotAngle = new Vector3(0, 0, -90);
                WavelotAngle = new Vector3(0, 0, 90);
                return;
            case 3:
                lotAngle = new Vector3(0, 0, 90);
                WavelotAngle = new Vector3(0, 180, 90);
                return;
        }
    }

    // Update is called once per frame
    void Update()
    {







        if (set && ControllerInput.Instance.buttonDown.circle && PlayState.playState.gameMode == PlayState.GameMode.Play)
        {
            Debug.Log(goAngle);
            PlayState.playState.RotationPotStart(lotAngle, WavelotAngle, goAngle, true);

        }
    }


    public bool set = false;

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            set = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            set = false;
        }
    }






}
