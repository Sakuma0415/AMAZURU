using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetMovePlanData : MonoBehaviour
{
    public void SetData()
    {
        if(GetComponent<InputField>().text == ""){ return; }

        EnemyDataSet e = transform.root.transform.GetChild(0).transform.GetChild(0).GetComponent<EnemyDataSet>();
        char c = name[0];
        EnemyDataSet.PositionData p; 
        switch (c)
        {
            case 'X':
                p = e.eed[e.selectDataNum].pData[int.Parse(name.Replace("X", ""))];
                p.X = float.Parse(GetComponent<InputField>().text);
                e.eed[e.selectDataNum].pData[int.Parse(name.Replace("X", ""))] = p;
                e.eed[e.selectDataNum].obj.GetComponent<LineRenderer>().SetPosition(int.Parse(name.Replace("X", "")), p.Pos);
                break;
            case 'Y':
                p = e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Y", ""))];
                p.Y = float.Parse(GetComponent<InputField>().text);
                e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Y", ""))] = p;
                e.eed[e.selectDataNum].obj.GetComponent<LineRenderer>().SetPosition(int.Parse(name.Replace("Y", "")), p.Pos);
                break;
            case 'Z':
                p = e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Z", ""))];
                p.Z = float.Parse(GetComponent<InputField>().text);
                e.eed[e.selectDataNum].pData[int.Parse(name.Replace("Z", ""))] = p;
                e.eed[e.selectDataNum].obj.GetComponent<LineRenderer>().SetPosition(int.Parse(name.Replace("Z", "")), p.Pos);
                break;
        }
    }
}
