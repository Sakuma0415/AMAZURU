using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AmeFurashiAmount : MonoBehaviour
{
    [SerializeField]
    Text text1, text2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text1.text = AmehurashiManager.amehurashi.amehurashiTrueCont.ToString ();
        text2.text = AmehurashiManager.amehurashi.AmehurashiQuantity.ToString();
    }
}
