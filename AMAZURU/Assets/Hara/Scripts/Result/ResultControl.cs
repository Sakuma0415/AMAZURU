using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultControl : MyAnimation
{
    [SerializeField, Tooltip("項目ボタン")] private Button[] menuButton = null;
    [SerializeField, Tooltip("テキストオブジェクト")] private Text textObject = null;

    [SerializeField, Header("アニメーション実行間隔"), Range(0, 3)] private float span = 1.0f;

    private float time = 0;
    private int step = 0;
    private Coroutine coroutine = null;
    private bool selectMenu = false;
    private int selectButtonNum = 0;

    // Start is called before the first frame update
    void Start()
    {
        ResultInit();
    }

    private void Update()
    {
        MenuButtonAction();
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

        // オブジェクトを非表示
        HiddenObject();
    }

    /// <summary>
    /// オブジェクトを非表示にする
    /// </summary>
    private void HiddenObject()
    {
        if(textObject != null) { textObject.gameObject.SetActive(false); }
        ButtonActive(false);
    }

    /// <summary>
    /// ボタンを押したときのアクション
    /// </summary>
    /// <param name="num"></param>
    private void ButtonAction(int num)
    {
        SoundManager.soundManager.StopBgm(0.5f);
        HiddenObject();

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
            button.gameObject.SetActive(active);
        }

        if (active)
        {
            time = 0;
            selectButtonNum = 0;
            selectMenu = true;
        }
        else
        {
            selectMenu = false;
        }
    }

    /// <summary>
    /// リザルトのアニメーションコルーチン
    /// </summary>
    /// <param name="resultFlag"></param>
    /// <returns></returns>
    private IEnumerator ResultAction(bool resultFlag)
    {
        if (textObject == null) { yield break; }

        if (resultFlag)
        {
            textObject.text = "STAGE CLEAR";
            textObject.color = new Color(255f / 255f, 179f / 255f, 0f / 255f);
        }
        else
        {
            textObject.text = "GAME OVER";
            textObject.color = Color.red;
        }

        textObject.transform.localPosition = Vector3.zero;
        textObject.gameObject.SetActive(true);

        time = 0;
        while(ScaleAnimation(textObject.gameObject, time, span, Vector3.zero, Vector3.one) == false)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (Wait(time, span * 0.5f) == false)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (MoveAnimation(textObject.gameObject, time, span, Vector3.zero, Vector3.up * Screen.height * 0.25f, true) == false)
        {
            time += Time.deltaTime;
            yield return null;
        }

        time = 0;
        while (Wait(time, span * 0.5f) == false)
        {
            time += Time.deltaTime;
            yield return null;
        }

        ButtonActive(true);
        coroutine = null;
    }

    /// <summary>
    /// ポーズ画面のアクション
    /// </summary>
    private void PauseAction()
    {
        if(textObject == null) { return; }

        textObject.text = "PAUSE";
        textObject.color = Color.black;
        textObject.transform.localPosition = Vector3.up * Screen.height * 0.25f;

        textObject.gameObject.SetActive(true);
        ButtonActive(true);
    }

    /// <summary>
    /// メニューボタンの処理
    /// </summary>
    private void MenuButtonAction()
    {
        if(selectMenu == false) { return; }

        bool end;
        float span = 1.0f;
        float input = Input.GetAxis("Vertical");
        int key = input > 0.1f ? -1 : input < -0.1f ? 1 : 0;
        // ボタンを選択状態にする
        menuButton[selectButtonNum].Select();

        // 選択状態のボタンのアニメーションを実行する
        switch (step)
        {
            case 0:
                end = ScaleAnimation(menuButton[selectButtonNum].gameObject, time, span / 4, Vector3.one, Vector3.one * 1.05f);
                time += Time.deltaTime;
                break;
            case 1:
                end = ScaleAnimation(menuButton[selectButtonNum].gameObject, time, span / 4, Vector3.one * 1.05f, Vector3.one);
                time += Time.deltaTime;
                break;
            case 2:
                end = ScaleAnimation(menuButton[selectButtonNum].gameObject, time, span / 4, Vector3.one, Vector3.one * 0.95f);
                time += Time.deltaTime;
                break;
            case 3:
                end = ScaleAnimation(menuButton[selectButtonNum].gameObject, time, span / 4, Vector3.one * 0.95f, Vector3.one);
                time += Time.deltaTime;
                break;
            default:
                step = 0;
                return;
        }

        if (end)
        {
            step++;
            time = 0;
        }

        if (key != 0)
        {
            menuButton[selectButtonNum].transform.localScale = Vector3.one;
            selectButtonNum += key;
            selectButtonNum = selectButtonNum >= menuButton.Length ? 0 : selectButtonNum < 0 ? menuButton.Length - 1 : selectButtonNum;
            step = 0;
            time = 0;
        }
    }

    /// <summary>
    /// ポーズ画面の表示・非表示
    /// </summary>
    /// <param name="active">trueなら表示、falseなら非表示</param>
    public void GamePause(bool active)
    {
        if (active)
        {
            PauseAction();
        }
        else
        {
            HiddenObject();
        }
    }

    /// <summary>
    /// リザルトを再生する
    /// </summary>
    /// <param name="clear">CLEARならtrue、GAMEOVERならfalse</param>
    public void StartResult(bool clear)
    {
        if(coroutine == null)
        {
            if (clear)
            {
                coroutine = StartCoroutine(ResultAction(true));
            }
            else
            {
                coroutine = StartCoroutine(ResultAction(false));
            }
        }
    }
}
