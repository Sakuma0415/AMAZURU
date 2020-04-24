using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHi : MonoBehaviour
{
    public float max=0.9f;
    float back;
    float next;
    bool rainAnime = false;
    float animeTime = 0;
    float animeTimeTik = 0;
    [SerializeField]
    Material[] material;
    [SerializeField]
    float anmeSpead = 1;
    //public DigitalRuby.RainMaker.RainScript[] rainScript;
    float ChangeTime;

    void Start()
    {
        ChangeTime = Camera.main.GetComponent<CameraPos>().rainPotChangeAnimeTimeSpead ;
    }

    private void FixedUpdate()
    {
        if (rainAnime)
        {


            animeTimeTik+= Time.fixedDeltaTime;
            //animeTime -= Time.fixedDeltaTime;
            if (animeTimeTik < animeTime-1f && animeTimeTik > 1f)
            {
                max = Mathf.Lerp(back, next, (animeTimeTik - 1f)/ (animeTime-2f)* anmeSpead);

            }
            if (animeTimeTik > animeTime - 1f)
            {
                //for (int i = 0; i < rainScript.Length; i++)
                //{
                //    rainScript[i].RainIntensity = 0.03f;
                //}
                max = next;
            }
            if (animeTimeTik > animeTime -ChangeTime)
            {
                Camera.main.gameObject.GetComponent<CameraPos>().RainPotChangeOut();
                rainAnime = false;
            }
        }







        for (int i=0;i<material.Length;i++)
        {

            material[i].SetFloat("_High", max);
            material[i].SetFloat("_Xside", transform.localScale.x);
            material[i].SetFloat("_Zside", transform.localScale.z);
        }


        transform.localScale = new Vector3(transform.localScale.x, max, transform.localScale.z);
    }


    public void HiChange(float nextHi)
    {
        if (!rainAnime)
        {
            //for (int i = 0; i < rainScript.Length; i++)
            //{
            //    rainScript[i].RainIntensity = 1;
            //}
            PlayState.playState.gameMode = PlayState.GameMode.Rain;
            //
            Debug.Log((Mathf.Abs((max + 0.1f) - nextHi) / anmeSpead) + 2f);
            PlayState.playState.rainTime = (Mathf.Abs ( (max+0.1f)-nextHi)/ anmeSpead )+ 2f;
            back = transform.localScale.y;
            next = nextHi-0.1f;
            rainAnime = true;

            animeTime = (Mathf.Abs((max + 0.1f) - nextHi) / anmeSpead) + 2f;
            animeTimeTik = 0;



            
        }


    }





}
