using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class SoundManager : MonoBehaviour
{
    static public SoundManager soundManager;

    [SerializeField]
    AudioSource seAudioSource;
    [SerializeField]
    AudioSource bgmAudioSource;


    [SerializeField]
    Transform audioParent;
    
    bool bgmFade = false;
    AudioClip fadeBf;
    float inTime = 0;
    float outTime = 0;
    float fadelate = 0;

    [SerializeField]
    float MaxVolume = 0;

    struct ClipList3D
    {
        public float time;
        public GameObject soundObject;
        public AudioSource audioSource;
        public void StateSet(Transform parent)
        {
            time = 0;
            soundObject = new GameObject();
            soundObject.transform.parent = parent;
            soundObject.AddComponent<AudioSource>();
            soundObject.name = "3DAudioSource";
            audioSource=soundObject.GetComponent<AudioSource>();
            audioSource.spatialBlend = 1;
        }
    }

    ClipList3D[] clipList3Ds=new ClipList3D[1];

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
    
    void Start()
    {
        
    }

    bool fas = false;

    void Update()
    {
        if (bgmFade)
        {
            if(outTime > 0)
            {
                outTime -= Time.deltaTime;
                
                if(outTime <= 0)
                {
                    outTime = 0;
                    bgmAudioSource.clip = fadeBf;
                    bgmAudioSource.Play();
                }
                bgmAudioSource.volume = (outTime / fadelate)* MaxVolume;
            }
            else
            {
                inTime -= Time.deltaTime;
                
                if (inTime <= 0)
                {
                    inTime = 0;
                    bgmFade = false;
                }
                bgmAudioSource.volume = MaxVolume - ((outTime / fadelate) * MaxVolume);
            }
        }

        for(int i=0;i< clipList3Ds.Length; i++)
        {
            clipList3Ds[i].time -= Time.deltaTime;
        }
    }

    public void StopBgm()
    {
        bgmAudioSource.Stop();
    }

    public void PlayBgm(string bgmName,float fadeTime)
    {
        string ResName = "Sounds/BGM/" + bgmName;
        fadeBf = Resources.Load(ResName) as AudioClip;
        if (fadeTime > 0)
        {
            bgmFade = true;
            inTime = fadeTime;
            outTime = fadeTime;
            fadelate = fadeTime;
        }
        else
        {
            bgmAudioSource.clip = fadeBf;
            bgmAudioSource.Play();
        }
    }

    public void PlaySe(string seName)
    {
        string ResName = "Sounds/SE/" + seName;
        AudioClip Clip = Resources.Load(ResName) as AudioClip;

        seAudioSource.PlayOneShot(Clip);
    }

    public void PlaySe3D(string seName,Vector3  pos)
    {
        string ResName = "Sounds/SE/" + seName;
        AudioClip Clip = Resources.Load(ResName) as AudioClip;

        int listnum = -1;

        Debug.Log(clipList3Ds.Length );

        for(int i=0;i< clipList3Ds.Length; i++)
        {
            if(clipList3Ds[i].time <= 0)
            {
                listnum = i;
                break;
            }
        }

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
    }
}
