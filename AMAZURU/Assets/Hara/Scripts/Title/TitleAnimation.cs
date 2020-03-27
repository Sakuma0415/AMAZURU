using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimationType
{
    Scale,
    Flash
}

public class TitleAnimation : MonoBehaviour
{
    [SerializeField, Header("アニメーションの対象オブジェクト")] private GameObject titleObject = null;
    [SerializeField, Header("アニメーション実行間隔"), Range(0, 3)] private float span = 1.0f;
    [SerializeField, Header("アニメーションの切り替え")] private AnimationType animeType = AnimationType.Scale;
    public AnimationType AnimeType { set { animeType = value; } }

    private Vector3 startSize = Vector3.zero;
    private Vector3 maxSize = Vector3.zero;
    private Vector3 minSize = Vector3.zero;
    private float sizeRange = 0.15f;
    private float time = 0;
    private int step = 0;
    private bool stepEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        TitleAnimetion();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        startSize = titleObject.transform.localScale;
        maxSize = startSize * (1 + sizeRange);
        minSize = startSize * (1 - sizeRange);
    }

    /// <summary>
    /// タイトルの文字のアニメーション処理
    /// </summary>
    private void TitleAnimetion()
    {
        if(animeType == AnimationType.Scale)
        {
            float duration = span / 4;
            titleObject.SetActive(true);

            switch (step)
            {
                case 0:
                    stepEnd = ScaleAnimation(titleObject, time, duration, startSize, maxSize);
                    time += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = ScaleAnimation(titleObject, time, duration, maxSize, startSize);
                    time += Time.deltaTime;
                    break;
                case 2:
                    stepEnd = ScaleAnimation(titleObject, time, duration, startSize, minSize);
                    time += Time.deltaTime;
                    break;
                case 3:
                    stepEnd = ScaleAnimation(titleObject, time, duration, minSize, startSize);
                    time += Time.deltaTime;
                    break;
                default:
                    step = 0;
                    return;
            }

            if (stepEnd)
            {
                step++;
                time = 0;
            }
        }
        else
        {
            titleObject.transform.localScale = startSize;

            float duration = span / 12;

            switch (step)
            {
                case 0:
                    stepEnd = FlashAnimation(titleObject, time, duration, false);
                    time += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = FlashAnimation(titleObject, time, duration, true);
                    time += Time.deltaTime;
                    break;
                default:
                    step = 0;
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
    /// スケールの拡大縮小アニメーション
    /// </summary>
    private bool ScaleAnimation(GameObject animationObject, float nowTime, float duration, Vector3 start, Vector3 end)
    {
        if (nowTime < duration)
        {
            float diff = nowTime / duration;
            animationObject.transform.localScale = Vector3.Lerp(start, end, diff);
            return false;
        }
        else
        {
            animationObject.transform.localScale = end;
            return true;
        }
    }

    /// <summary>
    /// 点滅のアニメーション
    /// </summary>
    private bool FlashAnimation(GameObject animationObject, float nowTime, float duration, bool active)
    {
        if(nowTime < duration)
        {
            return false;
        }
        else
        {
            animationObject.SetActive(active);
            return true;
        }
    }
}
