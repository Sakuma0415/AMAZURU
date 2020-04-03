using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainPot : MonoBehaviour
{

    [SerializeField]
    WaterHi waterHi;
    [SerializeField]
    LayerMask layerMask;


    public CameraPos cameraPos;
    public bool sw=false;

    [SerializeField]
    MeshRenderer meshRenderer;
    Material materials;
    [SerializeField]
    Shader shaders;

    // Start is called before the first frame update
    void Start()
    {
        materials= new Material(shaders); 
        meshRenderer.material = materials;

        cameraPos = Camera.main.gameObject.GetComponent<CameraPos>();
        Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 200, layerMask))
        {
            waterHi = hit.collider.gameObject.GetComponent<WaterHi>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sw)
        {
            materials.color = Color.red;
        }
        else
        {
            materials.color = Color.blue;
        }




    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Space) && PlayState.playState.gameMode == PlayState.GameMode.Play)
            {
                PlayState.playState.gameMode = PlayState.GameMode.RainSelect;
                Camera.main.gameObject.GetComponent<CameraPos>().RainPotChange();
                AmehurashiManager.amehurashi.rainPot = this;
                AmehurashiManager.amehurashi.hi = transform.position.y - 0.5f;
            }
        }
    }



}
