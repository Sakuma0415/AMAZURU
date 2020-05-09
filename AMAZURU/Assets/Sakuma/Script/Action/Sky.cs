using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sky : MonoBehaviour
{
    [SerializeField]
    GameObject testobj;
    [SerializeField]
    Rigidbody rigidbody;
    private void FixedUpdate()
    {
        rigidbody.transform.position = testobj.transform.position + new Vector3(0, -5, 0);

    }
}
