using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            if(Progress.progress.key)
            {
                Debug.Log("クリア");
                PlayState.playState.gameMode = PlayState.GameMode.Stop;
            }
            else
            {
                Debug.Log("かぎとってこいや");
            }
        }
    }
}
