using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainPot : MonoBehaviour
{

    //[SerializeField]
    //WaterHi waterHi;
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

    [SerializeField]
    GameObject Obj;
    [SerializeField]
    LookForCamera look;
    [SerializeField]
    Animator animator;

    bool set = false;

    // Start is called before the first frame update
    void Start()
    {
        //materials= new Material(shaders); 
        //meshRenderer.material = materials;

        look=GetComponent<LookForCamera>();
        look.RainFall = true;
        cameraPos = Camera.main.gameObject.GetComponent<CameraPos>();
        //Ray ray = new Ray(transform.position, transform.TransformDirection(Vector3.down));
        //RaycastHit hit;
        //if (Physics.Raycast(ray, out hit, 200, layerMask))
        //{
        //    waterHi = hit.collider.gameObject.GetComponent<WaterHi>();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        Obj.SetActive(sw);
        if(set&& ControllerInput.Instance.buttonDown.circle && PlayState.playState.gameMode == PlayState.GameMode.Play)
        {
            if (Camera.main.gameObject.GetComponent<CameraPos>().lookAnimeTime <= 0)
            {
                SoundManager.soundManager.PlaySe("btn09", 0.5f);
                look.RainFall = sw;
                sw = !sw;
                animator.SetBool("Bool", sw);
                AmehurashiManager.amehurashi.amehurashiTrueCont += sw ? 1 : -1;
                Progress.progress.waterHi.HiChange((AmehurashiManager.amehurashi.waterStep * AmehurashiManager.amehurashi.amehurashiTrueCont) + 1);
                Camera.main.gameObject.GetComponent<CameraPos>().RainPotChange();
                //UI選択時の奴
                //AmehurashiManager.amehurashi.rainPot = this;
                //AmehurashiManager.amehurashi.hi = transform.position.y - 0.5f;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            set = true;
        }
        }
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            set = false ;
        }
    }



}
