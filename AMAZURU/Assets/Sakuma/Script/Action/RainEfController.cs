using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainEfController : MonoBehaviour
{
    int AmehurashiCont;
    public DigitalRuby.RainMaker.RainScript[] rainScript;

    [SerializeField]
    float rainMax;
    [SerializeField]
    float maxSta;

    [SerializeField]
    SimpleRainBehaviour simpleRain;
    [SerializeField]
    StaticRainBehaviour staticRain;

    private void Start()
    {
        AmehurashiCont =-1;
    }

    // Update is called once per frame
    void Update()
    {
        if(AmehurashiCont!= AmehurashiManager.amehurashi.amehurashiTrueCont)
        {
            AmehurashiCont = AmehurashiManager.amehurashi.amehurashiTrueCont;
            float def = (float)AmehurashiCont / (float)AmehurashiManager.amehurashi.AmehurashiQuantity;
            def *= def;
            for (int i = 0; i < rainScript.Length; i++)
            {
                rainScript[i].RainIntensity = def;
            }
            simpleRain.Variables .EmissionRateMax = (int)(rainMax * def) +1;
            simpleRain.Variables.EmissionRateMin = (int)(rainMax * (def / 2)) + 1;
            staticRain.Alpha = def * maxSta;
        }



        AmehurashiCont = AmehurashiManager.amehurashi.amehurashiTrueCont;
    }
}
