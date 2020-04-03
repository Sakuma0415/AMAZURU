﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyAnimation : MonoBehaviour
{
    //--------Update関数などで呼び出す簡単なアニメーション処理-------//

    /// <summary>
    /// 待機処理
    /// </summary>
    /// <returns></returns>
    protected bool Wait(float time, float duration)
    {
        if(time < duration)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// スケールの拡大縮小アニメーション
    /// </summary>
    protected bool ScaleAnimation(GameObject obj, float time, float duration, Vector3 start, Vector3 end)
    {
        if (time < duration)
        {
            float diff = time / duration;
            obj.transform.localScale = Vector3.Lerp(start, end, diff);
            return false;
        }
        else
        {
            obj.transform.localScale = end;
            return true;
        }
    }

    /// <summary>
    /// 点滅のアニメーション
    /// </summary>
    protected bool FlashAnimation(GameObject obj, float time, float duration, bool active)
    {
        if (time < duration)
        {
            return false;
        }
        else
        {
            obj.SetActive(active);
            return true;
        }
    }

    /// <summary>
    /// 移動のアニメーション
    /// </summary>
    /// <returns></returns>
    protected bool MoveAnimation(GameObject obj, float time, float duration, Vector3 start, Vector3 end, bool local)
    {
        if(time < duration)
        {
            float diff = time / duration;
            Vector3 move = Vector3.Lerp(start, end, diff);
            _ = local ? obj.transform.localPosition = move : obj.transform.position = move;
            return false;
        }
        else
        {
            _ = local ? obj.transform.localPosition = end : obj.transform.position = end;
            return true;
        }
    }
}