using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleAnimation : MyAnimation
{
    [SerializeField, Header("アニメーションの対象オブジェクト")] private GameObject animationObj = null;
    [SerializeField, Header("アニメーション実行間隔"), Range(0, 3)] private float span = 1.0f;
    [SerializeField, Header("拡大縮小値"), Range(0, 1)]private float sizeRange = 0.15f;
    public bool AnimationFlag { set; private get; } = false;

    private Vector3 startSize = Vector3.zero;
    private Vector3 maxSize = Vector3.zero;
    private Vector3 minSize = Vector3.zero;
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
        TitleAction();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        if(animationObj == null) { return; }
        startSize = animationObj.transform.localScale;
        maxSize = startSize * (1 + sizeRange);
        minSize = startSize * (1 - sizeRange);
    }

    /// <summary>
    /// タイトルの文字のアニメーション処理
    /// </summary>
    private void TitleAction()
    {
        if(animationObj == null) { return; }

        if(AnimationFlag)
        {
            animationObj.transform.localScale = startSize;

            float duration = span / 8;

            switch (step)
            {
                case 0:
                    stepEnd = FlashAnimation(animationObj, time, duration, false);
                    time += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = FlashAnimation(animationObj, time, duration, true);
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
            float duration = span / 4;
            animationObj.SetActive(true);

            switch (step)
            {
                case 0:
                    stepEnd = ScaleAnimation(animationObj, time, duration, startSize, maxSize);
                    time += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = ScaleAnimation(animationObj, time, duration, maxSize, startSize);
                    time += Time.deltaTime;
                    break;
                case 2:
                    stepEnd = ScaleAnimation(animationObj, time, duration, startSize, minSize);
                    time += Time.deltaTime;
                    break;
                case 3:
                    stepEnd = ScaleAnimation(animationObj, time, duration, minSize, startSize);
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
}
