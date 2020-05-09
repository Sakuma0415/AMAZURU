using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField, Tooltip("タイトル用のアニメーションスクリプト")] private TitleAnimation titleAnime = null;

    private void Start()
    {
        SoundManager.soundManager.PlayBgm("MusMus-BGM-043", 0.1f, 0.8f);
        SoundManager.soundManager.PlayBgmBAG("rain_loop", 0.5f);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Circle"))
        {
            SoundManager.soundManager.PlaySe("btn01", 0.5f);
            titleAnime.AnimationFlag = true;
            SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName .StageSlect ,false );
            SoundManager.soundManager.StopBgm(1);
        }
    }
}
