using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsInWater : MonoBehaviour
{

    RemainsScaffold remainsScaffold=null;
    Material material;
    [SerializeField ]
    Color color;
    void Start()
    {
        remainsScaffold = transform.parent.GetComponent<RemainsScaffold>();
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        float intensity = 2;

        if (remainsScaffold.IsInWater)
        {
            intensity = 2f;
        }
        else
        {
            intensity = -1;
        }

        float factor = Mathf.Pow(2, intensity);
        material.SetColor("_EmissionColor", new Color(color.r * factor, color.g * factor, color.b * factor));

    }
}
