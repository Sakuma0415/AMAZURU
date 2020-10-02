using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNumber : MonoBehaviour
{

    public void SetDataNum()
    {
        transform.root.transform.GetChild(0).transform.GetChild(0).GetComponent<EnemyDataSet>().selectDataNum = 
            int.Parse(transform.parent.name.Replace("EnemyDataItem:", ""));
    }
}
