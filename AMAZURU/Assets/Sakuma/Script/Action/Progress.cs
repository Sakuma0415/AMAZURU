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
    public WaterHi waterHi;
    //Clear時に呼び出すresult
    [SerializeField]
    MenuMaster gameMenu = null;
    //resultを描画し始めるまでの時間
    [SerializeField]
    float ResultDelayTime;
    [SerializeField]
    AnimationClip[] animation;

    public Animator animator;
    bool IsThunder = false;

        //初期化
    void SetState()
    {
        key = false;
    }

    void Start()
    {
        SetState();
        progress = this;
        SoundManager.soundManager.PlayBgm("PerituneMaterial_Wonder3_loop", 0.5f, 0.1f, 1);

        IsThunder = false;
        //謎
        //SoundManager.soundManager.StopBgm(1f,0);
    }
    
    private void Update()
    {



        // プレイヤーと敵が接触した場合はポーズ画面を閉じる(開けないようにする)
        bool isHitEnemy = CharacterMaster.Instance.Player.IsHitEnemy;

        if (isHitEnemy)
        {
            if (PlayState.playState.gameMode == PlayState.GameMode.Pause)
            {
                gameMenu.Pause(false);
                PlayState.playState.gameMode = PlayState.GameMode.Play;
            }
        }
        else
        {
            //ポーズ画面の開閉
            if (ControllerInput.Instance.buttonDown.option)
            {
                if (PlayState.playState.gameMode == PlayState.GameMode.Play)
                {
                    gameMenu.Pause(true);
                    PlayState.playState.gameMode = PlayState.GameMode.Pause;
                }
                else
                if (PlayState.playState.gameMode == PlayState.GameMode.Pause)
                {
                    gameMenu.Pause(false);
                    PlayState.playState.gameMode = PlayState.GameMode.Play;
                }
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
        SoundManager.soundManager.VolFadeBgm(1,0.1f,0);
        SoundManager.soundManager.StopBgm(1, 1);
        float timeob = 0;
        if (GameOver)
        {
            while (!animator.GetBool("StageClear")||timeob >2f)
            {
                timeob += Time.deltaTime;
                yield return null;
            }
        }
        if (GameOver)
        {
            SoundManager.soundManager.PlaySe("wafu-success", 1);
        }
        else
        {
            SoundManager.soundManager.PlaySe("dead-sound", 1);
        }
        yield return new WaitForSeconds(animation[GameOver?0:1].length+ResultDelayTime);


        gameMenu.StartResult(GameOver);
    }

    public void NextStage()
    {
        StageMake.LoadStageData = StageMake.LoadStageData.NextStageData;
        SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
    }

}
