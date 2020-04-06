using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class AmehurashiManager : MonoBehaviour
{
    static public AmehurashiManager amehurashi;
    public WaterHi waterHi;

    public void ManagerSet()
    {
        amehurashi = this;
    }
    public float waterStep = 1;
    public float hi;
    public int amehurashiTrueCont = 0;
    public int AmehurashiQuantity = 0;
    public RainPot rainPot;
    [SerializeField]
    float[] hiList = new float[0];

    //凍結中
#if false
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
