using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Scenemanager : SingletonMonoBehaviour<Scenemanager>
{
    [System.Serializable]
    public enum SceneName
    {
        Title = 0,
        Action
    }

    private SceneName sceneName;

    public enum FadeMode
    {
        IN = 0,
        OUT
    }

    public Image fadeImage;

    void Update()
    {
    }

    /// <summary>
    /// シーンの非同期読込
    /// </summary>
    /// <param name="name">シーンの名前(列挙型)</param>
    /// <param name="fm1">最初に行うフェード操作</param>
    /// <param name="fm2">最後に行うフェード操作</param>
    public void LoadScene(SceneName name, FadeMode fm1, FadeMode fm2)
    {
        sceneName = name;
        StartCoroutine(Load(fm1, fm2));
    }

    /// <summary>
    /// シーンの非同期読み込み
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator Load(FadeMode fm1, FadeMode fm2)
    {
        StartCoroutine(Fade(1, fm1));
        yield return new WaitForSeconds(1f);
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName.ToString());
        async.allowSceneActivation = false;

        while (!async.isDone)
        {
            if (async.progress >= 0.9f) { Debug.Log("compleated"); }
            if (Input.GetKeyDown(KeyCode.A))
            {
                async.allowSceneActivation = true;
            }

            yield return null;
        }

        StartCoroutine(Fade(1, fm2));
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
        float speed = 1 / (sec * 60);
        float a = fadeImage.material.GetFloat("_Alpha");
        bool fadeEnd = false;

        while (!fadeEnd)
        {
            if (fadeMode == FadeMode.IN)
            {
                fadeImage.material.SetFloat("_Alpha", a);
                a -= speed;
                if(a <= 0) { fadeEnd = true; }
            }
            else if(fadeMode == FadeMode.OUT)
            {
                fadeImage.material.SetFloat("_Alpha", a);
                a += speed;
                if (a >= 1) { fadeEnd = true; }
            }

            yield return null;
        }
        yield return null;
    }
}
