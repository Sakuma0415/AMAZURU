﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultAnimation : MyAnimation
{
    [SerializeField, Header("アニメーションの対象オブジェクト")] private GameObject animationObj = null;
    [SerializeField, Header("アニメーション実行間隔"), Range(0, 3)] private float span = 1.0f;
    [SerializeField, Header("移動量"), Range(0, 5)] private float moveDistance = 0;
    [SerializeField, Header("メニューボタン")] private GameObject menuButton = null;
    [SerializeField, Tooltip("アニメーション管理フラグ")] private bool animationFlag = false;
    public bool AnimationFlag { set { animationFlag = value; } }

    private float time = 0;
    private int step = 0;
    private bool stepEnd = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ResultAction();
    }

    /// <summary>
    /// リザルトのアニメーション処理
    /// </summary>
    private void ResultAction()
    {
        if (animationObj == null || menuButton == null) { return; }

        if (animationFlag)
        {
            switch (step)
            {
                case 0:
                    stepEnd = ScaleAnimation(animationObj, time, span, Vector3.zero, Vector3.one);
                    time += Time.deltaTime;
                    break;
                case 1:
                    stepEnd = Wait(time, span * 0.5f);
                    time += Time.deltaTime;
                    break;
                case 2:
                    stepEnd = MoveAnimation(animationObj, time, span, animationObj.transform.localPosition, animationObj.transform.localPosition + Vector3.up * moveDistance, true);
                    time += Time.deltaTime;
                    break;
                case 3:
                    stepEnd = Wait(time, span * 0.5f);
                    time += Time.deltaTime;
                    break;
                default:
                    menuButton.SetActive(true);
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
            step = 0;
            time = 0;
            animationObj.transform.localPosition = Vector3.zero;
            animationObj.transform.localScale = Vector3.zero;
            menuButton.SetActive(false);
        }
    }
}
