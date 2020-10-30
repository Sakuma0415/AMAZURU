using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{

    [SerializeField, Tooltip("雷のアニメーター")] private Animator thunderAnimator = null;
    [SerializeField, Tooltip("雷のパーティクル")] private ParticleSystem thunderParticle = null;
    [SerializeField, Tooltip("雷のオブジェクト")] private GameObject thunderObject = null;

    // 雷の再生を管理するフラグ
    private bool thunderFlag = false;

    private void Start()
    {
        thunderObject.SetActive(false);
    }

    /// <summary>
    /// 雷を再生する
    /// </summary>
    /// <param name="position">雷を落とす座標</param>
    public void PlayThunder(Vector3 position)
    {
        gameObject.transform.position = position;
        thunderObject.SetActive(true);
        thunderFlag = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (thunderFlag && thunderAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            thunderFlag = false;
            thunderObject.SetActive(false);
        }
    }
}
