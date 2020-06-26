using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// この思いしもじーに届け！
/// </summary>
public class StageSlect_SS : MonoBehaviour
{

    [System.Serializable ]
    struct View
    {
        public int back;
        public GameObject[] viewPre;
        public GameObject[] viewObj;
    }
    //ビュー用のオブジェ
    [SerializeField]
    View[] views;
    //回転等のアニメーションの時間
    float animeTime = 0;
    //回転の中心の座標
    [SerializeField]
    Vector3 RotPos = Vector3.zero;
    //回転の半径
    [SerializeField]
    float r = 0;
    //回転の許容角
    [SerializeField]
    float rotAngle = 0;

    //現在参照している配列
    [SerializeField]
    int[] slect = new int[2];

    float changeTime = 0.5f;
    int changeMode = 0;
    GameObject[] PicUpObj = new GameObject[5];
    GameObject[] PicUpObjBf = new GameObject[5];
    void Init()
    {
        //ビューステージの生成
        for(int i=0;i< views.Length;i++)
        {
            views[i].back = 0;
            int cont= views[i].viewPre.Length;
            switch (views[i].viewPre.Length)
            {
                case 1:
                    views[i].viewObj = new GameObject[5];
                    break;
                case 2:
                    views[i].viewObj = new GameObject[6];
                    break;
                case 3:
                    views[i].viewObj = new GameObject[6];
                    break;
                case 4:
                    views[i].viewObj = new GameObject[8];
                    break;
                default:
                    views[i].viewObj = new GameObject[views[i].viewPre.Length];
                    break;
            }




            GameObject Parent=new GameObject();
            Parent.name = i.ToString();
            Parent.transform.parent = transform;
            for (int j = 0; j < views[i].viewObj.Length; j++)
            {
                views[i].viewObj[j]=Instantiate(views[i].viewPre[j% views[i].viewPre.Length ]);
                views[i].viewObj[j].transform.parent = Parent.transform;
            }

        }
        PicUp();
    }

    void PicUp()
    {
        for (int i = 0; i < views.Length; i++)
        {
            for (int j = 0; j < views[i].viewObj.Length; j++)
            {
                views[i].viewObj[j].SetActive(false);
                views[i].viewObj[j].transform.localScale = Vector3.zero;
            }
        }
        int se= slect[1];

        PicUpObj[0] = views[slect[0]].viewObj[((se - 2) < 0) ? (se - 2) + views[slect[0]].viewObj.Length : (se - 2)];
        PicUpObj[1] = views[slect[0]].viewObj[((se - 1) < 0) ? (se - 1) + views[slect[0]].viewObj.Length : (se - 1)];
        PicUpObj[1].transform.localScale = new Vector3 (0.5f, 0.5f, 0.5f);
        PicUpObj[2] = views[slect[0]].viewObj[se];
        PicUpObj[2].transform.localScale = new Vector3(1, 1, 1);
        PicUpObj[3] = views[slect[0]].viewObj[((se + 1) >= views[slect[0]].viewObj.Length) ? (se + 1) - views[slect[0]].viewObj.Length : (se + 1)];
        PicUpObj[3].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        PicUpObj[4] = views[slect[0]].viewObj[((se + 2) >= views[slect[0]].viewObj.Length) ? (se + 2) - views[slect[0]].viewObj.Length : (se + 2)];



        for(int i=0;i< PicUpObj.Length; i++)
        {
            PicUpObj[i].SetActive(true);
            PicUpObj[i].transform.position = RotPos + (new Vector3( Mathf.Cos(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad), 0, Mathf.Sin(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad)) * r);
        }
    }


    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (animeTime == 0)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                slect[1]++;
                if (slect[1] == views[slect[0]].viewObj.Length)
                {
                    slect[1] = 0;
                }
                changeMode = 1;
                animeTime = changeTime;
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                slect[1]--;
                if (slect[1] == -1)
                {
                    slect[1] = views[slect[0]].viewObj.Length - 1;
                }
                changeMode = 0;
                animeTime = changeTime;
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                views[slect[0]].back= slect[1];
                slect[0]++;
                if (slect[0] == views.Length)
                {
                    slect[0] = 0;
                }
                slect[1] = views[slect[0]].back;
                int se = views[slect[0]].back;
                PicUpObjBf[0] = views[slect[0]].viewObj[((se - 2) < 0) ? (se - 2) + views[slect[0]].viewObj.Length : (se - 2)];
                PicUpObjBf[1] = views[slect[0]].viewObj[((se - 1) < 0) ? (se - 1) + views[slect[0]].viewObj.Length : (se - 1)];
                PicUpObjBf[1].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                PicUpObjBf[2] = views[slect[0]].viewObj[se];
                PicUpObjBf[2].transform.localScale = new Vector3(1, 1, 1);
                PicUpObjBf[3] = views[slect[0]].viewObj[((se + 1) >= views[slect[0]].viewObj.Length) ? (se + 1) - views[slect[0]].viewObj.Length : (se + 1)];
                PicUpObjBf[3].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                PicUpObjBf[4] = views[slect[0]].viewObj[((se + 2) >= views[slect[0]].viewObj.Length) ? (se + 2) - views[slect[0]].viewObj.Length : (se + 2)];

                for (int i = 0; i < PicUpObjBf.Length; i++)
                {
                    PicUpObjBf[i].SetActive(true);
                    PicUpObjBf[i].transform.position = RotPos + (new Vector3(Mathf.Cos(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad), 100, Mathf.Sin(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad)) * r);
                }

                changeMode = 2;
                animeTime = changeTime;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                views[slect[0]].back = slect[1];

                slect[0]--;
                if (slect[0] == -1)
                {
                    Debug.Log("a");
                    slect[0] = views.Length - 1;
                }

