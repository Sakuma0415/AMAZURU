using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : MonoBehaviour
{
    static public PlayState playState;
    public bool copyFlg = false; 
    public enum GameMode
    {
        StartEf,
        Play,
        Menu,
        Anime,
        Stop,
        Rain,
        RainSelect,
        Clear
    }
    GameMode backGameMode;
    public GameMode gameMode;

    public float rainTime = 0;








    // Start is called before the first frame update
    void Start()
    {

        playState = new PlayState();
        playState.gameMode = GameMode.StartEf;

        if (!copyFlg)
        {
            DontDestroyOnLoad(gameObject);
            playState.copyFlg = true;
        }
        else
        {
            Destroy(gameObject);
        }

    }




    private void Update()
    {
        Debug.Log(playState.gameMode);
        if(playState.backGameMode != playState.gameMode)
        {
            switch (playState.gameMode)
            {
                case GameMode.Clear:
                    Progress.progress.ResultSet();
                    break;
                case GameMode.Rain:
                    //playState.rainTime = 2;
                    break;
            }

        }
        playState.backGameMode = playState.gameMode;

        switch (playState.gameMode)
        {
            case GameMode.Rain:
                playState.RainUpDate();
                break;
        }





    }


    void RainUpDate()
    {
        //Debug.Log(playState.rainTime);
        playState.rainTime -= Time.deltaTime;
        if (playState.rainTime <= 0)
        {
            playState.gameMode = GameMode.Play;
        }
    }

}
