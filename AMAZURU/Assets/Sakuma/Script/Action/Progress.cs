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
    //鍵の取得のフラグ(現在未使用)
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
