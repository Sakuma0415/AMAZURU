using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenu : MyAnimation
{
    [SerializeField, Tooltip("カーソル")] private GameObject cursorObject = null;
    [SerializeField, Tooltip("項目")] private GameObject menuObject = null;

    /// <summary>
    /// カメラオプションを開くフラグ
    /// </summary>
    public bool IsOpenCamOption { set; get; } = false;

    /// <summary>
    /// ポーズ画面を閉じるフラグ
    /// </summary>
    public bool IsClosePause { private set; get; } = false;

    private TextMeshProUGUI[] textMeshes = null;
    private int selectID = 0;
    private bool actionFlag = false;

    // 選択項目のアニメーション用の変数
    private float selectAnimeTimer = 0;
    private int step = 0;
    private int selectedID = 0;  // 選択していた項目番号を保存する変数

    // カーソル用の変数
    private float cursorChangeTimer = 0;
    private bool inputFlag = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (actionFlag == false) { return; }
        CursorMove();
        SelectAnimation();
        SelectAction();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void PauseInit()
    {
        textMeshes = new TextMeshProUGUI[menuObject.transform.childCount];
        for (int i = 0; i < textMeshes.Length; i++)
        {
            textMeshes[i] = menuObject.transform.GetChild(i).gameObject.GetComponent<TextMeshProUGUI>();
        }
    }

    /// <summary>
    /// 項目を表示してカーソルを合わせる
    /// </summary>
    public void SetCursor()
    {
        selectID = 0;
        selectedID = 0;

        // カーソルの位置を設定
        Vector3 pos = textMeshes[selectID].transform.position;
        pos += Vector3.left * textMeshes[selectID].rectTransform.sizeDelta.x * 0.6f;
        cursorObject.transform.position = pos;

        // 項目オブジェクトの初期化
        foreach (var txt in textMeshes)
        {
            txt.transform.localScale = Vector3.one;
        }

        // カーソルと項目の表示
        cursorObject.SetActive(true);
        menuObject.SetActive(true);

        actionFlag = true;
    }

    /// <summary>
    /// 選択項目に合わせてカーソルの移動
    /// </summary>
    private void CursorMove()
    {
        // 選択項目
        GameObject selectObject = textMeshes[selectID].gameObject;

        // カーソルの位置
        cursorObject.transform.position = new Vector3(cursorObject.transform.position.x, selectObject.transform.position.y, cursorObject.transform.position.z);

        // キー入力の取得
        float input = ControllerInput.Instance.stick.crossVertical;
        int key = input > 0.1f ? -1 : input < -0.1f ? 1 : 0;

        // キー入力によって選択項目を変える
        if (inputFlag == false && key != 0)
        {
            selectID += key;
            if (selectID < 0) { selectID = textMeshes.Length - 1; }
            if (selectID > textMeshes.Length - 1) { selectID = 0; }
            inputFlag = true;
        }
        else
        {
            if (cursorChangeTimer < 0.15f && key != 0)
            {
                cursorChangeTimer += Time.deltaTime;
            }
            else
            {
                cursorChangeTimer = 0;
                inputFlag = false;
            }
        }
    }

    /// <summary>
    /// 選択項目を強調するアニメーション
    /// </summary>
    private void SelectAnimation()
    {
        // 選択項目
        GameObject selectObject = textMeshes[selectID].gameObject;

        // 選択項目が切り替わったら実行
        if (selectID != selectedID)
        {
            step = 0;
            selectAnimeTimer = 0;
            textMeshes[selectedID].transform.localScale = Vector3.one;

            // ID情報の更新
            selectedID = selectID;
        }

        bool end;

        // 選択状態のボタンのアニメーションを実行する
        switch (step)
        {
            case 0:
                end = ScaleAnimation(selectObject, selectAnimeTimer, 0.25f, Vector3.one, Vector3.one * 1.05f);
                selectAnimeTimer += Time.deltaTime;
                break;
            case 1:
                end = ScaleAnimation(selectObject, selectAnimeTimer, 0.25f, Vector3.one * 1.05f, Vector3.one);
                selectAnimeTimer += Time.deltaTime;
                break;
            case 2:
                end = ScaleAnimation(selectObject, selectAnimeTimer, 0.25f, Vector3.one, Vector3.one * 0.95f);
                selectAnimeTimer += Time.deltaTime;
                break;
            default:
                end = ScaleAnimation(selectObject, selectAnimeTimer, 0.25f, Vector3.one * 0.95f, Vector3.one);
                selectAnimeTimer += Time.deltaTime;
                break;
        }

        if (end)
        {
            step++;
            if (step > 3) { step = 0; }
            selectAnimeTimer = 0;
        }
    }

    /// <summary>
    /// 選択項目の処理を実行
    /// </summary>
    private void SelectAction()
    {
        if (ControllerInput.Instance.buttonDown.circle)
        {
            // 選択項目
            string select = textMeshes[selectedID].text;

            // 入力時のSEを再生
            SoundManager.soundManager.PlaySe("btn01", 0.2f);

            switch (select)
            {
                case "ステージせんたく":
                    SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.StageSelect_v2, false);
                    break;
                case "リトライ":
                    SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
                    break;
                case "タイトル":
                    SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Title, false);
                    break;
                default:
                    IsOpenCamOption = true;
                    return;
            }

            // BGMの停止
            SoundManager.soundManager.StopBgm(0.5f, 0);
            SoundManager.soundManager.StopBgm(0.5f, 1);

            // メニューを閉じる
            IsClosePause = true;
        }
    }
}
