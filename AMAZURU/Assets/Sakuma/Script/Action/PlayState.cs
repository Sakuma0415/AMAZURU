using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクションシーン中のプレイの状態を管理するクラス
/// </summary>
public class PlayState : MonoBehaviour
{

    //Instance
    static public PlayState playState;
    //アクションシーンの再再生時にオブジェを複製するかどうかのフラグ
    static public bool copyFlg = false; 
    //ゲームの状態
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
        GameOver,
        Pause,
        Thunder,
    }
    //ゲームモードの変化を検知するシーケンサー用の変数
    GameMode backGameMode;
    //現在のゲームモード
    public GameMode gameMode;
    //アメフラシ起動演出の状態からプレイモードに遷移するまでの時間
    public float rainTime = 0;

    public bool Tutorial = false;

    [SerializeField]
    TutorialUI tutorialUI;
    [SerializeField ]
    public  float ThunderTime=0;
    //
    bool IsThunder = false;
    [SerializeField]
    CharacterMaster ChaMs;

    // 初期化
    void Start()
    {
        playState = new PlayState();
        playState.gameMode =gameMode;
        playState.ChaMs = ChaMs;
        playState.Tutorial = StageMake.LoadStageData.TutorialFlg;
        if (playState.Tutorial) { tutorialUI.TutorialStart(); }
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

        //ゲームモードが変更されたときに呼び出す処理
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
                    break;
                case GameMode.Thunder:
                    playState.ThunderTime = 5;
                    break;
            }
        }
        playState.backGameMode = playState.gameMode;

        //ゲームモードごとに毎フレーム呼び出す処理
        switch (playState.gameMode)
        {
            case GameMode.Rain:
                playState.RainUpDate();
                break;
            case GameMode.Thunder:
                playState.ThunderUpDate();
                break;
        }
        //Debug.Log(playState.gameMode);
    }

    //アメフラシ演出中の処理
    void RainUpDate()
    {
        playState.rainTime -= Time.deltaTime;
        if (playState.rainTime <= 0)
        {
            if ((AmehurashiManager.amehurashi.amehurashiTrueCont == StageMake.LoadStageData.DoThunder && !IsThunder && StageMake.LoadStageData.IsThunder))
            {
                IsThunder = true;
                PlayState.playState.gameMode = PlayState.GameMode.Thunder;

               
            }
            else
            {
                playState.gameMode = GameMode.Play;

            }
        }
    }
    void ThunderUpDate()
    {
        ThunderTime = (ThunderTime - Time.deltaTime < 0) ? 0 : ThunderTime - Time.deltaTime;
        if (ThunderTime == 0)
        {
            playState.gameMode = GameMode.Play;
        }
    }
}
