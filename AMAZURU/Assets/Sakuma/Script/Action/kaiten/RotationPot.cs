using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationPot : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (set && ControllerInput.Instance.buttonDown.circle && PlayState.playState.gameMode == PlayState.GameMode.Play)
        {
            PlayState .playState . RotationPotStart();

        }
    }



    [SerializeField]
    bool set=false ;

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
            set = false ;
        }
    }






}
