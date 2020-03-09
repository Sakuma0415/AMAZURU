using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    bool anime = false;
    float animeTime = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            anime = true;
            animeTime = 1;
            Progress.progress.key = true;
            GetComponent<Animator>().SetBool("set", true);
        }
    }
    private void Update()
    {
        if (anime)
        {
            animeTime -= Time.deltaTime;
            if (animeTime<0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
