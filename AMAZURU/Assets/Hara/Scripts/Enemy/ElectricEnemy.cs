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
    /// ステージの帯電状態フラグ
    /// </summary>
    public bool IsStageElectric { private set; get; } = false;

    [SerializeField] private bool isElectric = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckState();
    }

    /// <summary>
    /// エネミーを帯電化させる
    /// </summary>
    /// <param name="active">trueなら帯電化、falseなら帯電解除</param>
    public void ElectricMode(bool active)
    {
        isElectric = active;
    }

    /// <summary>
    /// 帯電状態のナマコが水中にいるかをチェック
    /// </summary>
    private void CheckState()
    {
        if (isElectric && EnemyObject != null)
        {
            IsStageElectric = EnemyObject.InWater;
        }
        else
        {
            IsStageElectric = false;
        }
    }
}
