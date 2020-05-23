using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///雨のエフェクトを管理するクラス 
/// </summary>
public class RainEfController : MonoBehaviour
{

    [Header("設定項目")]

    //雨のエフェクト
    public DigitalRuby.RainMaker.RainScript[] rainScript;
    //雨のエフェクトの最大量
    [SerializeField]
    float rainMax;
    //画面につく雨のエフェクト
    [SerializeField]
    SimpleRainBehaviour simpleRain;
    //画面につく雨のエフェクトのクラス
    [SerializeField]
    RainCameraController rainCamera;
    //水中でカメラにエフェクトをかけるクラス
    [SerializeField]
    InCamera inCamera;

    //private

    //アメフラシの起動数の変化を検知するバッファ
    int AmehurashiCont;

    //初期化処理
    private void Start()
    {
        AmehurashiCont =-1;
        rainCamera.Play ();
    }

    //フレーム処理
    void Update()
    {

        //アメフラシの起動数0 OR カメラが水中の場合水滴のエフェクトを切る
        if (AmehurashiCont == 0|| inCamera.set)
        {
            rainCamera.Stop();
        }
        else
        {
            rainCamera.Play();
        }

        //アメフラシの起動数が変動した時
        if (AmehurashiCont!= AmehurashiManager.amehurashi.amehurashiTrueCont)
        {
            AmehurashiCont = AmehurashiManager.amehurashi.amehurashiTrueCont;

            //起動数で雨の量を決定
            float def = (float)AmehurashiCont / (float)AmehurashiManager.amehurashi.AmehurashiQuantity;
            //def *= def;
            for (int i = 0; i < rainScript.Length; i++)
            {
                rainScript[i].RainIntensity = def;
            }
            simpleRain.Variables .EmissionRateMax = (int)(rainMax * def) ;
            simpleRain.Variables.EmissionRateMin = (int)(rainMax * (def / 2)) ;
        }

        //アメフラシの起動数を同期
        AmehurashiCont = AmehurashiManager.amehurashi.amehurashiTrueCont;
    }
}
