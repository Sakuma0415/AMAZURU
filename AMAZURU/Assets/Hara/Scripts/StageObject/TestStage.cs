#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStage : MonoBehaviour
{
    [SerializeField, Header("テスト用のステージデータ")] StageData[] testStageData = null;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.T) && SceneLoadManager.Instance.SceneLoadFlg == false)
        {
            if(Input.GetKeyDown(KeyCode.Alpha0) || Input.GetKeyDown(KeyCode.Keypad0))
            {
                LoadTestStage(0);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                LoadTestStage(1);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                LoadTestStage(2);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                LoadTestStage(3);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                LoadTestStage(4);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                LoadTestStage(5);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                LoadTestStage(6);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                LoadTestStage(7);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
            {
                LoadTestStage(8);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha9) || Input.GetKeyDown(KeyCode.Keypad9))
            {
                LoadTestStage(9);
            }
            else
            {
                Debug.Log("0～9のいずれかのキーを押してテストステージが実行できます");
            }
        }
    }

    /// <summary>
    /// テストステージの読み込み
    /// </summary>
    /// <param name="index">ステージID</param>
    private void LoadTestStage(int index)
    {
        if(index >= testStageData.Length || testStageData[index] == null)
        {
            Debug.LogError(index + "番にステージが登録されていません！");
            return;
        }

        StageMake.LoadStageData = testStageData[index];
        SoundManager.soundManager.StopBgm(0.5f, 1);
        SoundManager.soundManager.StopBgm(0.5f, 0);
        SoundManager.soundManager.PlaySe("btn01", 0.3f);
        SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
    }
}
#endif