using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの入力を管理するクラス
/// </summary>
public class ControllerInput : MonoBehaviour
{
    //インスタンス
    static public ControllerInput Instance;

    //PS4コントローラかどうかのフラグ
    [SerializeField]
    bool IsPS4Controller = false;
    [System .Serializable ]
    public struct Button
    {
        public bool circle;
        public bool cross;
        public bool triangle;
        public bool option;
        public void Init()
        {
            circle = false;
            cross = false;
            triangle = false;
            option = false;
        }
    }
    [System.Serializable]
    public struct ButtonDown
    {
        public bool circle;
        public bool cross;
        public bool triangle;
        public bool option;
        public void Init()
        {
            circle = false;
            cross = false;
            triangle = false;
            option = false;
        }
    }
    [System.Serializable]
    public struct Stick
    {
        public float LStickHorizontal;
        public float LStickVertical;
        public float RStickHorizontal;
        public float RStickVertical;
        public float crossHorizontal;
        public float crossVertical;
        public void Init()
        {
            LStickHorizontal = 0;
            LStickVertical = 0;
            RStickHorizontal = 0;
            RStickVertical = 0;
            crossHorizontal = 0;
            crossVertical = 0;
        }
    }

    public Stick stick;
    public Button button;
    public ButtonDown buttonDown;

    private void Awake()
    {
        if(Instance==null)
        {
            string contName = "";
            for (int i = 0; i < Input.GetJoystickNames().Length; i++)
            {
                if (Input.GetJoystickNames()[i] != "")
                {
                    contName = Input.GetJoystickNames()[i];
                    break;
                }
            }

            //Debug.Log("接続しているコントローラーは " + contName + " です。");
            stick.Init();
            button.Init();
            buttonDown.Init();
            IsPS4Controller = !(contName == "Controller (XBOX 360 For Windows)");
            DontDestroyOnLoad(gameObject);
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    //
    void Update()
    {

        if (IsPS4Controller)
        {
            Instance.button.circle = Input.GetButton("Circle");
            Instance.button.cross = Input.GetButton("Cross");
            Instance.button.triangle = Input.GetButton("Triangle");
            Instance.button.option = Input.GetButton("Option");

            Instance.buttonDown.circle = Input.GetButtonDown ("Circle");
            Instance.buttonDown.cross = Input.GetButtonDown("Cross");
            Instance.buttonDown.triangle = Input.GetButtonDown("Triangle");
            Instance.buttonDown.option = Input.GetButtonDown("Option");

            Instance.stick.LStickHorizontal = Input.GetAxis("Horizontal");
            Instance.stick.LStickVertical  = Input.GetAxis("Vertical");
            Instance.stick.RStickHorizontal  = Input.GetAxis("Horizontal2");
            Instance.stick.RStickVertical  = Input.GetAxis("Vertical2");
            Instance .stick .crossHorizontal = Input.GetAxis("Horizontal3");
            Instance.stick.crossVertical = Input.GetAxis("Vertical3");
        }
        else
        {
            Instance.button.circle = Input.GetButton("Cross");
            Instance.button.cross = Input.GetButton("A");
            Instance.button.triangle = Input.GetButton("Triangle");
            Instance.button.option = Input.GetButton("Start");

            Instance.buttonDown.circle = Input.GetButtonDown("Cross");
            Instance.buttonDown.cross = Input.GetButtonDown("A");
            Instance.buttonDown.triangle = Input.GetButtonDown("Triangle");
            Instance.buttonDown.option = Input.GetButtonDown("Start");

            Instance.stick.LStickHorizontal = Input.GetAxis("Horizontal");
            Instance.stick.LStickVertical = Input.GetAxis("Vertical");
            Instance.stick.RStickHorizontal = Input.GetAxis("DPadH");
            Instance.stick.RStickVertical =-1* Input.GetAxis("DPadV");
            Instance.stick.crossHorizontal =-1* Input.GetAxis("Vertical2");
            Instance.stick.crossVertical = Input.GetAxis("Horizontal3");
        }

    }
}
