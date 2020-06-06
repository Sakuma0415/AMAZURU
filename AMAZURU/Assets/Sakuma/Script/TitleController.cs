using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MyAnimation
{
    [SerializeField, Tooltip("タイトルロゴ")] private GameObject titleLogo = null;
    [SerializeField, Tooltip("タイトル画面の項目")] private GameObject titleMenu = null;
    [SerializeField, Tooltip("クレジット")] private GameObject creditObject = null;
    [SerializeField, Tooltip("カーソルオブジェクト")] private GameObject cursor = null;

    private GameObject[] buttonObject = null;
    private Coroutine coroutine = null;
    private int selectNum = 0;
    private bool creditFlag = false;
    private bool dontInput = false;
    private float changeTime = 0;
    private bool keyDown = false;

    // アニメーション用の変数
    private float animeTime = 0;
    private int step = 0;
    private bool stepEnd = false;
    private bool animeMode = false;
    private bool animeStop = false;

    private void Start()
    {
        Init();
    }
    // Update is called once per frame
    void Update()
    {
        InputKey();
        SelectedMenu();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        // BGMの再生
        SoundManager.soundManager.PlayBgm("MusMus-BGM-043", 0.1f, 0.5f, 0);
        SoundManager.soundManager.PlayBgm("rain_loop", 0.1f, 0.3f, 1);

        // 変数の初期化処理
        selectNum = 0;
        creditFlag = false;
        dontInput = false;
        keyDown = false;
        animeMode = false;
        animeStop = false;
        if(titleMenu != null)
        {
            buttonObject = new GameObject[titleMenu.transform.childCount];
            for(int i = 0; i < buttonObject.Length; i++)
            {
                buttonObject[i] = titleMenu.transform.GetChild(i).gameObject;
            }
        }

        // 必要なオブジェクト以外のオブジェクトを非表示にする
        OpenCredit(false);
    }

    /// <summary>
    /// ゲーム開始のコルーチン処理
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartGameCoroutine()
    {
        // 0.5秒後にシーン遷移を開始
        float changeTime = 0;
        while(changeTime < 0.5f)
        {
            changeTime += Time.deltaTime;
            yield return null;
        }

        SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.StageSlect, false);
        SoundManager.soundManager.VolFadeBgm(1, 0.2f, 0);
        coroutine = null;
    }

    /// <summary>
    /// 選択項目を切り替える処理
    /// </summary>
    /// <returns></returns>
    private void ChangeSelectId(bool conditional, bool flag)
    {
        bool setFlag = false;
        if (flag)
        {
            if(changeTime < 0.3f)
            {
                changeTime += Time.deltaTime;
            }
            else
            {
                setFlag = true;
                changeTime = 0;
            }
        }
        else
        {
            setFlag = true;
        }

        if (setFlag)
        {
            step = 0;
            animeTime = 0;
            buttonObject[selectNum].transform.localScale = Vector3.one;
            selectNum = conditional ? selectNum == 0 ? buttonObject.Length - 1 : selectNum - 1 : selectNum == buttonObject.Length - 1 ? 0 : selectNum + 1;
        }
    }

    /// <summary>
    /// 選択項目オブジェクトのアニメーションを実行する
    /// </summary>
    private void SelectedMenu()
    {
        GameObject animeObject = buttonObject[selectNum];

        // カーソルを移動する
        if(cursor != null)
        {
            Vector3 cursorPos = animeObject.transform.localPosition;
            cursorPos += Vector3.left * 260;
            cursor.transform.localPosition = cursorPos;
        }

        if(animeObject == null) { return; }
        if (animeStop)
        {
            animeObject.transform.localScale = Vector3.one;
            step = 0;
            animeTime = 0;
            return;
        }

        if (animeMode)
        {
            // 点滅アニメーション
            switch (step)
            {
                case 0:
                    stepEnd = FlashAnimation(animeObject, animeTime, 0.1f, false);
                    animeTime += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = FlashAnimation(animeObject, animeTime, 0.1f, true);
                    animeTime += Time.deltaTime;
                    break;
                default:
                    step = 0;
                    return;
            }
        }
        else
        {
            // 拡大縮小アニメーション
            switch (step)
            {
                case 0:
                    stepEnd = ScaleAnimation(animeObject, animeTime, 0.5f, Vector3.one, Vector3.one * 1.1f);
                    animeTime += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = ScaleAnimation(animeObject, animeTime, 0.5f, Vector3.one * 1.1f, Vector3.one);
                    animeTime += Time.deltaTime;
                    break;
                case 2:
                    stepEnd = ScaleAnimation(animeObject, animeTime, 0.5f, Vector3.one, Vector3.one * 0.9f);
                    animeTime += Time.deltaTime;
                    break;
                case 3:
                    stepEnd = ScaleAnimation(animeObject, animeTime, 0.5f, Vector3.one * 0.9f, Vector3.one);
                    animeTime += Time.deltaTime;
                    break;
                default:
                    step = 0;
                    return;
            }
        }

        if (stepEnd)
        {
            step++;
            animeTime = 0;
        }
    }

    /// <summary>
    /// コントローラーの入力処理
    /// </summary>
    private void InputKey()
    {
        if (Input.GetButtonDown("Circle") && SceneLoadManager.Instance.SceneLoadFlg == false && dontInput == false)
        {
            ButtonAction(selectNum);
        }

        if(Input.GetButtonDown("Cross") && creditFlag)
        {
            // ボタンSEの再生
            SoundManager.soundManager.PlaySe("btn01", 0.2f);

            // クレジットを非表示にする
            OpenCredit(false);
        }

        float input = dontInput == false && SceneLoadManager.Instance.SceneLoadFlg == false ? Input.GetAxis("Vertical3") : 0;
        if(Mathf.Abs(input) > 0.1f)
        {
            ChangeSelectId(input > 0, keyDown);
            keyDown = true;
        }
        else
        {
            changeTime = 0;
            keyDown = false;
        }
    }

    /// <summary>
    /// ボタンのアクション
    /// </summary>
    private void ButtonAction(int id)
    {
        // ボタンSEの再生
        SoundManager.soundManager.PlaySe("btn01", 0.2f);

        // idによる処理内容の分岐
        switch (id)
        {
            case 0:
                // ゲーム開始
                StartGame();
                break;
            case 1:
                // クレジット表示
                OpenCredit(true);
                break;
            default:
                // ゲームのシャットダウン
                QuitGame();
                break;
        }
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    private void StartGame()
    {
        if(coroutine != null) { return; }
        coroutine = StartCoroutine(StartGameCoroutine());
        dontInput = true;
        animeMode = true;
    }

    /// <summary>
    /// クレジットの表示・非表示
    /// </summary>
    private void OpenCredit(bool open)
    {
        // 特定のキー入力を制限する
        dontInput = open;

        // タイトルロゴの表示管理
        if (titleLogo != null) { titleLogo.SetActive(!open); }

        // カーソルの非表示
        if(cursor != null) { cursor.SetActive(!open); }

        // 選択項目アニメーションの再生管理
        animeStop = open;

        // 選択項目の表示管理
        if(buttonObject != null)
        {
            foreach(var obj in buttonObject)
            {
                obj.SetActive(!open);
            }
        }
        
        // クレジット表示中かどうかのフラグ
        creditFlag = open;

        // クレジットオブジェクトの表示管理
        if(creditObject != null) { creditObject.SetActive(open); }
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
        #endif
    }
}
