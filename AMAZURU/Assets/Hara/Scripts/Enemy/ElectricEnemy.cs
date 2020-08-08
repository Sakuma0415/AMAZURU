using Enemy;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricEnemy : MonoBehaviour
{
    /// <summary>
    /// 帯電化させるエネミーの情報
    /// </summary>
    public EnemyController EnemyObject { set; get; } = null;

    /// <summary>
    /// エネミーの帯電状態フラグ
    /// </summary>
    public bool IsElectric { private set; get; } = false;

    private GameObject[] enemyModels = null;
    private Animator[] enemyAnimators = null;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Init()
    {
        enemyModels = new GameObject[transform.childCount];
        enemyAnimators = new Animator[transform.childCount];

        for(int i = 0; i < transform.childCount; i++)
        {
            enemyModels[i] = transform.GetChild(i).gameObject;
            enemyAnimators[i] = enemyModels[i].GetComponent<Animator>();
        }
    }

    /// <summary>
    /// エネミーを帯電化させる
    /// </summary>
    /// <param name="active">trueなら帯電化、falseなら帯電解除</param>
    public void ElectricMode(bool active)
    {
        IsElectric = active;

        for(int i = 0; i < enemyModels.Length; i++)
        {
            enemyModels[i].SetActive(!enemyModels[i].activeSelf);
            if (enemyModels[i].activeSelf)
            {
                EnemyObject.EnemyAnime = enemyAnimators[i];
            }
        }
    }
}
