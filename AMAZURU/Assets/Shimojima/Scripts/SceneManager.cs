using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : SingletonMonoBehaviour<SceneManager>
{
    [System.Serializable]
    public enum SceneName
    {
        aaa = 0
    }

    public SceneName sceneName;

    void Update()
    {
        
    }

    /// <summary>
    /// Sceneのロード
    /// </summary>
    public void SceneLoad()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName.ToString());
    }
}
