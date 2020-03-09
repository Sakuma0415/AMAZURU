using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : MonoBehaviour
{
    static public PlayState playState;
    public enum GameMode
    {
        Play,
        Menu,
        Anime,
        Stop
    }
    public GameMode gameMode;
    // Start is called before the first frame update
    void Start()
    {
        playState = new PlayState();
        playState.gameMode = GameMode.Play;
    }


}
