using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class O2Controller : MonoBehaviour
{
    public bool breath=true;
    [SerializeField]
    float breathLimitTime;
    float breathTime=0;
    [SerializeField]
    Image gage;
    [SerializeField]
    float fatAnimeTimeLimit;
    float fatAnimeTime = 0;
    [SerializeField]
    RectTransform rectTransform;

    public PlayerType2 playerType2;
    void Update()
    {

        breath = !playerType2.UnderWater;

        if (!breath)
        {
            if(PlayState .playState.gameMode ==PlayState.GameMode.Play )
            breathTime = (breathTime + Time.deltaTime  > breathLimitTime) ? breathLimitTime : breathTime + Time.deltaTime;
        }
        else
        {
            breathTime = (breathTime - (Time.deltaTime * 5) < 0) ? 0 : breathTime - (Time.deltaTime * 5);
        }

        float late = 1 - (breathTime / breathLimitTime);
        gage.fillAmount = late;
        late = 1 - late;
        gage.color = new Color(late < 0.5f ? late*2:1, late > 0.5f ? 1-((late -0.5f)*2) : 1, gage.color.b, 1);
        
        if (breathTime > 0)
        {
            fatAnimeTime = fatAnimeTime+Time.deltaTime<fatAnimeTimeLimit ? fatAnimeTime + Time.deltaTime: fatAnimeTimeLimit;
            rectTransform.localScale = new Vector3(1+((fatAnimeTime/ fatAnimeTimeLimit) *0.1f), 1 + ((fatAnimeTime / fatAnimeTimeLimit) * 0.1f),1);
        }
        else
        {
            fatAnimeTime = fatAnimeTime - Time.deltaTime > 0 ? fatAnimeTime - Time.deltaTime : 0;
            rectTransform.localScale = new Vector3(1 + ((fatAnimeTime / fatAnimeTimeLimit) * 0.1f),  1 + ((fatAnimeTime / fatAnimeTimeLimit) * 0.1f),1);
        }
        if(breathTime>= breathLimitTime)
        {
            PlayState.playState.gameMode = PlayState.GameMode.GameOver;
        }

        if (playerType2.ContactEnemy)
        {
            PlayState.playState.gameMode = PlayState.GameMode.GameOver;
        }

    }
}
