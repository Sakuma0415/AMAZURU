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

    public SceneName sceneName;

    public Canvas fadeCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("SceneLoad");
        }
    }

    /// <summary>
    /// シーンの非同期読み込み
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    public IEnumerator SceneLoad()
    {
        StartCoroutine(Fade(1, 0));

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

        StartCoroutine(Fade(1, 1));
        yield return null;
    }

    private IEnumerator Fade(float sec, int fadeIndex = 0)
    {
        GameObject obj = fadeCanvas.transform.GetChild(0).gameObject;
        Color c = obj.GetComponent<Image>().color;
        float speed = 1 / (sec * 60);
        float a = c.a;
        bool fadeEnd = false;

        while (!fadeEnd)
        {
            if (fadeIndex == 0)
            {
                obj.GetComponent<Image>().color = new Color(c.r, c.g, c.b, a);
                a += speed;
                if(a >= 1) { fadeEnd = true; }
            }
            else
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
