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
    SimpleRainBehaviour simpleRain;
    //[SerializeField]
    //StaticRainBehaviour staticRain;

    [SerializeField]
    GameObject simpleRainObj;
    [SerializeField]
    RainCameraController rainCamera;
    [SerializeField]
    InCamera inCamera;
    private void Start()
    {
        AmehurashiCont =-1;
        simpleRainObj.SetActive(true);
        rainCamera.Play ();
    }

    // Update is called once per frame
    void Update()
    {

        if (AmehurashiCont == 0|| inCamera.set)
        {
            rainCamera.Stop();
        }
        else
        {
            rainCamera.Play();
        }



        if (AmehurashiCont!= AmehurashiManager.amehurashi.amehurashiTrueCont)
        {
            AmehurashiCont = AmehurashiManager.amehurashi.amehurashiTrueCont;
            float def = (float)AmehurashiCont / (float)AmehurashiManager.amehurashi.AmehurashiQuantity;
            def *= def;
            for (int i = 0; i < rainScript.Length; i++)
            {
                rainScript[i].RainIntensity = def;
            }
            simpleRain.Variables .EmissionRateMax = (int)(rainMax * def) ;
            simpleRain.Variables.EmissionRateMin = (int)(rainMax * (def / 2)) ;



        }



        AmehurashiCont = AmehurashiManager.amehurashi.amehurashiTrueCont;
    }
}
