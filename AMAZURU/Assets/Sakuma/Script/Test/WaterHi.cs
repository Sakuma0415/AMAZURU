using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterHi : MonoBehaviour
{
    public float max=0.25f;
    float back;
    float next;
    bool rainAnime = false;
    float animeTime = 0;
    float animeTimeTik = 0;
    [SerializeField]
    Material[] material;
    [SerializeField]
    float anmeSpead = 1;
    public DigitalRuby.RainMaker.RainScript[] rainScript;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (rainAnime)
        {


            animeTimeTik+= Time.fixedDeltaTime;
            //animeTime -= Time.fixedDeltaTime;
            if (animeTimeTik < animeTime-0.5f && animeTimeTik > 0.5f)
            {
                max = Mathf.Lerp(back, next, (animeTimeTik - 0.5f)/ (animeTime-1)* anmeSpead);

            }
            if (animeTimeTik > animeTime - 1)
            {
                for (int i = 0; i < rainScript.Length; i++)
                {
                    rainScript[i].RainIntensity = 0.03f;
                }
                max = next;
            }
            if (animeTimeTik > animeTime - 0.5f)
            {
                Camera.main.gameObject.GetComponent<CameraPos>().RainPotChangeOut();
                rainAnime = false;
            }
        }







        for (int i=0;i<material.Length;i++)
        {

            material[i].SetFloat("_High", max);
        }


        transform.localScale = new Vector3(transform.localScale.x, max, transform.localScale.z);
    }


    public void HiChange(float nextHi)
    {
        if (!rainAnime)
        {
            for (int i = 0; i < rainScript.Length; i++)
            {
                rainScript[i].RainIntensity = 1;
            }
            PlayState.playState.gameMode = PlayState.GameMode.Rain;
            //
            PlayState.playState.rainTime = Mathf.Abs ( (max+0.1f)-nextHi)/ anmeSpead + 1.5f;
            back = transform.localScale.y;
            next = nextHi-0.1f;
            rainAnime = true;

            animeTime = Mathf.Abs((max + 0.1f) - nextHi) / anmeSpead + 1.5f;
            animeTimeTik = 0;



            
        }


    }





}
