using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayState : MonoBehaviour
{
    static public PlayState playState;
    static public bool copyFlg = false; 
    public enum GameMode
    {
        StartEf,
        Play,
        Menu,
        Anime,
        Stop,
        Rain,
        RainSelect,
        Clear,
        GameOver
    }
    GameMode backGameMode;
    public GameMode gameMode;

    public float rainTime = 0;








    // Start is called before the first frame update
    void Start()
    {
        playState = new PlayState();
        playState.gameMode = gameMode;


        if (!PlayState.copyFlg)
        {
            DontDestroyOnLoad(gameObject);
            PlayState.copyFlg = true;
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
                case GameMode.GameOver:
                    Progress.progress.GameOverSet();
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
