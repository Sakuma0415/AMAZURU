using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultControl : MyAnimation
{
    [SerializeField, Tooltip("項目ボタン")] private Button[] menuButton = null;
    [SerializeField, Tooltip("STAGECLEARオブジェクト")] private GameObject clearObject = null;
    [SerializeField, Tooltip("GAMEOVERオブジェクト")] private GameObject gameOverObject = null;

    [SerializeField, Header("アニメーション実行間隔"), Range(0, 3)] private float span = 1.0f;

    private float time = 0;
    private int step = 0;
    private bool stepEnd = false;

    [SerializeField]private bool clearFlag = false;
    private bool gameOverFlag = false;
    private bool clearAnimeEnd = false;
    private bool gameOverAnimeEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        ResultInit();
    }

    // Update is called once per frame
    void Update()
    {
        ClearAction();
        GameOverAction();
        ObjectReset();

        if(clearAnimeEnd || gameOverAnimeEnd)
        {
            ButtonActive(true);
        }
        else
        {
            ButtonActive(false);
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void ResultInit()
    {
        // ボタンのアクションの設定
        for(int i = 0; i < menuButton.Length; i++)
        {
            int num = i;
            menuButton[i].onClick.AddListener(() => ButtonAction(num));
        }
    }

    /// <summary>
    /// ボタンアクション
    /// </summary>
    /// <param name="num"></param>
    private void ButtonAction(int num)
    {
        if (clearFlag) { clearFlag = false; }
        if (gameOverFlag) { gameOverFlag = false; }

        SceneLoadManager.SceneName name;
        switch (num)
        {
            case 0:
                name = SceneLoadManager.SceneName.Action;
                break;
            case 1:
                name = SceneLoadManager.SceneName.StageSlect;
                break;
            default:
                name = SceneLoadManager.SceneName.Title;
                break;
        }
        SceneLoadManager.Instance.LoadScene(name);
    }

    /// <summary>
    /// ボタンの表示と非表示管理
    /// </summary>
    /// <param name="active"></param>
    private void ButtonActive(bool active)
    {
        foreach (var button in menuButton)
        {
            if (button.gameObject.activeSelf == active) { break; }
            button.gameObject.SetActive(active);
        }
    }

    /// <summary>
    /// StageClear時のアニメーション処理
    /// </summary>
    private void ClearAction()
    {
        if (clearObject == null) { return; }

        if (clearFlag)
        {
            switch (step)
            {
                case 0:
                    stepEnd = ScaleAnimation(clearObject, time, span, Vector3.zero, Vector3.one);
                    time += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = Wait(time, span * 0.5f);
                    time += Time.deltaTime;
                    break;
                case 2:
                    stepEnd = MoveAnimation(clearObject, time, span, Vector3.zero, (Vector3.up * Screen.height * 0.5f) * 0.5f, true);
                    time += Time.deltaTime;
                    break;
                case 3:
                    stepEnd = Wait(time, span * 0.5f);
                    time += Time.deltaTime;
                    break;
                default:
                    clearAnimeEnd = true;
                    return;
            }

            if (stepEnd)
            {
                step++;
                time = 0;
            }
        }
    }

    /// <summary>
    /// GameOver時のアニメーション処理
    /// </summary>
    private void GameOverAction()
    {
        if (gameOverObject == null) { return; }

        if (gameOverFlag)
        {
            switch (step)
            {
                case 0:
                    stepEnd = ScaleAnimation(gameOverObject, time, span, Vector3.zero, Vector3.one);
                    time += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = Wait(time, span * 0.5f);
                    time += Time.deltaTime;
                    break;
                case 2:
                    stepEnd = MoveAnimation(gameOverObject, time, span, Vector3.zero, (Vector3.up * Screen.height * 0.5f) * 0.5f, true);
                    time += Time.deltaTime;
                    break;
                case 3:
                    stepEnd = Wait(time, span * 0.5f);
                    time += Time.deltaTime;
                    break;
                default:
                    gameOverAnimeEnd = true;
                    return;
            }

            if (stepEnd)
            {
                step++;
                time = 0;
            }
        }
    }

    /// <summary>
    /// フラグがOFFならオブジェクトをリセット
    /// </summary>
    private void ObjectReset()
    {
        if (clearFlag == false && gameOverFlag == false)
        {
            step = 0;
            time = 0;
        }

        if(clearFlag == false)
        {
            clearObject.transform.localPosition = Vector3.zero;
            clearObject.transform.localScale = Vector3.zero;
            clearAnimeEnd = false;
        }

        if(gameOverFlag == false)
        {
            gameOverObject.transform.localPosition = Vector3.zero;
            gameOverObject.transform.localScale = Vector3.zero;
            gameOverAnimeEnd = false;
        }
    }

    /// <summary>
    /// リザルトを再生する
    /// </summary>
    /// <param name="clear">CLEARならtrue、GAMEOVERならfalse</param>
    public void StartResult(bool clear)
    {
        if (clear)
        {
            if (gameOverFlag) { gameOverFlag = false; }
            clearFlag = true;
        }
        else
        {
            if (clearFlag) { clearFlag = false; }
            gameOverFlag = true;
        }
    }
}
