using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetNumber : MonoBehaviour
{
    private void OnMouseDown()
    {
        transform.parent.GetComponent<EnemyDataSet>().selectDataNum = int.Parse(transform.parent.name.Replace("EnemyDataItem", ""));
    }
}
