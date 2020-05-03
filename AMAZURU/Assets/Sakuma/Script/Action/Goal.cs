using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゴールを管理するクラス
/// </summary>
public class Goal : MonoBehaviour
{

    //接触判定
    private void OnTriggerEnter(Collider other)
    {

        //プレイヤー接触時
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {

            //アメフラシ全体の数と起動中のアメフラシの数が同じ
            if(AmehurashiManager.amehurashi.amehurashiTrueCont == AmehurashiManager.amehurashi.AmehurashiQuantity)
            {
                PlayState.playState.gameMode = PlayState.GameMode.Clear ;
            }
        }
    }
}
