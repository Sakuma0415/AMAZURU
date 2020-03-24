using UnityEngine;
using System.Collections;

public class MoveLight : MonoBehaviour
{

    Transform myTransform;
    Vector3 m_v3Position;

    public Vector3 m_v3Speed;

    // Use this for initialization
    void Start()
    {

        m_v3Speed = new Vector3(0.0075f, 0.0f, 0.001f);

        myTransform = transform;
        m_v3Position = myTransform.localPosition;

    }

    // Update is called once per frame
    void Update()
    {

        //m_v3Position += m_v3Speed;
        myTransform.localPosition += m_v3Speed;

    }
}
