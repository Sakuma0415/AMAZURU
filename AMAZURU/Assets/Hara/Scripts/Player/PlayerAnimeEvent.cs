using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimeEvent : MonoBehaviour
{
    /// <summary>
    /// 水中移動時のフラグ
    /// </summary>
    public bool WaterStep { set; private get; } = false;

    /// <summary>
    /// プレイヤーアニメーションのイベントから呼び出す足音を再生する関数
    /// </summary>
    public void Step()
    {
        if (WaterStep)
        {
            // 水中時
            SoundManager.soundManager.PlaySe3D("FitGround_Dast2_1", transform.position, 0.3f);
        }
        else
        {
            // 通常時
            SoundManager.soundManager.PlaySe3D("FitGround_Dast2_1", transform.position, 0.3f);
        }
    }
}
