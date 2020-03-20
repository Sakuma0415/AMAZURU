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
        StageEdit
    }

    private SceneName sceneName;

    public enum FadeMode
    {
        IN = 0,
        OUT
    }

    public Canvas fadeCanvas;

    void Update()
    {
    }

    /// <summary>
    /// シーンの非同期読込
    /// </summary>
    /// <param name="name">シーンの名前(列挙型)</param>
    public void LoadScene(SceneName name)
    {
        sceneName = name;
        StartCoroutine("Load");
    }

    /// <summary>
    /// シーンの非同期読み込み
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    private IEnumerator Load()
    {
        StartCoroutine(Fade(1, FadeMode.OUT));
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

        StartCoroutine(Fade(1, FadeMode.IN));
        yield return null;
    }

    /// <summary>
    /// フェードインアウト
    /// <para>第一引数 = 時間</para>
    /// <para>第二引数 = フェードモード</para>
    /// </summary>
    /// <param name="sec"></param>
    /// <param name="fadeIndex"></param>
    /// <returns></returns>
    private IEnumerator Fade(float sec, FadeMode fadeMode)
    {
        GameObject obj = fadeCanvas.transform.GetChild(0).gameObject;
        Color c = obj.GetComponent<Image>().color;
        float speed = 1 / (sec * 60);
        float a = c.a;
        bool fadeEnd = false;

        while (!fadeEnd)
        {
            if (fadeMode == FadeMode.OUT)
            {
                obj.GetComponent<Image>().color = new Color(c.r, c.g, c.b, a);
                a += speed;
                if(a >= 1) { fadeEnd = true; }
            }
            else if(fadeMode == FadeMode.IN)
            {
                obj.GetComponent<Image>().color = new Color(c.r, c.g, c.b, a);
                a -= speed;
                if (a <= 0) { fadeEnd = true; }
            }

            yield return null;
        }
        yield return null;
    }
}
