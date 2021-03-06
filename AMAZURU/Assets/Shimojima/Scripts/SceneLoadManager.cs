﻿using System.Collections;
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
        StageSelect_v2
    }

    private SceneName sceneName;

    public enum FadeMode
    {
        IN = 0,
        OUT
    }

    private bool IsLoadScene = false;
    public GameObject[] loadingImages;
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
            Debug.Log(ex.ToString());
            return;
        }
    }
#endif

    private void Start()
    {
        //Warningつぶし
        if (!shader) { shader = Shader.Find("Custom/InObj"); }
        if(color == null) { color = new Color(); }
        if (!anounceText) { anounceText = new GameObject(); }

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
        loadingImages[0].SetActive(DoKeyPress);
        Animator animator = anounceText.GetComponent<Animator>();
        async.allowSceneActivation = false;
        bool DoOnce = false;
        bool cha = false;
        while (!async.isDone)
        {
            if (DoKeyPress)
            {
                if (async.progress >= 0.9f && !DoOnce && fadeEnd) { DoOnce = true; }
                if (ControllerInput.Instance.buttonDown.circle && fadeEnd&& !cha)
                {
                    async.allowSceneActivation = true;
                    loadingImages[0].GetComponent<LoadImage>().WaveReSet();
                    loadingImages[0].SetActive(false);
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
        StartCoroutine(Fade(1, FadeMode.IN));
        yield return new WaitForSeconds(1f);
        loadingImages[1].GetComponent<Animator>().SetTrigger("Start");
        yield return new WaitForSeconds(1.5f);
        IsLoadScene = false;
        SceneLoadFlg = false;
        loadingImages[1].GetComponent<Animator>().SetTrigger("End");
        yield return null;
        loadingImages[1].SetActive(false);
        Color c = loadingImages[1].transform.GetChild(1).GetComponent<Image>().color;
        loadingImages[1].transform.GetChild(1).GetComponent<Image>().color = new Color(c.r, c.g, c.b, 1);
        loadingImages[1].transform.GetChild(0).GetComponent<LoadEnd>().Up = 1;
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
                if (alphaCut <= 0) { fadeEnd = true; }
            }
            else if (fadeMode == FadeMode.OUT)
            {
                alphaCut += Time.deltaTime / sec; ;
                fadeImage.material.SetFloat("_Alpha", alphaCut);
                if (alphaCut >= 1) { fadeEnd = true; }
            }

            yield return null;
        }
        //if (fadeMode == FadeMode.IN) { SceneLoadFlg = false; }

        if (!loadingImages[1].activeSelf){ loadingImages[1].SetActive(true); }

        yield return null;
    }
}