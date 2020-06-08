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
    //resultを描画し始めるまでの時間
    [SerializeField]
    float ResultDelayTime;
    [SerializeField]
    AnimationClip[] animation;
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
        StartCoroutine(ResultDelay(true));
    }

    //gameover画面を呼び出す関数
    public void GameOverSet()
    {
        StartCoroutine(ResultDelay(false));
    }

    IEnumerator ResultDelay(bool GameOver)
    {
        SoundManager.soundManager.VolFadeBgm(1,0.2f,0);
        SoundManager.soundManager.StopBgm(1, 1);
        yield return new WaitForSeconds(animation[GameOver?0:1].length+ResultDelayTime);
        if(GameOver)
        {
            SoundManager.soundManager.PlaySe("wafu-success", 1);
        }
        else
        {
            SoundManager.soundManager.PlaySe("dead-sound", 1);
        }
        resultControl.StartResult(GameOver);
    }

}
