using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progress : MonoBehaviour
{
    static public Progress progress;

    public bool key;
    [SerializeField]
    ResultControl resultControl;

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
        resultControl.StartResult( true);
    }
    public void GameOverSet()
    {
        
        resultControl.StartResult(false );
    }
}
