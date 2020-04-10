using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookForCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject selectUI;
    [SerializeField]
    private GameObject[] images;
    private bool lookCamera = false;
    private bool rainFall;
    /// <summary>
    /// アメフラシが起動しているかの確認
    /// <para>false か true でUIが変更されます</para>
    /// </summary>
    public bool RainFall
    {
        get
        {
            return rainFall;
        }

        set
        {
            if (value)
            {
                images[1].SetActive(false);
                images[0].SetActive(true);
            }
            else
            {
                images[0].SetActive(false);
                images[1].SetActive(true);
            }

            rainFall = value;
        }
    }

    private void Start()
    {
        //RainFall = false;
    }

    void Update()
    {
        if (!lookCamera) { return; }
        LookCamera();
    }

    private void LookCamera()
    {
        Vector3 pos = Camera.main.transform.position - selectUI.transform.position;
        selectUI.transform.rotation = Quaternion.LookRotation(-pos, Vector3.up);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == 9)
        {
            selectUI.SetActive(true);
            lookCamera = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 9)
        {
            selectUI.SetActive(false);
            lookCamera = false;
        }
    }
}
