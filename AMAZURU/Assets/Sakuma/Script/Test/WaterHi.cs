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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (rainAnime)
        {
            animeTime -= Time.fixedDeltaTime;
            if (animeTime < 1.5f && animeTime > 0.5f)
            {
                //transform.localScale = new Vector3(transform.localScale.x, Mathf.Lerp(back, next,1-(animeTime -0.5f)), transform.localScale.z);
                max = Mathf.Lerp(back, next, 1 - (animeTime - 0.5f));
            }
            if (animeTime < 0.5f)
            {
                //transform.localScale = new Vector3(transform.localScale.x, next, transform.localScale.z);
                max = next;
            }
            if (animeTime <= 0)
            {
                rainAnime = false;
            }
        }









        transform.localScale = new Vector3(transform.localScale.x, max, transform.localScale.z);
    }
    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.U)) { HiChange(4); }
        //if (Input.GetKey(KeyCode.I)) { max -= Time.deltaTime; }





        
    }

    public void HiChange(float nextHi)
    {
        if (!rainAnime)
        {
            PlayState.playState.gameMode = PlayState.GameMode.Rain;
            back = transform.localScale.y;
            next = nextHi-0.1f;
            rainAnime = true;
            animeTime = 2;
        }


    }





}
