using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 段差を降りれる状態の時表示するテキストを管理するクラス
/// </summary>
public class StepTrueText : MonoBehaviour
{
    //テキストを表示するかどうかのフラグ
    static public bool textFlg = false;

    [Header("設定項目")]
    //テキストのオブジェ
    [SerializeField]
    GameObject textObj;
    //テキストのオブジェ
    [SerializeField]
    Material textMaterial;
    //完全表示までの時間
    [SerializeField]
    float fadeTime;

    //private

    //テキストのフェード処理に使うタイマー
    float textTime = 0;
    //カメラのオブジェ
    GameObject cameraObj;

    //開始時処理
    private void Start()
    {
        StepTrueText.textFlg = false;
        textTime = 0;
        cameraObj = Camera.main.gameObject;
        textMaterial.color = new Color(0, 0, 0, 0);
    }

    void Update()
    {
        //テキストのアルファ値を設定
        textTime = StepTrueText.textFlg ?
            (textTime + Time.deltaTime > fadeTime ? fadeTime : textTime + Time.deltaTime) :
            (textTime - Time.deltaTime < 0 ? 0 : textTime - Time.deltaTime)
            ;
        //textMaterial.color = new Color(textMaterial.color.r, textMaterial.color.g, textMaterial.color.b, textTime*(1/fadeTime));

        //テキストが常にカメラを向く
        textObj.transform.eulerAngles = new Vector3(cameraObj.transform.eulerAngles.x, cameraObj.transform.eulerAngles.y,0);
    }
}
