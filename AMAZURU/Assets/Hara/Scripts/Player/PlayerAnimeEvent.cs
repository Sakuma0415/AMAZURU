using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimeEvent : MonoBehaviour
{
    /// <summary>
    /// プレイヤーアニメーションのイベントから呼び出す足音を再生する関数
    /// </summary>
    public void Step()
    {
        SoundManager.soundManager.PlaySe3D("FitGround_Dast2_1", transform.position, 0.3f);
    }
}
