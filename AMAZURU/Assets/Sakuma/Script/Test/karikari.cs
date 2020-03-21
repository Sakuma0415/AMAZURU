using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class karikari : MonoBehaviour
{
    [SerializeField]
    GameObject bgameObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bgameObject.SetActive(PlayState.playState.gameMode == PlayState.GameMode.RainSelect);
    }
}
