using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CameraSpeed
{
    Slow,
    Nomal,
    Quick
}

public class ResultControl : MyAnimation
{
    [SerializeField, Tooltip("項目ボタン")] private GameObject[] menuButton = null;
    [SerializeField, Tooltip("カメラ感度設定ボタン")] private GameObject[] cameraOptionButton = null;
    [SerializeField, Tooltip("テキストオブジェクト")] private Text textObject = null;
    [SerializeField, Tooltip("カメラ感度設定ウィンドウ")] private GameObject cameraOptionWindow = null;
    [SerializeField, Tooltip("カメラ感度の現在設定")] private Text nowSettingText = null;

    [SerializeField, Header("アニメーション実行間隔"), Range(0, 3)] private float span = 1.0f;

    private float time = 0;
    private float selectChangeTime = 0;
    private bool keyDown = false;
    private int step = 0;
    private Coroutine coroutine = null;
    private bool selectMenu = false;
    private int selectButtonNum = 0;
    private GameObject[] animationObject = null;

    private int minButtonLength = 0;
    private bool actionFlag = false;

    /// <summary>
    /// カメラの速さの設定を取得
    /// </summary>
    public CameraSpeed SpeedType { set; get; } = CameraSpeed.Nomal;

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
        // オブジェクトを非表示
        HiddenObject(false);
        Option(false);
    }

    /// <summary>
    /// オブジェクトを非表示にする
    /// </summary>
    private void HiddenObject(bool hiddenOnlyButton)
    {
        if(textObject != null && hiddenOnlyButton == false) { textObject.gameObject.SetActive(false); }
        if(hiddenOnlyButton == false) { StopButtonAnimation(); }
        ButtonActive(false);
    }

    /// <summary>
    /// ボタンを押したときのアクション
    /// </summary>
    /// <param name="num"></param>
    private void ButtonAction(int num)
    {
        SoundManager.soundManager.StopBgm(0.5f);
        bool hiddenFlag;

        switch (num)
        {
            case 0:
                Option(true);
                hiddenFlag = true;
                break;
            case 1:
                SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
                hiddenFlag = false;
                break;
            case 2:
                SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.StageSlect);
                hiddenFlag = false;
                break;
            default:
                SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Title);
                hiddenFlag = false;
                break;
        }
        HiddenObject(hiddenFlag);
    }

    /// <summary>
    /// ボタンの表示と非表示管理
    /// </summary>
    /// <param name="active"></param>
    private void ButtonActive(bool active, bool optionWindow = false)
    {
        int index;
        if (active)
        {
            if (optionWindow)
            {
                index = 0;
            }
            else
            {
                index = 1;
            }
        }
        else
        {
            index = 0;
        }

        for(int i = index; i < menuButton.Length; i++)
        {
            if (active)
            {
                if (optionWindow)
                {
                    menuButton[i].transform.localPosition = new Vector3(0, 180 + (-120 * i), 0);
                }
                else
                {
                    menuButton[i].transform.localPosition = new Vector3(0, 140 + (-140 * (i - 1)), 0);
                }
            }
            menuButton[i].SetActive(active);
        }

        if (active)
        {
            StartButtonAnimation(menuButton, index);
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
        ButtonActive(true, true);
    }

    /// <summary>
    /// メニューボタンの処理
    /// </summary>
    private void MenuButtonAction()
    {
        if(selectMenu == false) { return; }

        bool end;
        float span = 1.0f;
        float input = Input.GetAxis("Vertical3");
        int key = input > 0.1f ? -1 : input < -0.1f ? 1 : 0;

        // 選択状態のボタンのアニメーションを実行する
        switch (step)
        {
            case 0:
                end = ScaleAnimation(animationObject[selectButtonNum], time, span / 4, Vector3.one, Vector3.one * 1.05f);
                time += Time.deltaTime;
                break;
            case 1:
                end = ScaleAnimation(animationObject[selectButtonNum], time, span / 4, Vector3.one * 1.05f, Vector3.one);
                time += Time.deltaTime;
                break;
            case 2:
                end = ScaleAnimation(animationObject[selectButtonNum], time, span / 4, Vector3.one, Vector3.one * 0.95f);
                time += Time.deltaTime;
                break;
            case 3:
                end = ScaleAnimation(animationObject[selectButtonNum], time, span / 4, Vector3.one * 0.95f, Vector3.one);
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

        // 選択カーソルの移動処理
        if (key != 0 && keyDown == false)
        {
            keyDown = true;
            animationObject[selectButtonNum].transform.localScale = Vector3.one;
            selectButtonNum += key;
            selectButtonNum = selectButtonNum >= animationObject.Length ? minButtonLength : selectButtonNum < minButtonLength ? animationObject.Length - 1 : selectButtonNum;
            step = 0;
            time = 0;
        }
        else if(key != 0 && keyDown)
        {
            if (selectChangeTime > 0.3f)
            {
                animationObject[selectButtonNum].transform.localScale = Vector3.one;
                selectButtonNum += key;
                selectButtonNum = selectButtonNum >= animationObject.Length ? minButtonLength : selectButtonNum < minButtonLength ? animationObject.Length - 1 : selectButtonNum;
                step = 0;
                time = 0;
                selectChangeTime = 0;
            }
            else
            {
                selectChangeTime += Time.deltaTime;
            }
        }
        else
        {
            selectChangeTime = 0;
            keyDown = false;
        }

        // キー入力処理
        if (Input.GetButtonDown("Circle"))
        {
            if (actionFlag)
            {
                CameraOptionAction(selectButtonNum);
            }
            else
            {
                if (!SceneLoadManager.Instance.SceneLoadFlg)
                {
                    ButtonAction(selectButtonNum);
                }
            }
        }

        if(Input.GetButtonDown("Cross") && actionFlag)
        {
            PauseAction();
            Option(false);
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
            HiddenObject(false);
            Option(false);
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

    /// <summary>
    /// ゲーム設定画面を表示・非表示
    /// </summary>
    private void Option(bool open)
    {
        if (open)
        {
            if (SpeedType == CameraSpeed.Quick)
            {
                nowSettingText.text = "はやい";
            }
            else if (SpeedType == CameraSpeed.Slow)
            {
                nowSettingText.text = "ゆっくり";
            }
            else
            {
                nowSettingText.text = "ふつう";
            }
            StartButtonAnimation(cameraOptionButton, 0, true);
        }
        cameraOptionWindow.SetActive(open);
    }

    /// <summary>
    /// カメラオプションのボタンアクション
    /// </summary>
    /// <param name="num"></param>
    private void CameraOptionAction(int num)
    {
        switch (num)
        {
            case 0:
                nowSettingText.text = "はやい";
                SpeedType = CameraSpeed.Quick;
                break;
            case 1:
                nowSettingText.text = "ふつう";
                SpeedType = CameraSpeed.Nomal;
                break;
            default:
                nowSettingText.text = "ゆっくり";
                SpeedType = CameraSpeed.Slow;
                break;
        }
    }

    /// <summary>
    /// ボタンアニメーションの開始
    /// </summary>
    private void StartButtonAnimation(GameObject[] objects, int selectedObjectNum, bool cameraOption = false)
    {
        step = 0;
        time = 0;
        selectButtonNum = selectedObjectNum;
        minButtonLength = selectedObjectNum;
        foreach(var obj in objects)
        {
            obj.transform.localScale = Vector3.one;
        }
        animationObject = objects;
        actionFlag = cameraOption;
        selectMenu = true;
    }

    /// <summary>
    /// ボタンアニメーションの停止
    /// </summary>
    private void StopButtonAnimation()
    {
        selectMenu = false;
    }
}
