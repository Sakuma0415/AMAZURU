using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleController : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Circle"))
        {
            SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName .StageSlect );
        }
    }
}
