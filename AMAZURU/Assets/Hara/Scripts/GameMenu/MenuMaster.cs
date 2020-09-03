using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMaster : MonoBehaviour
{
    [SerializeField, Tooltip("リザルトのメニュースクリプト")] private ResultMenu resultMenu = null;
    [SerializeField, Tooltip("ポーズのメニュースクリプト")] private PauseMenu pauseMenu = null;
    [SerializeField, Tooltip("カメラ設定のメニュースクリプト")] private CameraOptionMenu cameraOption = null;
    [SerializeField, Tooltip("ポーズ画面用の背景")] private GameObject backGround = null;

    // Start is called before the first frame update
    void Start()
    {
        MenuInit();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMenuState();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void MenuInit()
    {
        // 各初期化処理を実行
        resultMenu.ResultInit();
        pauseMenu.PauseInit();
        cameraOption.CamOpInit();

        // ゲーム開始時にすべてのウィンドウを非表示にする
        resultMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);
        cameraOption.gameObject.SetActive(false);
        backGround.SetActive(false);
    }

    /// <summary>
    /// リザルト画面の表示
    /// </summary>
    /// <param name="state"></param>
    private void ResultWindow(bool state)
    {
        // 他のメニューウィンドウを非表示
        Pause(false);

        // ウィンドウの表示
        resultMenu.gameObject.SetActive(true);
        resultMenu.StartResultAnimation(state);
        
    }

    /// <summary>
    /// ポーズ画面の表示
    /// </summary>
    private void PauseWindow()
    {
        // 他のメニューウィンドウを非表示
        resultMenu.gameObject.SetActive(false);
        cameraOption.gameObject.SetActive(false);

        // ウィンドウの表示
        pauseMenu.SetCursor();
        pauseMenu.gameObject.SetActive(true);
    }

    /// <summary>
    /// カメラ設定画面の表示
    /// </summary>
    private void CameraWindow()
    {
        // 他のメニューウィンドウを非表示
        resultMenu.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(false);

        // ウィンドウの表示
        cameraOption.SetCursor();
        cameraOption.gameObject.SetActive(true);
    }

    /// <summary>
    /// リザルトの表示処理を開始する
    /// </summary>
    /// <param name="isClear">クリアならtrue、ゲームオーバーならfalse</param>
    public void StartResult(bool isClear)
    {
        ResultWindow(isClear);
    }

    /// <summary>
    /// ポーズ画面の表示・非表示
    /// </summary>
    /// <param name="active"></param>
    public void Pause(bool active)
    {
        if (active)
        {
            PauseWindow();
            backGround.SetActive(true);
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            cameraOption.gameObject.SetActive(false);
            backGround.SetActive(false);
        }
    }

    /// <summary>
    /// 各メニューのフラグをチェック
    /// </summary>
    private void CheckMenuState()
    {
        if (pauseMenu.IsOpenCamOption)
        {
            // カメラオプションを開くフラグを検知したら実行
            pauseMenu.IsOpenCamOption = false;
            CameraWindow();
        }

        if (pauseMenu.IsClosePause)
        {
            // ポーズ画面を閉じるフラグを検知したら実行
            Pause(false);
        }

        if (cameraOption.IsOpenPause)
        {
            // ポーズ画面を開くフラグを検知したら実行
            cameraOption.IsOpenPause = false;
            PauseWindow();
        }
    }
}
