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
                if (index == 6) { index = 0; }
            }
            else if (i == 1)
            {
                index--;
                if (index == -1) { index = 0; }
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
        if (!isRotation) { return; }
    }

    private void Init()
    {
        if (!SoundManager.soundManager.BGMnull1) { SoundManager.soundManager.PlayBgm("MusMus-BGM-043", 0.1f, 0.2f, 0); }
        if (!SoundManager.soundManager.BGMnull2) { SoundManager.soundManager.PlayBgm("rain_loop", 0.1f, 0.3f, 1); }

        leftAnimator = selectUI[0].GetComponent<Animator>();
        rightAnimator = selectUI[1].GetComponent<Animator>();

        //表示するステージの数より配列の要素が少ないときに使用
        int overCount = 1;
        //一番目のステージが選択された状態にする為の数字
        int initNumber = 2;

        for (int i = 0; i < 3; i++)
        {
            PrefabStageDatas[] pData = allPSD[i];
            for (int j = 0; j < 5; j++)
            {
                if (pData.Length - 1 < initNumber)
                {
                    initNumber = pData.Length - 1;
                }

                viewStage[i, j].stage = Instantiate(pData[initNumber].psd.viewStage);
                viewStage[i, j].name = pData[initNumber].psd.stageName;
                viewStage[i, j].difficulity = pData[initNumber].psd.diificulty;
                viewStage[i, j].amehurashiNum = pData[initNumber].psd.amehurashiNum;
                viewStage[i, j].increasedWaterVolume = pData[initNumber].psd.increasedWaterVolume;
                StageReSize(pData[initNumber].psd, i, j);
                viewStage[i, j].psdIndex = pData[initNumber].stageNumber;
                viewStage[i, j].stage.transform.localScale = viewStage[i, j].defScale;

            }
        }
    }

    /// <summary>
    /// ステージがプレビューで一定の大きさになるようにサイズ調整する
    /// </summary>
    /// <param name="data">ステージデータアセット</param>
    /// <param name="i">列インデックス</param>
    /// <param name="j">行インデックス</param>
    private void StageReSize(PrefabStageData data, int i, int j)
    {
        if (scaleAdjust.x <= 0 || scaleAdjust.y < 0) { return; }
        Vector2 scale = new Vector2(data.gridCells.x, data.gridCells.z);
        if (scale.x > scaleAdjust.x || scale.y > scaleAdjust.y)
        {
            int a, b, r;
            if (scale.x > scale.y)
            {
                a = (int)scale.x;
                b = (int)scale.y;
            }
            else
            {
                a = (int)scale.y;
                b = (int)scale.x;
            }

            r = a % b;
            while (r != 0)
            {
                a = b;
                b = r;
                r = a % b;
            }

            float x = scale.x / b;
            float z = scale.y / b;
            float magni;

            if (x > z) { magni = scaleAdjust.x / x; }
            else { magni = scale.y / z; }

            Vector2 reSize = new Vector2((x * magni) / scale.x, (z * magni) / scale.y);
            viewStage[i, j].defScale = new Vector3(reSize.x, 1, reSize.y);
        }
        else
        {
            viewStage[i, j].defScale = Vector3.one;
        }
    }

    private void SetScaleChangeSpeed(int i, int j)
    {
        if (viewStage[i, j].stage.transform.localScale == Vector3.one)
        {
            Vector2 scale = new Vector2(1.5f - viewStage[i, j].stage.transform.localScale.x,
                                        1.5f - viewStage[i, j].stage.transform.localScale.z);
            viewStage[i, j].sizeChangeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);
        }
    }
}
