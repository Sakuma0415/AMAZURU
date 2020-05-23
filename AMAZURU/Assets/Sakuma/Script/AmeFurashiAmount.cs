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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        text1.text = AmehurashiManager.amehurashi.amehurashiTrueCont.ToString ();
        text2.text = AmehurashiManager.amehurashi.AmehurashiQuantity.ToString();

        if (AmehurashiManager.amehurashi.AmehurashiQuantity < 10)
        {
            text2.fontSize = fontSizeMax;
            text1.fontSize = fontSizeMax;
            text3.fontSize = fontSizeMax;
        }
        else
        {
            text2.fontSize = fontSizeMin;
            text1.fontSize = fontSizeMin;
            text3.fontSize = fontSizeMin;
        }

    }
}
