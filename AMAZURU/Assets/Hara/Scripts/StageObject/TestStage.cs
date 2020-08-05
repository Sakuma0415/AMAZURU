#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStage : MonoBehaviour
{
    [SerializeField] StageData testStageData = null;
    // Update is called once per frame
    void Update()
    {
        if(testStageData == null) { return; }

        if (Input.GetKeyDown(KeyCode.T) && SceneLoadManager.Instance.SceneLoadFlg == false)
        {
            StageMake.LoadStageData = testStageData;
            SoundManager.soundManager.StopBgm(0.5f, 1);
            SoundManager.soundManager.StopBgm(0.5f, 0);
            SoundManager.soundManager.PlaySe("btn01", 0.3f);
            SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
        }
    }
}
#endif