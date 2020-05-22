using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{
    [SerializeField, Tooltip("タイトル用のアニメーションスクリプト")] private TitleAnimation titleAnime = null;

    float changeTime = 0;
    bool changeFlg = false;
    private void Start()
    {
        SoundManager.soundManager.PlayBgm("MusMus-BGM-043", 0.1f, 0.8f,0);
        SoundManager.soundManager.PlayBgm("rain_loop",0.1f, 0.5f,1);

        changeTime = 0;
        changeFlg = false;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Circle")&& !changeFlg)
        {
            changeFlg = true;
            SoundManager.soundManager.PlaySe("btn01", 0.5f);
        }

        if (changeFlg)
        {
            changeTime += Time.deltaTime;
            if (changeTime > 0.5f && !SceneLoadManager.Instance.SceneLoadFlg)
            {
                titleAnime.AnimationFlag = true;
                SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.StageSlect, false);
                SoundManager.soundManager.StopBgm(1, 0);
            }
        }




    }
}
