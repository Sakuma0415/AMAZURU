using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoadManager : SingletonMonoBehaviour<SceneLoadManager>
{
    [System.Serializable]
    public enum SceneName
    {
        Title = 0,
        Action,
        StageSlect
    }

    private SceneName sceneName;

    public enum FadeMode
    {
        IN = 0,
        OUT
    }

    private bool IsLoadScene = false;
    public GameObject loadImage;
    public Image fadeImage;
    [SerializeField]
    private Shader shader;
    [SerializeField]
    private Color color;
    [SerializeField, Tooltip("アルファ値のカット値"),Range(0,1)]
    private float alphaCut = 0;
    [SerializeField]
    private GameObject anounceText;
    private bool fadeEnd, DoKeyPress;
    public bool SceneLoadFlg = false;
#if UNITY_EDITOR
    void OnValidate()
    {
        try
        {
            fadeImage.material.SetFloat("_Alpha", alphaCut);
        }
        catch(NullReferenceException ex)
        {
            return;
        }
    }
#endif

    private void Start()
    {

        fadeImage.material = new Material(shader);
        fadeImage.material.SetFloat("_Alpha", alphaCut);
        fadeImage.material.SetColor("_Color", color);

    }

    void Update()
    {
    }

    /// <summary>
    /// シーンの非同期読込
    /// </summary>
    /// <param name="name">シーンの名前(列挙型)</param>
    /// <param name="fm1">最初に行うフェード操作</param>
    public void LoadScene(SceneName name, bool keyPress = true)
    {
        if (IsLoadScene) { return; }
        sceneName = name;
        DoKeyPress = keyPress;
        StartCoroutine("Load");
        IsLoadScene = true;
        SceneLoadFlg = true;
    }

    /// <summary>
    /// シーンの非同期読み込み
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator Load()
    {
        StartCoroutine(Fade(1, FadeMode.OUT));
        yield return new WaitForSeconds(1);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName.ToString());
        loadImage.SetActive(DoKeyPress);
        Animator animator = anounceText.GetComponent<Animator>();
        async.allowSceneActivation = false;
        bool DoOnce = false;
        bool cha = false;
        while (!async.isDone)
        {
            if (DoKeyPress)
            {
                if (async.progress >= 0.9f && !DoOnce && fadeEnd) { DoOnce = true; Debug.Log("compleated"); animator.SetTrigger("FadeIn"); }
                if (Input.GetButtonDown("Circle") && fadeEnd&& !cha)
                {
                    async.allowSceneActivation = true;
                    loadImage.GetComponent<LoadImage>().WaveReSet();
                    loadImage.SetActive(false);
                    animator.ResetTrigger("FadeIn");
                    animator.SetTrigger("FadeOut");
                    cha = true;
                }
            }
            else
            {
                if (async.progress >= 0.9f && fadeEnd)
                {
                    async.allowSceneActivation = true;
                }
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        StartCoroutine(Fade(1, FadeMode.IN));
        IsLoadScene = false;
        yield return null;
    }

    /// <summary>
    /// フェードインアウト
    /// <para>第一引数 = 時間</para>
    /// <para>第二引数 = フェードモード</para>
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="fadwMode"></param>
    /// <returns></returns>
    private IEnumerator Fade(float sec, FadeMode fadeMode)
    {
        alphaCut = fadeImage.material.GetFloat("_Alpha");
        fadeEnd = false;

        while (!fadeEnd)
        {
            if (fadeMode == FadeMode.IN)
            {
                alphaCut -= Time.deltaTime / sec; ;
                fadeImage.material.SetFloat("_Alpha", alphaCut);
                if(alphaCut <= 0) { fadeEnd = true; }
            }
            else if(fadeMode == FadeMode.OUT)
            {
                alphaCut += Time.deltaTime / sec; ;
                fadeImage.material.SetFloat("_Alpha", alphaCut);
                if (alphaCut >= 1) { fadeEnd = true; }
            }

            yield return null;
        }
        if (fadeMode == FadeMode.IN) { SceneLoadFlg = false; }
        yield return null;
    }
}