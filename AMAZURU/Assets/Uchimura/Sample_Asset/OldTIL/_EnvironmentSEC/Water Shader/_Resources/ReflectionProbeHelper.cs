using UnityEngine;
using System.Collections;

public class ReflectionProbeHelper : MonoBehaviour {

    [ExecuteInEditMode]
	// Use this for initialization

    public bool RenderProbe;
    public float RayLength = 100f;
    public float RenderDelay = 0.1f;
    float curTime = 0f;

    ReflectionProbe probe;

	// Update is called once per frame
    void Awake()
    {
        probe = GetComponent<ReflectionProbe>();
    }
	void Update () {
        if (RenderProbe)
        {
            float tempHitY;
            float tempY;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.transform.position, Vector3.down, out hit, RayLength))
            {
                tempHitY = hit.point.y;
            }
            else
            {
                tempHitY = 0f;
            }

            tempY = tempHitY - (Camera.main.transform.position.y - tempHitY);
            transform.position = new Vector3(Camera.main.transform.position.x, tempY, Camera.main.transform.position.z);

            if (probe)
            {
                curTime += Time.deltaTime;

                if (curTime > RenderDelay)
                {
                    probe.RenderProbe();
                    curTime = 0f;
                }
            }
        }
    }
}
