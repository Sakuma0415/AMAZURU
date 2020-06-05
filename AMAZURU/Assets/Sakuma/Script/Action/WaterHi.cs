using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 水の高さを管理するクラス
/// </summary>
public class WaterHi : MonoBehaviour
{
    [Header("設定項目")]

    //水面のマテリアル
    [SerializeField]
    Material[] material;
    //雨のアニメーションの再生速度
    [SerializeField]
    float anmeSpead = 1;

    [Header("変更不可")]

    //水の高さ
    public float max=0.98f;

    //private

    //水位が変化する前の水の高さ
    float back;
    //水位が変化した後の水の高さ
    float next;
    //雨のアニメーション中かどうか
    bool rainAnime = false;
    //雨のアニメーションの全体時間
    float animeTime = 0;
    //雨のアニメーションの経過時間
    float animeTimeTik = 0;

    //雨のアニメの前後の空きフレーム
    float ChangeTime;

    //初期化処理
    void Start()
    {
        ChangeTime = Camera.main.GetComponent<CameraPos>().rainPotChangeAnimeTimeSpead ;
    }

    //フレーム処理
    private void FixedUpdate()
    {

        //雨のアニメ中
        if (rainAnime)
        {
            animeTimeTik+= Time.fixedDeltaTime;
            if (animeTimeTik < animeTime-1f && animeTimeTik > 1f)
            {
                max = Mathf.Lerp(back, next, (animeTimeTik - 1f)/ (animeTime-2f)* anmeSpead);

            }
            if (animeTimeTik > animeTime - 1f)
            {
                max = next;
            }
            if (animeTimeTik > animeTime -ChangeTime)
            {
                Camera.main.gameObject.GetComponent<CameraPos>().RainPotChangeOut();
                rainAnime = false;
            }
        }
        
        //水のマテリアルのステータス更新
        for (int i=0;i<material.Length;i++)
        {

            material[i].SetFloat("_High", max);
            material[i].SetFloat("_Xside", transform.localScale.x);
            material[i].SetFloat("_Zside", transform.localScale.z);
        }

        //水の高さ更新
        transform.localScale = new Vector3(transform.localScale.x, max, transform.localScale.z);
    }

    //高さを変更する関数
    public void HiChange(float nextHi)
    {
        if (!rainAnime)
        {
            PlayState.playState.gameMode = PlayState.GameMode.Rain;

            PlayState.playState.rainTime = (Mathf.Abs ( (max+0.02f)-nextHi)/ anmeSpead )+ 2f;
            back = transform.localScale.y;
            next = nextHi- 0.02f;
            rainAnime = true;

            animeTime = (Mathf.Abs((max + 0.02f) - nextHi) / anmeSpead) + 2f;
            animeTimeTik = 0;
        }
    }
}