                slect[1] = views[slect[0]].back;
                int se = views[slect[0]].back;
                PicUpObjBf[0] = views[slect[0]].viewObj[((se - 2) < 0) ? (se - 2) + views[slect[0]].viewObj.Length : (se - 2)];
                PicUpObjBf[1] = views[slect[0]].viewObj[((se - 1) < 0) ? (se - 1) + views[slect[0]].viewObj.Length : (se - 1)];
                PicUpObjBf[1].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                PicUpObjBf[2] = views[slect[0]].viewObj[se];
                PicUpObjBf[2].transform.localScale = new Vector3(1, 1, 1);
                PicUpObjBf[3] = views[slect[0]].viewObj[((se + 1) >= views[slect[0]].viewObj.Length) ? (se + 1) - views[slect[0]].viewObj.Length : (se + 1)];
                PicUpObjBf[3].transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                PicUpObjBf[4] = views[slect[0]].viewObj[((se + 2) >= views[slect[0]].viewObj.Length) ? (se + 2) - views[slect[0]].viewObj.Length : (se + 2)];

                for (int i = 0; i < PicUpObjBf.Length; i++)
                {
                    PicUpObjBf[i].SetActive(true);
                    PicUpObjBf[i].transform.position = RotPos + (new Vector3(Mathf.Cos(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad), 100, Mathf.Sin(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad)) * r);
                }

                changeMode = 3;
                animeTime = changeTime;
            }
        }
        else
        {

            animeTime = (animeTime - Time.deltaTime < 0) ? 0 : animeTime - Time.deltaTime;

            switch (changeMode)
            {
                case 0:
                    PicUpObj[0].transform.position = RotPos + (new Vector3(Mathf.Cos(((-2 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((-2 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);
                    PicUpObj[1].transform.position = RotPos + (new Vector3(Mathf.Cos(((-1 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((-1 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);
                    PicUpObj[2].transform.position = RotPos + (new Vector3(Mathf.Cos(((0 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((0 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);
                    PicUpObj[3].transform.position = RotPos + (new Vector3(Mathf.Cos(((1 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((1 * rotAngle) + ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);

                    PicUpObj[0].transform.localScale = (new Vector3(Mathf.Lerp(0, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(0, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(0, 0.5f, (1 - animeTime / changeTime))));
                    PicUpObj[1].transform.localScale = (new Vector3(Mathf.Lerp(0.5f, 1, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 1, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 1, (1 - animeTime / changeTime))));
                    PicUpObj[2].transform.localScale = (new Vector3(Mathf.Lerp(1, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(1, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(1, 0.5f, (1 - animeTime / changeTime))));
                    PicUpObj[3].transform.localScale = (new Vector3(Mathf.Lerp(0.5f, 0, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 0, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 0, (1 - animeTime / changeTime))));
                    break;
                case 1:
                    PicUpObj[1].transform.position = RotPos + (new Vector3(Mathf.Cos(((-1 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((-1 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);
                    PicUpObj[2].transform.position = RotPos + (new Vector3(Mathf.Cos(((0 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((0 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);
                    PicUpObj[3].transform.position = RotPos + (new Vector3(Mathf.Cos(((1 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((1 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);
                    PicUpObj[4].transform.position = RotPos + (new Vector3(Mathf.Cos(((2 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad), 0, Mathf.Sin(((2 * rotAngle) - ((1 - animeTime / changeTime) * rotAngle)) * Mathf.Deg2Rad)) * r);

                    PicUpObj[1].transform.localScale = (new Vector3(Mathf.Lerp(0.5f, 0, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 0, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 0, (1 - animeTime / changeTime))));
                    PicUpObj[2].transform.localScale = (new Vector3(Mathf.Lerp(1, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(1, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(1, 0.5f, (1 - animeTime / changeTime))));
                    PicUpObj[3].transform.localScale = (new Vector3(Mathf.Lerp(0.5f, 1, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 1, (1 - animeTime / changeTime)), Mathf.Lerp(0.5f, 1, (1 - animeTime / changeTime))));
                    PicUpObj[4].transform.localScale = (new Vector3(Mathf.Lerp(0, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(0, 0.5f, (1 - animeTime / changeTime)), Mathf.Lerp(0, 0.5f, (1 - animeTime / changeTime))));
                    break;
                case 2:
                    for (int i = 0; i < PicUpObj.Length; i++)
                    {
                        PicUpObj[i].transform.position = RotPos + (new Vector3(Mathf.Cos(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad), Mathf.Lerp (0,30, (1 - animeTime / changeTime))/r, Mathf.Sin(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad)) * r);
                    }
                    for (int i = 0; i < PicUpObjBf.Length; i++)
                    {
                        PicUpObjBf[i].transform.position = RotPos + (new Vector3(Mathf.Cos(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad), Mathf.Lerp( -30,0, (1 - animeTime / changeTime)) / r, Mathf.Sin(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad)) * r);
                    }
                    break;
                case 3:
                    for (int i = 0; i < PicUpObj.Length; i++)
                    {
                        PicUpObj[i].transform.position = RotPos + (new Vector3(Mathf.Cos(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad), Mathf.Lerp(0, -30, (1 - animeTime / changeTime)) / r, Mathf.Sin(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad)) * r);
                    }
                    for (int i = 0; i < PicUpObjBf.Length; i++)
                    {
                        PicUpObjBf[i].transform.position = RotPos + (new Vector3(Mathf.Cos(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad), Mathf.Lerp(30,0, (1 - animeTime / changeTime)) / r, Mathf.Sin(((-2 * rotAngle) + (rotAngle * i)) * Mathf.Deg2Rad)) * r);
                    }
                    break;
            }




            if (animeTime == 0)
            {
                PicUp();
            }
        }
    }
}
