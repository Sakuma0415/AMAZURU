using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoTest : MonoBehaviour
{

    [SerializeField]
    MeshRenderer meshRenderer ;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Camera"))
        {
            meshRenderer.enabled = false;
            Debug.Log("あ");
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Camera"))
        {
            meshRenderer.enabled = true;
            Debug.Log("は");
        }
    }
}
