using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rain : MonoBehaviour
{
    public WaterHi water;
    [SerializeField]
    float rate;
    void Update()
    {
        if(PlayState.playState .gameMode ==PlayState.GameMode.Play)
        {
            water.max += Time.deltaTime/rate;
        }
    }
}
