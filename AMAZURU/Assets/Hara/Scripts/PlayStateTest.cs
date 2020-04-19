using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayStateTest : MonoBehaviour
{
    [SerializeField, Tooltip("GameMode")] private PlayState.GameMode mode = PlayState.GameMode.Play;
    void Update()
    {
        PlayState.playState.gameMode = mode;
    }
}
