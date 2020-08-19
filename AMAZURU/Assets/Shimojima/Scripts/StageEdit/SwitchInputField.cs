using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwitchInputField : MonoBehaviour
{
    [System.Serializable]
    public struct SwitchableInputField
    {
        public string name;
        public InputField inputF;
        public int fieldNum;
    }

    public SwitchableInputField[] sif;

    private int selectIF = -1;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeInputField();
        }

        CheackSelectIF();
    }

    private void CheackSelectIF()
    {
        for (int i = 0; i < sif.Length; i++)
        {
            if (sif[i].inputF.isFocused) { selectIF = sif[i].fieldNum; return; }
        }

        selectIF = -1;
    }

    private void ChangeInputField()
    {
        EventSystem.current.SetSelectedGameObject(null);
        selectIF++;
        if (selectIF > sif.Length - 1) { selectIF = 0; }
        //切り替え先のInputFieldの受付を開始
        //
        sif[selectIF].inputF.ActivateInputField();
    }
}
