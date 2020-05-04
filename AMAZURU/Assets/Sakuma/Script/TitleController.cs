using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField, Tooltip("タイトル用のアニメーションスクリプト")] private TitleAnimation titleAnime = null;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Circle"))
        {
            titleAnime.AnimationFlag = true;
            SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName .StageSlect );
        }
    }
}
