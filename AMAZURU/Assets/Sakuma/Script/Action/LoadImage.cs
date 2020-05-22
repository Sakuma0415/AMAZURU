using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ロード中の牙城を管理するクラス
/// </summary>
public class LoadImage : MonoBehaviour
{
    [Header("設定項目")]
    //画像のオブジェ
    [SerializeField]
    GameObject[] Object;
    //画像のShader
    [SerializeField]
    Shader shader;
    //画像にかける水の色
    [SerializeField]
    Color color;

    //private

    //経過時間
    float time = 0;
    //…こうしん
    float dotTime = 0;
    //マテリアル
    Material[] material;
    //波のリセットした時の高さ
    float resetHi = 0;

    void Start()
    {

        material = new Material[Object.Length];
        float underPos = 10000; 

        //画像の初期設定
        for (int i=0;i<Object.Length; i++)
        {
            RectTransform rectTransform = Object[i].GetComponent<RectTransform>();
            Texture  sprite = Object[i].GetComponent<Image >().mainTexture ;
            material[i] = new Material (shader);
            Object[i].GetComponent<Image>().material = material[i];
            material[i].SetTexture("_MainTex",sprite);
            material[i].SetColor ("_Color", color );
            material[i].SetVector("_Pos",new Vector4(rectTransform.anchoredPosition.x , rectTransform.anchoredPosition.y, 0,0));
            material[i].SetVector("_Scale", new Vector4(rectTransform.sizeDelta.x, rectTransform.sizeDelta.y,0, 0));
            material[i].SetFloat("_Hi", 0);

            if(underPos> rectTransform.anchoredPosition.y - (rectTransform.sizeDelta.y / 2))
            {
                underPos = rectTransform.anchoredPosition.y - (rectTransform.sizeDelta.y / 2);
            }
            
        }

        //タイマー初期化
        dotTime = 0;
        time = underPos - 10;
        resetHi = time;
    }

    //波の高さをリセット
    public void WaveReSet()
    {
        time= resetHi;
    }

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.O))
        {
            WaveReSet();
        }

        //波更新
        time += Time.deltaTime*40;
        for (int i = 0; i < Object.Length; i++)
        {
            material[i].SetFloat("_Hi", time);
        }

        dotTime += Time.deltaTime*2;
        switch ((int)dotTime % 3)
        {
            case 0:
                Object[1].SetActive(true);
                Object[2].SetActive(false);
                Object[3].SetActive(false);
                break;
            case 1:
                Object[1].SetActive(false);
                Object[2].SetActive(true);
                Object[3].SetActive(false);
                break;
            case 2:
                Object[1].SetActive(false);
                Object[2].SetActive(false);
                Object[3].SetActive(true);
                break;
        }


    }
}
