using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainTest : MonoBehaviour
{
    [SerializeField]
    GameObject gameObject;
    [SerializeField]
    ParticleSystem ps;
    void Start()
    {
        ps = Instantiate(gameObject).transform .GetChild (0).GetComponent<ParticleSystem>();
        var sh = ps.shape;
        sh.enabled = true;
        sh.scale =(new Vector3(100, 10, 0));
        
    }
    
    void Update()
    {
        
    }
}
