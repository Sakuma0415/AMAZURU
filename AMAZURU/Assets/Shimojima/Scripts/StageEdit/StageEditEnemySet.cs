using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageEditEnemySet : MonoBehaviour
{
    [SerializeField]
    private GameObject content, enemyItem;

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            GameObject obj = Instantiate(enemyItem);
            obj.transform.SetParent(content.transform);
        }
    }
}
