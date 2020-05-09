using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// サウンドを管理するクラス
/// </summary>
public class SoundManager : MonoBehaviour
{
    //Instance
    static public SoundManager soundManager;

    [Header ("設定項目")]
    //2DのSEを再生させるオーディオ
    [SerializeField]
    AudioSource seAudioSource;
    //2DのBGMを再生させるオーディオ
    [SerializeField]
    AudioSource bgmAudioSource;
    //2DのBGMを再生させるオーディオ
    [SerializeField]
    AudioSource bgmAudioSource2;
    //3Dサウンドの再生地点を記憶するオブジェの親
    [SerializeField]
    Transform audioParent;
    
    //private

    //BGMのボリューム
    float MaxVolume = 0;
    //BGMがフェード中かどうかのフラグ
    bool bgmFade = false;
    //フェード後に再生するBGMの退避先
    AudioClip fadeBf;
    //フェードインの時間
    float inTime = 0;
    //フェードアウトの時間
    float outTime = 0;
    //フェードインアウトの全体時間
    float fadelate = 0;
    //フェードアウト後音量を格納しておく場所
    float afterVol = 0;
    //BGM停止時にフェードする場合のフラグ
    bool stopFade = false;

    //3Dサウンドの再生オブジェの状態をまとめた構造体
    struct ClipList3D
    {
        //再生時間
        public float time;
        //再生しているオブジェ
        public GameObject soundObject;
        //再生しているオーディオ
        public AudioSource audioSource;

        //初期化
        public void StateSet(Transform parent)
        {
            time = 0;
            soundObject = new GameObject();
            soundObject.transform.parent = parent;
            soundObject.AddComponent<AudioSource>();
            soundObject.name = "3DAudioSource";
            audioSource=soundObject.GetComponent<AudioSource>();
            audioSource.spatialBlend = 1;
            audioSource.maxDistance  = 7;
            audioSource.minDistance = 1;
            audioSource.rolloffMode = AudioRolloffMode.Linear;
        }
    }

    //構造体の配列
    ClipList3D[] clipList3Ds=new ClipList3D[1];

    //起動時
    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;
            DontDestroyOnLoad(gameObject);
            clipList3Ds[0].StateSet(audioParent);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //フレーム処理
    void Update()
    {

        //BGMがフェード処理中
        if (bgmFade)
        {

            if(outTime > 0)
            {

                //フェードアウトの処理
                outTime -= Time.deltaTime;
                bgmAudioSource.volume = (outTime / fadelate) * MaxVolume;

                if (outTime <= 0)
                {
                    if (stopFade)
                    {
                        bgmFade = false;
                        stopFade = false;
                        bgmAudioSource.Stop();
                        bgmAudioSource.clip = null;
                    }
                    else
                    {
                        outTime = 0;
                        bgmAudioSource.clip = fadeBf;
                        MaxVolume = afterVol;
                        bgmAudioSource.Play();
                    }
                }
            }
            else
            {
                //フェードインの処理
                inTime -= Time.deltaTime;
                bgmAudioSource.volume = MaxVolume - ((inTime / fadelate) * MaxVolume);

                if (inTime <= 0)
                {
                    inTime = 0;
                    bgmFade = false;
                }
            }
        }

        //3Dサウンドの再生時間更新
        for(int i=0;i< clipList3Ds.Length; i++)
        {
            clipList3Ds[i].time -= Time.deltaTime;
        }
    }

    //BGM停止
    public void StopBgm(float fadeTime)
    {
        if (fadeTime > 0)
        {

            //フェードする場合
            bgmFade = true;
            stopFade = true;
            outTime = fadeTime;
            fadelate = fadeTime;
        }
        else
        {

            //フェードしない場合
            bgmAudioSource.Stop();
            bgmAudioSource.clip = null;
        }
    }

    //BGM再生
    public void PlayBgm(string bgmName,float fadeTime, float volume)
    {
        string ResName = "Sounds/BGM/" + bgmName;
        fadeBf = Resources.Load(ResName) as AudioClip;

        //フェードがいる処理
        if (fadeTime > 0)
        {
            bgmFade = true;
            inTime = fadeTime;
            fadelate = fadeTime;
            afterVol = volume;
            if (bgmAudioSource.clip ==null)
            {
                outTime = 0;
                MaxVolume = afterVol;
                bgmAudioSource.clip = fadeBf;
                bgmAudioSource.Play();
            }
            else
            {
                outTime = fadeTime;
            }

        }
        else
        {
            bgmAudioSource.clip = fadeBf;
            bgmAudioSource.volume = volume;
            bgmAudioSource.Play();
        }
    }

    //2DSE再生
    public void PlaySe(string seName, float volume)
    {
        string ResName = "Sounds/SE/" + seName;
        AudioClip Clip = Resources.Load(ResName) as AudioClip;

        seAudioSource.PlayOneShot(Clip);
        seAudioSource.volume = volume;
    }

    //3DSE再生
    public void PlaySe3D(string seName,Vector3  pos, float volume)
    {
        string ResName = "Sounds/SE/" + seName;
        AudioClip Clip = Resources.Load(ResName) as AudioClip;

        int listnum = -1;
        
        //再生完了したオブシェを検索
        for(int i=0;i< clipList3Ds.Length; i++)
        {
            if(clipList3Ds[i].time <= 0)
            {
                listnum = i;
                break;
            }
        }

        //空きがなければオブジェ複製
        if (listnum == -1)
        {
            listnum = clipList3Ds.Length;
            Array.Resize(ref clipList3Ds, clipList3Ds.Length +1);
            clipList3Ds[listnum] = new ClipList3D();
            clipList3Ds[listnum].StateSet(audioParent);
        }
        
        clipList3Ds[listnum].time = Clip.length;
        clipList3Ds[listnum].soundObject.transform.position = pos;
        clipList3Ds[listnum].audioSource.PlayOneShot(Clip);
        clipList3Ds[listnum].audioSource.volume= volume;
    }


    //裏のBGM停止
    public void StopBgmBAG()
    {
        bgmAudioSource2.Stop();
        bgmAudioSource2.clip = null;
        
    }

    //裏のBGM再生
    public void PlayBgmBAG(string bgmName,float volume)
    {
        string ResName = "Sounds/BGM/" + bgmName;
        bgmAudioSource2.clip = Resources.Load(ResName) as AudioClip;
        bgmAudioSource2.volume = volume;
        bgmAudioSource2.Play();
    }

}
