using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class NewTypeSelect : MonoBehaviour
{
    private StageData sData;

    [SerializeField]
    private Sprite[] referenceImage;
    [SerializeField]
    private Image[] dfImage, selectUI;
    [SerializeField]
    private GameObject senterPivot;
    [SerializeField]
    private Vector3 pivotCubeSize;
    [SerializeField]
    private TextMeshProUGUI stageName, amehurashiNum, increasedWaterVolume;

    [System.Serializable]
    public struct PrefabStageDatas
    {
        public PrefabStageData psd;
        public int stageNumber;
    }

    public PrefabStageDatas[] psdNormal;
    public PrefabStageDatas[] psdExtra;

    public List<PrefabStageDatas[]> allPSD;

    private int level;

    [Header("操作負荷"), SerializeField]
    private Vector3 defPos = Vector3.zero;
    [SerializeField]
    private Vector2 scaleAdjust = Vector2.zero;
    [SerializeField, Range(0, 10), Tooltip("回転速度")]
    private float speed = 1;
    [SerializeField]
    private int rotateAngle = 0;
    private int sumAngle = 0;
    private float h, h2 = 0;

    private Animator rightAnimator, leftAnimator;

    private bool isRotation = false;

    private enum Selection
    {
        Right,
        Left,
        Up,
        Down
    }

    [Tooltip("選択方向")]
    private Selection selection;

    public struct ViewStage
    {
        public string name;
        public int difficulity;
        public int amehurashiNum;
        public int increasedWaterVolume;
        public GameObject stage;
        public Vector3 defScale;
        public Vector3 sizeChangeSpeed;
        public Vector3 mimamSizeChangeSpeed;
        public int index;
        public int psdIndex;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            name = "";
            difficulity = 0;
            amehurashiNum = 0;
            increasedWaterVolume = 0;
            stage = null;
            defScale = Vector3.zero;
            sizeChangeSpeed = Vector3.zero;
            mimamSizeChangeSpeed = Vector3.zero;
            index = 0;
            psdIndex = 0;
        }

        /// <summary>
        /// データの引き渡し
        /// </summary>
        /// <param name="v"></param>
        public void TookOver(ViewStage v)
        {
            name = v.name;
            difficulity = v.difficulity;
            amehurashiNum = v.amehurashiNum;
            increasedWaterVolume = v.increasedWaterVolume;
            stage = v.stage;
            defScale = v.defScale;
            sizeChangeSpeed = v.sizeChangeSpeed;
            mimamSizeChangeSpeed = v.mimamSizeChangeSpeed;
            index = v.index;
            psdIndex = v.psdIndex;
        }

        public void CountUpDown(int i = 0)
        {
            if (i == 0)
            {
                index++;
                if(index == 6) { index = 0; }
            }
            else if(i == 1)
            {
                index--;
                if(index == -1) { index = 0; }
            }
        }
    }

    public ViewStage[,] viewStage = new ViewStage[3, 6];

    private void Start()
    {
        //warnign潰し
        if (pivotCubeSize == null) { pivotCubeSize = Vector3.zero; }
        if (dfImage.Length == 0) { dfImage = new Image[2]; }
        if (selectUI.Length == 0) { selectUI = new Image[2]; }
        if (!amehurashiNum) { amehurashiNum = new TextMeshProUGUI(); }
        if (!stageName) { stageName = new TextMeshProUGUI(); }
        if (!increasedWaterVolume) { increasedWaterVolume = new TextMeshProUGUI(); }
        if (referenceImage.Length == 0) { referenceImage = new Sprite[1]; }

        Init();
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }

    private void Init()
    {
        if (!SoundManager.soundManager.BGMnull1) { SoundManager.soundManager.PlayBgm("MusMus-BGM-043", 0.1f, 0.2f, 0); }
        if (!SoundManager.soundManager.BGMnull2) { SoundManager.soundManager.PlayBgm("rain_loop", 0.1f, 0.3f, 1); }

        leftAnimator = selectUI[0].GetComponent<Animator>();
        rightAnimator = selectUI[1].GetComponent<Animator>();

        int overCount = 1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
            }
        }
    }
}
