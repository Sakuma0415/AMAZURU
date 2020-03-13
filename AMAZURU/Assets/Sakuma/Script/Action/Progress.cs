using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress : MonoBehaviour
{
    static public Progress progress;

    public bool key;


    void SetState()
    {
        key = false;
    }
    void Start()
    {
        SetState();
        progress = this;
        SoundManager.soundManager.PlayBgm("rain_loop", 0);
    }
}
