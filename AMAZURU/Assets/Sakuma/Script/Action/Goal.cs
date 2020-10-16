using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴールを管理するクラス
/// </summary>
public class Goal : MonoBehaviour
{
    [SerializeField]
    float fadeTime=1;
    [SerializeField]
    Material material;
    [SerializeField]
    GameObject tree;
    private void Start()
    {
        material.SetFloat("_Fade", 0f);
    }
    //接触判定
    private void OnTriggerEnter(Collider other)
    {

        //プレイヤー接触時
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {

            //アメフラシ全体の数と起動中のアメフラシの数が同じ
            if(AmehurashiManager.amehurashi.amehurashiTrueCont == AmehurashiManager.amehurashi.AmehurashiQuantity)
            {
                PlayState.playState.gameMode = PlayState.GameMode.ClearFront  ;
                Camera.main.GetComponent<CameraPos>().GoalPos = transform.position;
                Progress.progress.treeObj = tree;
                //StartCoroutine("FadeIn");
            }
        }
    }



    IEnumerator FadeIn()
    {
        float time = 0;

        while (time < fadeTime)
        {
            material.SetFloat("_Fade", time / fadeTime);
            time += Time.deltaTime;
            yield return null;
        }
        material.SetFloat("_Fade",1f);
    }
}
