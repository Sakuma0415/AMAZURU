using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StepMode
{
    Nomal,
    InWater,
    UnderWater
}

public class PlayerAnimeEvent : MonoBehaviour
{
    // 足音のボリューム
    private float volume = 0.5f;

    public StepMode PlayerStepMode { set; private get; } = StepMode.Nomal;

    /// <summary>
    /// プレイヤーの座標情報
    /// </summary>
    public Vector3 PlayerPosition { set; private get; } = Vector3.zero;

    /// <summary>
    /// プレイヤーアニメーションのイベントから呼び出す足音を再生する関数
    /// </summary>
    public void Step()
    {
        if(PlayerStepMode == StepMode.UnderWater)
        {
            // 完全にプレイヤーが水没している際の足音
            SoundManager.soundManager.PlaySe3D("step_under_water", PlayerPosition, volume);
        }
        else if(PlayerStepMode == StepMode.InWater)
        {
            // プレイヤーが腰の高さまで水に浸かっている際の足音
            float rnd = Random.Range(0, 10);
            if(rnd % 2 == 0)
            {
                SoundManager.soundManager.PlaySe3D("step_in_water_1", PlayerPosition, volume);
            }
            else
            {
                SoundManager.soundManager.PlaySe3D("step_in_water_2", PlayerPosition, volume);
            }
        }
        else
        {
            // 通常時のプレイヤーの足音
            SoundManager.soundManager.PlaySe3D("FitGround_Dast2_1", PlayerPosition, volume);
        }
    }
}
