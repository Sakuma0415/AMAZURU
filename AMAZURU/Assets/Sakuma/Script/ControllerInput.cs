using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(Input.GetJoystickNames().Length);
        string contName = "";
        for(int i=0;i< Input.GetJoystickNames().Length; i++)
        {
            if (Input.GetJoystickNames()[i] != "")
            {
                contName = Input.GetJoystickNames()[i];
                break;
            }
        }

        Debug.Log("接続しているコントローラーは "+contName+" です。");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
