using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AmeFurashiAmount : MonoBehaviour
{
    [SerializeField]
    Text text1, text2, text3;
    [SerializeField]
    int fontSizeMax=0, fontSizeMin=0;
    [SerializeField]
    Sprite[] image;
    [SerializeField]
    Sprite backimage;
    [SerializeField]
    Image[] ti1;
    [SerializeField]
    Image[] ti2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private int GetPointDigit(int num, int digit)
    {
        return (int)(num / Mathf.Pow(10, digit - 1)) % 10;
    }

    // Update is called once per frame
    void Update()
    {

        if (AmehurashiManager.amehurashi.AmehurashiQuantity<10)
        {
            ti1[0].sprite  = image[AmehurashiManager.amehurashi.AmehurashiQuantity];
            ti1[1].sprite = backimage;
            ti1[2].sprite = backimage;
        }
        else
        {
            ti1[0].sprite = backimage;
            ti1[2].sprite = image[GetPointDigit(AmehurashiManager.amehurashi.AmehurashiQuantity,1)];
            ti1[1].sprite = image[GetPointDigit(AmehurashiManager.amehurashi.AmehurashiQuantity, 2)];
        }

        if (AmehurashiManager.amehurashi.amehurashiTrueCont < 10)
        {
            ti2[0].sprite = image[AmehurashiManager.amehurashi.amehurashiTrueCont];
            ti2[1].sprite = backimage;
            ti2[2].sprite = backimage;
        }
        else
        {
            ti2[0].sprite = backimage;
            ti2[2].sprite = image[GetPointDigit(AmehurashiManager.amehurashi.amehurashiTrueCont, 1)];
            ti2[1].sprite = image[GetPointDigit(AmehurashiManager.amehurashi.amehurashiTrueCont, 2)];
        }

        //text1.text = AmehurashiManager.amehurashi.amehurashiTrueCont.ToString ();
        //text2.text = AmehurashiManager.amehurashi.AmehurashiQuantity.ToString();

        //if (AmehurashiManager.amehurashi.AmehurashiQuantity < 10)
        //{
        //    text2.fontSize = fontSizeMax;
        //    text1.fontSize = fontSizeMax;
        //    text3.fontSize = fontSizeMax;
        //}
        //else
        //{
        //    text2.fontSize = fontSizeMin;
        //    text1.fontSize = fontSizeMin;
        //    text3.fontSize = fontSizeMin;
        //}

    }
}
