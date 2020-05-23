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
    AudioSource[] bgmAudioSource;
    //3Dサウンドの再生地点を記憶するオブジェの親
    [SerializeField]
    Transform audioParent;
    
    //private
    struct BGMList
    {
        //BGMのボリューム
        public float MaxVolume;
        //BGMがフェード中かどうかのフラグ
        public bool bgmFade;
        //フェード後に再生するBGMの退避先
        public AudioClip fadeBf;
        //フェードインの時間
        public float inTime;
        //フェードアウトの時間
        public float outTime;
        //フェードインアウトの全体時間
        public float fadelate;
        //フェードアウト後音量を格納しておく場所
        public float afterVol;
        //BGM停止時にフェードする場合のフラグ
        public bool stopFade;

        public bool volBgmFade;
        public float startVol;
        public float endVol;
        public float volFadeTime;
        public float volFadeSpan;

        public void Init()
        {
            MaxVolume = 0;
            bgmFade = false;
            inTime = 0;
            outTime = 0;
            fadelate = 0;
            afterVol = 0;
            stopFade = false;
            volBgmFade=false;
            startVol=0;
            endVol=0;
            volFadeTime=0;
            volFadeSpan=0;
        }
    }

    private BGMList[] BGMLists=new BGMList[2];
    public bool BGMnull1=false , BGMnull2 = false;

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
            for(int i=0;i< BGMLists.Length; i++)
            {
                BGMLists[i].Init();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //フレーム処理
    void Update()
    {

        BGMnull1= !(bgmAudioSource[0].clip == null);
        BGMnull2 = !(bgmAudioSource[1].clip == null);

        //BGMがフェード処理中
        for (int i = 0; i < BGMLists.Length; i++)
        {
            if (BGMLists[i].bgmFade)
            {

                if (BGMLists[i].outTime > 0)
                {

                    //フェードアウトの処理
                    BGMLists[i].outTime -= Time.deltaTime;
                    bgmAudioSource[i].volume = (BGMLists[i].outTime / BGMLists[i].fadelate) * BGMLists[i].MaxVolume;

                    if (BGMLists[i].outTime <= 0)
                    {

                        bgmAudioSource[i].volume = 0;

                        if (BGMLists[i].stopFade)
                        {
                            BGMLists[i].bgmFade = false;
                            BGMLists[i].stopFade = false;
                            bgmAudioSource[i].Stop();
                            bgmAudioSource[i].clip = null;
                        }
                        else
                        {
                            BGMLists[i].outTime = 0;
                            bgmAudioSource[i].clip = BGMLists[i].fadeBf;
                            BGMLists[i].MaxVolume = BGMLists[i].afterVol;
                            bgmAudioSource[i].Play();
                        }
                    }
                }
                else
                {
                    //フェードインの処理
                    BGMLists[i].inTime -= Time.deltaTime;
                    bgmAudioSource[i].volume = BGMLists[i].MaxVolume - ((BGMLists[i].inTime / BGMLists[i].fadelate) * BGMLists[i].MaxVolume);

                    if (BGMLists[i].inTime <= 0)
                    {
                        bgmAudioSource[i].volume = BGMLists[i].MaxVolume;
                        BGMLists[i].inTime = 0;
                        BGMLists[i].bgmFade = false;
                    }
                }
            }

            if (BGMLists[i].volBgmFade)
            {
                BGMLists[i].volFadeTime += Time.deltaTime;
                bgmAudioSource[i].volume =Mathf.Lerp (BGMLists[i].startVol , BGMLists[i].endVol,(BGMLists[i].volFadeTime / BGMLists[i].volFadeSpan));
                if (BGMLists[i].volFadeTime> BGMLists[i].volFadeSpan)
                {
                    bgmAudioSource[i].volume = BGMLists[i].endVol;
                    BGMLists[i].MaxVolume = BGMLists[i].endVol;
                    BGMLists[i].volBgmFade = false;
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
    public void StopBgm(float fadeTime,int hierarchy)
    {
        if (fadeTime > 0)
        {

            //フェードする場合
            BGMLists[hierarchy].bgmFade = true;
            BGMLists[hierarchy].stopFade = true;
            BGMLists[hierarchy].outTime = fadeTime;
            BGMLists[hierarchy].fadelate = fadeTime;
        }
        else
        {

            //フェードしない場合
            bgmAudioSource[hierarchy].Stop();
            bgmAudioSource[hierarchy].clip = null;
        }
    }

    //BGM再生
    public void PlayBgm(string bgmName,float fadeTime, float volume, int hierarchy)
    {
        string ResName = "Sounds/BGM/" + bgmName;
        BGMLists[hierarchy].fadeBf = Resources.Load(ResName) as AudioClip;

        //フェードがいる処理
        if (fadeTime > 0)
        {
            BGMLists[hierarchy].bgmFade = true;
            BGMLists[hierarchy].inTime = fadeTime;
            BGMLists[hierarchy].fadelate = fadeTime;
            BGMLists[hierarchy].afterVol = volume;
            if (bgmAudioSource[hierarchy].clip ==null)
            {
                BGMLists[hierarchy].outTime = 0;
                BGMLists[hierarchy].MaxVolume = BGMLists[hierarchy].afterVol;
                bgmAudioSource[hierarchy].clip = BGMLists[hierarchy].fadeBf;
                bgmAudioSource[hierarchy].Play();
            }
            else
            {
                BGMLists[hierarchy].outTime = fadeTime;
            }

        }
        else
        {
            bgmAudioSource[hierarchy].clip = BGMLists[hierarchy].fadeBf;
            bgmAudioSource[hierarchy].volume = volume;
            bgmAudioSource[hierarchy].Play();
        }
    }

    //BGMのフェード
    public void VolFadeBgm(float fadeTime, float volume, int hierarchy)
    {
        BGMLists[hierarchy].volBgmFade =true;
        BGMLists[hierarchy].startVol= BGMLists[hierarchy].MaxVolume;
        BGMLists[hierarchy].endVol=volume ;
        BGMLists[hierarchy].volFadeTime=0;
        BGMLists[hierarchy].volFadeSpan=fadeTime;
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


    ////裏のBGM停止
    //public void StopBgmBAG()
    //{
    //    bgmAudioSource2.Stop();
    //    bgmAudioSource2.clip = null;
        
    //}

    ////裏のBGM再生
    //public void PlayBgmBAG(string bgmName,float volume)
    //{
    //    string ResName = "Sounds/BGM/" + bgmName;
    //    bgmAudioSource2.clip = Resources.Load(ResName) as AudioClip;
    //    bgmAudioSource2.volume = volume;
    //    bgmAudioSource2.Play();
    //}

}
