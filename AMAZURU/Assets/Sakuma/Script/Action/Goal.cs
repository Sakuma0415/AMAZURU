using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            if(AmehurashiManager.amehurashi.amehurashiTrueCont == AmehurashiManager.amehurashi.AmehurashiQuantity)
            {
                Debug.Log("クリア");
                PlayState.playState.gameMode = PlayState.GameMode.Clear ;
            }
            else
            {
                Debug.Log("かぎとってこいや");
            }
        }
    }
}
