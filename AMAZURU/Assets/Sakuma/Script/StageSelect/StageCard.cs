using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StageCard : MonoBehaviour
{


    [SerializeField]
    Image image;
    [SerializeField]
    Text title;
    [SerializeField]
    Text rank;

   

    public void CardMake(Sprite sprite,string titleText,int rankNum)
    {
        image.sprite = sprite;
        title.text = titleText;
        switch (rankNum)
        {
            case 0:
                rank.text = "☆☆☆";
                break;
            case 1:
                rank.text = "★☆☆";
                break;
            case 2:
                rank.text = "★★☆";
                break;
            case 3:
                rank.text = "★★★";
                break;
        }
        

    }
}
