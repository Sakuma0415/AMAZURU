using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maru : MonoBehaviour
{
    Vector3 pos;
    float time;
    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        time = 0;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = new Vector3 (0, Camera.main.transform.eulerAngles.y,0);
        time += Time.deltaTime;
        transform.position = pos + new Vector3(0,Mathf.Sin ((time*360)*Mathf.Deg2Rad)*0.15f,0);
    }
}
