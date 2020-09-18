using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEnemy : MonoBehaviour
{
    [SerializeField, Tooltip("帯電エフェクト")] private ParticleSystem electricEffect = null;

    /// <summary>
    /// ゲーム停止フラグ
    /// </summary>
    public bool IsGameStop { set; private get; } = false;

    /// <summary>
    /// 帯電化させるエネミーの情報
    /// </summary>
    public EnemyController EnemyObject { set; get; } = null;

    /// <summary>
    /// エネミーの帯電状態フラグ
    /// </summary>
    public bool IsElectric { private set; get; } = false;

    public GameObject[] EnemyModels { set; private get; } = null;
    public Animator[] EnemyAnimators { set; private get; } = null;

    /// <summary>
    /// エネミーを帯電化させる
    /// </summary>
    /// <param name="active">trueなら帯電化、falseなら帯電解除</param>
    public void ElectricMode(bool active)
    {
        IsElectric = active;

        for(int i = 0; i < EnemyModels.Length; i++)
        {
            EnemyModels[i].SetActive(!EnemyModels[i].activeSelf);
            if (EnemyModels[i].activeSelf)
            {
                EnemyObject.EnemyAnime = EnemyAnimators[i];
            }
        }
    }

    private void Update()
    {
        if (IsGameStop)
        {
            if (electricEffect.isPlaying)
            {
                electricEffect.Pause();
            }
        }
        else
        {
            if (electricEffect.isPaused)
            {
                electricEffect.Play();
            }
        }
    }
}
