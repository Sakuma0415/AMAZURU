using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select2 : MonoBehaviour
{
    [SerializeField]
    private GameObject senterPivot;
    [SerializeField]
    private Vector3 size;

    private List<GameObject> stages = new List<GameObject>();
    [SerializeField]
    private Vector3 pos;
    [SerializeField]
    private Vector3 defSize;
    [SerializeField]
    private GameObject[] viewStage = new GameObject[4];

    void Start()
    {
        StageSelectorInit();
    }

    void Update()
    {
        
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void StageSelectorInit()
    {
        GameObject[] objs = Resources.LoadAll<GameObject>("Prefabs/Stage");
        float angle = 360 / objs.Length;
        int i = 0;
        foreach (GameObject obj in objs)
        {
            stages.Add(Instantiate(obj));
            stages[i].transform.position = pos;
            stages[i].transform.localScale = defSize;
            stages[i].transform.RotateAround(senterPivot.transform.position, Vector3.up , angle * (i + 1));
            i++;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(senterPivot.transform.position, size);
    }
}
