using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxMake : MonoBehaviour
{

    
    [SerializeField]
    bool BoxCheck = false;
    //面のマテリアル
    [SerializeField]
    Material material;
    [SerializeField]
    Material topMaterial;
    [SerializeField ]
     public float sethi=0;

    MeshFilter[] meshFilter=new MeshFilter[7];

    Vector2[] movePt = new Vector2[2];
    float moveTime = 0;
    public float Hi=0;
    int cont = 4;
    float lat = 0;
    float bootTime = 1.75f;

    void Start()
    {
        //Init();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            //MoveStart(sethi);
        }
        
        if (BoxCheck)
        {
            moveTime = (moveTime + Time.deltaTime/ bootTime) > 1 ? 1 : (moveTime + Time.deltaTime/ bootTime);
            lat = 1 - ((1 - moveTime) * (1 - moveTime));
        }
        else
        {
            Hi = sethi;
        }

        planeUpdate();





    }

    void planeUpdate()
    {
        cont = 4;

        if (Hi - lat > 0)
        {
            movePt[0] = new Vector2(Hi - lat, 1);
        }
        else
        {
            movePt[0] = new Vector2(0, 1 + (Hi - lat));
            cont -= 1;
        }

        if (Hi + lat < 1)
        {
            movePt[1] = new Vector2(Hi + lat, 0);
        }
        else
        {
            movePt[1] = new Vector2(1, (Hi + lat) - 1);
            cont += 1;
        }

        Vector3[] vertList = new Vector3[cont];

        vertList[0] = new Vector3(0, 0, 0);
        if (Hi + lat < 1)
        {
            if (Hi - lat > 0)
            {
                vertList[3] = movePt[1];
                vertList[2] = movePt[0];
                vertList[1] = new Vector3(0, 1, 0);
            }
            else
            {
                vertList[2] = movePt[1];
                vertList[1] = movePt[0];
            }
        }
        else
        {
            if (Hi - lat > 0)
            {
                vertList[4] = new Vector3(1, 0, 0);
                vertList[3] = movePt[1];
                vertList[2] = movePt[0];
                vertList[1] = new Vector3(0, 1, 0);
            }
            else
            {
                vertList[3] = new Vector3(1, 0, 0);
                vertList[2] = movePt[1];
                vertList[1] = movePt[0];
            }
        }


        //一枚目
        MeshSet(vertList, meshFilter[0], true);

        //裏面
        for (int i = 0; i < vertList.Length; i++)
        {
            vertList[i].z += 1;
        }
        MeshSet(vertList, meshFilter[1], false);




        //L字面

        MeshSet(new Vector3[] {
                new Vector3 (0, 0f,0),
                new Vector3 (0, 0f,1),
                new Vector3 (0f, movePt[0].y,1),
                new Vector3 (0f, movePt[0].y,0.0001f)
                },
            meshFilter[2], true);

        MeshSet(new Vector3[] {
                new Vector3 (0, 0f,0),
                new Vector3 (0, 0f,1),
                new Vector3 (movePt[1].x, 0,1),
                new Vector3 (movePt[1].x, 0,0.0001f)
                },
            meshFilter[3], false);

        //斜面
        MeshSet(new Vector3[] {
                movePt[1],
                (Vector3 )movePt[1]+new Vector3 (0,0,1),
                (Vector3 )movePt[0]+new Vector3 (0,0,1),
                movePt[0]
                },
            meshFilter[4], false);

        //特殊面
        if (Hi - lat > 0)
        {
            meshFilter[5].gameObject.SetActive(true);
            MeshSet(new Vector3[] {
                new Vector3 (0, 1f,0),
                new Vector3 (0, 1f,1),
                new Vector3 (movePt[0].x, 1,1),
                new Vector3 (movePt[0].x, 1,0.0001f)
                },
            meshFilter[5], true);
        }
        else
        {
            meshFilter[5].gameObject.SetActive(false);
        }
        //特殊面
        if (Hi + lat > 1)
        {
            meshFilter[6].gameObject.SetActive(true);
            MeshSet(new Vector3[] {
                new Vector3 (1, 0f,0),
                new Vector3 (1, 0f,1),
                new Vector3 (1, movePt[1].y,1),
                new Vector3 (1, movePt[1].y,0.0001f)
                },
            meshFilter[6], false);
        }
        else
        {
            meshFilter[6].gameObject.SetActive(false);
        }

        if (moveTime == 1)
        {
            BoxCheck = false;
        }
    }

    public void MoveStart(float hi)
    {
        movePt[0] = new Vector2(hi, 1);
        movePt[1] = new Vector2(hi, 0);

        BoxCheck = true;

        moveTime = 0;
        Hi = hi;
    }

    public void Init()
    {
        for (int i=0; i < meshFilter.Length; i++)
        {
            GameObject plane = new GameObject();
            plane.transform.parent = transform;
            plane.transform.localScale = new Vector3(1, 1, 1);
            plane.transform.localPosition = new Vector3(-0.5f, -0.5f, -0.5f);
            plane.transform.localEulerAngles  = Vector3.zero;
            meshFilter[i] = plane.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = plane.AddComponent<MeshRenderer>();
            if (i == 4)
            {
                meshRenderer.material = topMaterial ;
                //plane.AddComponent<PlaneReflection>();
            }
            else
            {
                meshRenderer.material = material;
            }
            






        }
        planeUpdate();
    }

    Mesh MeshSet(Vector3[] vertices ,MeshFilter  meshF,bool reverse)
    {
        var mesh = new Mesh();

        mesh.vertices = vertices;

        int[] vertList = new int[(vertices.Length - 2) * 3];

        if (reverse)
        {
            for (int i = 0; i < vertList.Length; i++)
            {
                switch (i % 3)
                {
                    case 0:
                        vertList[i] = 0;
                        break;
                    default:
                        vertList[i] = (i % 3) + ((int)i / 3);
                        break;

                }
                //Debug.Log(vertList[i]);
            }
        }
        else
        {
            int cont = 0;
            for (int i = vertList.Length-1; i >= 0; i--)
            {

                switch (i % 3)
                {
                    case 0:
                        vertList[cont] = 0;
                        break;
                    default:
                        vertList[cont] = (i % 3) + ((int)i / 3);
                        break;

                }
                cont++;
                //Debug.Log(vertList[i]);
            }
        }
        

        mesh.triangles = vertList;

        mesh.RecalculateNormals();
        meshF.sharedMesh = mesh;
        return mesh;
    }

}
