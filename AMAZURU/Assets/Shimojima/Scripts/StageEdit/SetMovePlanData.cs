using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMovePlanData : MonoBehaviour
{
    public void SetData()
    {
        EnemyDataSet e = transform.root.transform.GetChild(0).transform.GetChild(0).GetComponent<EnemyDataSet>();
        char c = name[0];
        EnemyDataSet.PositionData p; 
        switch (c)
        {
            case 'X':
                p = e.eed[e.selectDataNum].pData[int.Parse(name.Replace("X", ""))];
                p.x = float.Parse(GetComponent<InputField>().text);
                e.eed[e.selectDataNum].pData[int.Parse(name.Replace("X", ""))] = p;
                break;
            case 'Y':
                p = e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Y", ""))];
                p.y = float.Parse(GetComponent<InputField>().text);
                e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Y", ""))] = p;
                break;
            case 'Z':
                p = e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Z", ""))];
                p.z = float.Parse(GetComponent<InputField>().text);
                e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Z", ""))] = p;
                break;
        }
    }
}
