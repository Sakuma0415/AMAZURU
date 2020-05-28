using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージのデータを格納しておくクラス
/// </summary>
[CreateAssetMenu(menuName = "Scriptable/StageData")]
public class StageData : ScriptableObject
{

    //ステージの名前
    public string stageName;
    //ステージの大きさ
    public Vector3 stageSize;
    //開始地点
    public Vector3 startPos;
    //アメフラシ一つで上昇する水の量
    public float waterStep;
    //ステージのプレファブ
    public GameObject stagePrefab;
    //アメフラシの数
    public int AmehurashiQuantity;
    //カメラの初期角度
    public float startAngle;
    //ステージ-カメラ間の距離
    public float CameraDisS;
    //プレイヤー-カメラ間の距離
    public float CameraDisP;
    //チュートリアルがあるかどうかのフラグ
    public bool TutorialFlg=false ;
    //チュートリアルに表示するオブジェ
    public GameObject[] TutorialObjs; 
}