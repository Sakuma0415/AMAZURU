using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// アメフラシ全体で共有する情報のクラス
/// </summary>
public class AmehurashiManager : MonoBehaviour
{
    //アクセス先の静的オブジェ
    static public AmehurashiManager amehurashi;

    [Header("StageDataからの読み込みのため以下変更不可")]
    //水位の高さ
    public WaterHi waterHi;
    //アメフラシ一つで上昇する水位の高さ
    public float waterStep = 1;
    //ステージ上に存在するアメフラシの数
    public int AmehurashiQuantity = 0;
    //アメフラシを起動している数
    public int amehurashiTrueCont = 0;
    //アメフラシのシーケンサー
    private int amehurashiBackTrueCont = 0;
    //雨のBGMの連続性を検知するための変数
    private int backRainBGM = -1;
    //+　初期化
    public void ManagerSet()
    {
        amehurashi = this;
    }

    //フレーム処理
    private void Update()
    {

        //アメフラシの起動数変更時BGM更新
        if(amehurashiBackTrueCont != amehurashiTrueCont)
        {
            Debug.Log((float)amehurashiTrueCont / (float)AmehurashiQuantity);
            if((float)amehurashiTrueCont / (float)AmehurashiQuantity > 0.5f)
            {
                if (backRainBGM != 2)
                {
                    backRainBGM = 2;
                    SoundManager.soundManager.PlayBgm("haevy_rain_loop", 0.5f, 0.3f,0);
                }
            }
            else if((float)amehurashiTrueCont / (float)AmehurashiQuantity > 0)
            {
                if (backRainBGM != 1)
                {
                    backRainBGM = 1;
                    SoundManager.soundManager.PlayBgm("rain_loop", 0.5f, 0.8f,0);
                }
            }
            else
            {
                if (backRainBGM != 0)
                {
                    backRainBGM = 0;
                    SoundManager.soundManager.StopBgm(1f,0);
                }
            }
        }
        amehurashiBackTrueCont = amehurashiTrueCont;
    }

    //凍結中
    /*
        以下アメフラシ起動時にウィンドウを表示する関数
        未使用だが参考、再使用の可能性も考慮し保持
    */
#if false
    public float hi;
    public RainPot rainPot;
    [SerializeField]
    float[] hiList = new float[0];

    public void SwOn()
    {
        if (!rainPot.sw)
        {
            Debug.Log("ONにした");
            rainPot.sw = true;

            Array.Resize(ref hiList, hiList.Length +1);
            hiList[hiList.Length - 1] = amehurashi.hi;
            float max = 1;
            for(int i=0;i< hiList.Length; i++)
            {
                if(max< hiList[i])
                {
                    max = hiList[i];
                }

            }
            waterHi.HiChange((hiList.Length* waterStep) +1);

        }
        else
        {
            RainCancel();
        }
    }

    public void SwOff()
    {
        if (rainPot.sw)
        {
            Debug.Log("Offにした");
            rainPot.sw = false;

            float[] hiList2 = new float[hiList.Length-1] ;
            bool cost = false;

            for(int i = 0; i < hiList.Length; i++)
            {
                if(hiList[i]== amehurashi.hi && !cost)
                {
                    cost = true;
                }
                else
                {
                    hiList2[i - (cost ? 1 : 0)] = hiList[i];
                }
                
            }
            hiList = hiList2;

            float max = 1;
            for (int i = 0; i < hiList.Length; i++)
            {
                if (max < hiList[i])
                {
                    max = hiList[i];
                }

            }
            waterHi.HiChange((hiList.Length * waterStep) + 1);
        }
        else
        {
            RainCancel();
        }
    }

    public void RainCancel()
    {
        Camera.main.gameObject.GetComponent<CameraPos>().RainPotChangeOut();
        PlayState.playState.gameMode = PlayState.GameMode.Play;
    }
#endif
}
