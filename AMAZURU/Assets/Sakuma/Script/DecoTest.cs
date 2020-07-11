using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoTest : MonoBehaviour
{

    [SerializeField]
    LayerMask layerMask;
    private void Update()
    {
        bool Ist = false;
        Collider[] col = Physics.OverlapCapsule(VewPos.vewcolPos.endPt1, VewPos.vewcolPos.endPt2, VewPos.vewcolPos.rad, layerMask);
        for(int i = 0; i < col.Length; i++)
        {
            if(col[i].gameObject ==this.gameObject)
            {
                Debug.Log("as");
                Ist = true;
            }
        }

        if (Ist)
        {
            gameObject.layer = LayerMask.NameToLayer("Deco");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Deco2");
        }

    }

}
