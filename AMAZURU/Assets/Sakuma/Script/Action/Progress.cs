using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress : MonoBehaviour
{
    static public Progress progress;

    public bool key;
    [SerializeField]
    ResultAnimation resultAnimation;

    void SetState()
    {
        key = false;
    }
    void Start()
    {
        SetState();
        progress = this;
        //SoundManager.soundManager.PlayBgm("rain_loop", 0);
    }

    public void ResultSet()
    {
        resultAnimation.AnimationFlag = true;
    }
}
