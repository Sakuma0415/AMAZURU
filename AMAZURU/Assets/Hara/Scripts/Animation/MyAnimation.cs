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

    /// <summary>
    /// 回転のアニメーション
    /// </summary>
    /// <returns></returns>
    protected bool RotateAnimation(GameObject obj, Vector3 forward, Vector3 upwards, float time, bool local)
    {
        Quaternion rot = Quaternion.LookRotation(forward, upwards);
        Quaternion rotSlerp = Quaternion.Slerp(_ = local ? obj.transform.localRotation : obj.transform.rotation, rot, time);
        _ = local ? obj.transform.localRotation = rotSlerp : obj.transform.rotation = rotSlerp;

        Vector3 absRot = new Vector3(Mathf.Abs(rot.x), Mathf.Abs(rot.y), Mathf.Abs(rot.z));
        Vector3 objAbsRot = local ? new Vector3(Mathf.Abs(obj.transform.localRotation.x), Mathf.Abs(obj.transform.localRotation.y), Mathf.Abs(obj.transform.localRotation.z)) : new Vector3(Mathf.Abs(obj.transform.rotation.x), Mathf.Abs(obj.transform.rotation.y), Mathf.Abs(obj.transform.rotation.z));

        if (Vector3.Distance(objAbsRot, absRot) < 0.001f)
        {
            _ = local ? obj.transform.localRotation = rot : obj.transform.rotation = rot;
            return true;
        }
        else
        {
            return false;
        }
    }
}
