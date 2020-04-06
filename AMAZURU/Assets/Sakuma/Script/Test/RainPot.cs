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
    [SerializeField]
    LookForCamera forCamera;
    // Start is called before the first frame update
    void Start()
    {
        //materials= new Material(shaders); 
        //meshRenderer.material = materials;



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
        
    }

    private void OnTriggerStay(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            if (Input.GetKeyDown(KeyCode.Space) && PlayState.playState.gameMode == PlayState.GameMode.Play)
            {
                forCamera.RainFall = sw;
                sw = !sw;
                AmehurashiManager.amehurashi.amehurashiTrueCont += sw ? 1 : -1;
                waterHi.HiChange((AmehurashiManager.amehurashi.waterStep * AmehurashiManager.amehurashi.amehurashiTrueCont)+1);
                Camera.main.gameObject.GetComponent<CameraPos>().RainPotChange();
                //UI選択時の奴
                //AmehurashiManager.amehurashi.rainPot = this;
                //AmehurashiManager.amehurashi.hi = transform.position.y - 0.5f;
            }
        }
    }



}
