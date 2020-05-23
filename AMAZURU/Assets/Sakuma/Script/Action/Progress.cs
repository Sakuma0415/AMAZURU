using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの進行状況を管理するクラス
/// </summary>
public class Progress : MonoBehaviour
{
    //Instance
    static public Progress progress;
    //水中呼吸アイテム取得のフラグ
    public bool key;
    //Clear時に呼び出すresult
    [SerializeField]
    ResultControl resultControl;
    //初期化
    void SetState()
    {
        key = false;
    }

    void Start()
    {
        SetState();
        progress = this;
        SoundManager.soundManager.PlayBgm("PerituneMaterial_Wonder3_loop", 0.5f, 0.4f, 1);
        //謎
        //SoundManager.soundManager.StopBgm(1f,0);
    }
    
    private void Update()
    {


        //ポーズ画面の開閉
        if (Input.GetButtonDown("Option"))
        {
            if (PlayState.playState.gameMode == PlayState.GameMode.Play)
            {
                resultControl.GamePause(true);
                PlayState.playState.gameMode = PlayState.GameMode.Pause;
            }
            else
            if (PlayState.playState.gameMode == PlayState.GameMode.Pause)
            {
                resultControl.GamePause(false);
                PlayState.playState.gameMode = PlayState.GameMode.Play;
            }
        }
    }

    //result画面を呼び出す関数
    public void ResultSet()
    {
        resultControl.StartResult( true);
    }

    //gameover画面を呼び出す関数
    public void GameOverSet()
    {
        resultControl.StartResult(false );
    }
}
