using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーの酸素を管理するクラス
/// </summary>
public class O2Controller : MonoBehaviour
{

    [Header("設定項目")]
    //酸素ゲージのImage
    [SerializeField]
    Image gage;
    //酸素ゲージのRectTransform
    [SerializeField]
    RectTransform rectTransform;
    //酸素ゲージ変更中にゲージを拡大するのにかける時間
    [SerializeField]
    float fatAnimeTimeLimit;
    //呼吸が続く時間
    [SerializeField]
    float breathLimitTime;

    [Header("変更不要")]
    //呼吸できる状態かのフラグ
    public bool breath = true;
    //プレイヤーの情報所得用
    public CharacterMaster master;
    //水中死亡のフラグ
    public bool WaterDeth = false;

    //private
    //呼吸できない状態の経過時間
    float breathTime = 0;
    //酸素ゲージ変更中にゲージを拡大している時間
    float fatAnimeTime = 0;

    float now = 0;
    public  Material material;
    private void Start()
    {
        WaterDeth = false;
    }

    void Update()
    {

        //呼吸できるかの状態を取得
        breath = !master.Player.UnderWater;

        //プレイ中のみ処理
        if (PlayState.playState.gameMode == PlayState.GameMode.Play)
        {

            //呼吸可能時間の増減
            if (!breath&&!Progress .progress.key )
            {
                breathTime = (breathTime + Time.deltaTime > breathLimitTime) ? breathLimitTime : breathTime + Time.deltaTime;
            }
            else
            {
                breathTime = (breathTime - (Time.deltaTime * 5) < 0) ? 0 : breathTime - (Time.deltaTime * 5);
            }
        }

        //酸素ゲージの色、ゲージの更新
        float late = 0.925f- ((((breathTime / breathLimitTime)) / 2)*(0.6f/0.5f));
        gage.fillAmount = late;
        late = 1 - late;
        //gage.color = new Color(late < 0.5f ? late*2:1, late > 0.5f ? 1-((late -0.5f)*2) : 1, gage.color.b, 1);
        
        //酸素ゲージ拡大する処理
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

        //酸素なくなった時の処理
        if(breathTime>= breathLimitTime)
        {
            WaterDeth = true;
            PlayState.playState.gameMode = PlayState.GameMode.GameOver;
        }

        //敵接触時の処理(移設予定)
        if (master.IsGameOver)
        {
            PlayState.playState.gameMode = PlayState.GameMode.GameOver;
        }


        if(breathTime/ breathLimitTime < 1f / 3f)
        {
            gage.color = new Color(0,1, 1f-(breathTime / (breathLimitTime/3f)));
        }
        else if (breathTime / breathLimitTime < 2f / 3f)
        {
            gage.color = new Color(((breathTime- (breathLimitTime / 3f)) / (breathLimitTime / 3)), 1, 0);
        }
        else
        {
            gage.color = new Color(1, 1-((breathTime - (2*(breathLimitTime / 3f))) / (breathLimitTime / 3)), 0);
        }

        float AMFlate = (float )AmehurashiManager.amehurashi.amehurashiTrueCont / (float)AmehurashiManager.amehurashi.AmehurashiQuantity;
        if (now+(0.1f*Time.deltaTime )< AMFlate)
        {
            now += 0.1f * Time.deltaTime;
        }

        if (now - (0.1f * Time.deltaTime) > AMFlate)
        {
            now -= 0.1f * Time.deltaTime;
        }


        material.SetFloat("_Gage", now);











    }
}
