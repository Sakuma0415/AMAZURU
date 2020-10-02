using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

[CreateAssetMenu(menuName = "Scriptable/EnemyData")]
public class EnemyData : ScriptableObject
{
    // 敵の種類
    public EnemyType Type = EnemyType.Normal;

    // スタート向き
    public enum RotateDirection
    {
        Forward,
        Back,
        Right,
        Left
    }
    public RotateDirection StartRotate = RotateDirection.Forward;

    // サイズ倍率
    public float Size = 1.0f;

    // スタート時の位置を設定するか
    public bool UseStartPosSetting = false;

    // スタート時の座標
    public Vector3 StartPosition = Vector3.zero;

    // 行動計画
    [Header("行動計画")]
    public Vector3[] MovePlan = null;

    // 行動パターン
    public EnemyMoveType MoveType = EnemyMoveType.Lap;

    // 移動速度
    public float NomalSpeed = 1.0f;

    // 水中速度
    public float WaterSpeed = 1.0f;

    // デフォルト設定を使用するか
    public bool UseDefaultSetting = true;

    // 乾燥ブロックの設置高さ
    public int BlockSetPosY = 0;

    // 乾燥ブロック状態に戻すか
    public bool ReturnBlock = false;
}