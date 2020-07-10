using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Select : MonoBehaviour
{
    private StageData sData;

    [SerializeField]
    private Sprite[] referenceImage;
    [SerializeField]
    private Image[] dfImage, selectUIs;
    [SerializeField]
    private GameObject senterPivot;
    [SerializeField]
    private Vector3 pivotCubeSize;

    
    [SerializeField]
    private TextMeshProUGUI stageName, amehurashiNum, increasedWaterVolume;

    [System.Serializable]
    public struct PrefabStageDatas
    {
        //プレファブステージデータ
        public PrefabStageData psd;
        public int stageNumber;
    }

    public PrefabStageDatas[] psd;
    [SerializeField,Tooltip("操作不要変数")]
    private List<PrefabStageData> _psd = new List<PrefabStageData>();
    [SerializeField]
    private List<int> psdNumber = new List<int>();

    private int level = 0;
    
    [SerializeField,Tooltip("初期位置")]
    private Vector3 defPos = Vector3.zero;
    [SerializeField,Tooltip("サイズ調整")]
    private Vector2 scaleAdjust = Vector2.zero;
    [SerializeField, Range(1, 10),Tooltip("回転速度")]
    private float speed = 1;
    [SerializeField]
    private int rotateAngle = 0;
    private float sumAngle = 0;
    private float h, h2 = 0;

    private Animator rightAnimator, leftAnimator;

    private bool isRotation = false;
    private enum Selection
    {
        Forward = 0,
        FallBack
    }

    [Tooltip("選択方向")]
    private Selection selection;

    private string selectStage;

    [System.Serializable]
    public struct ViewStage
    {
        public string name;
        public int difficulity;
        public int amehurashiNum;
        public int increasedWaterVolume;
        public GameObject stage;
        public Vector3 defScale;
        public Vector3 reSizeSpeed;
        public Vector3 zeroSizeChangeSpeed;
        public Vector3 verticalMoveSizeChangeSpeed;
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
            reSizeSpeed = Vector3.zero;
            zeroSizeChangeSpeed = Vector3.zero;
            verticalMoveSizeChangeSpeed = Vector3.zero;
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
            reSizeSpeed = v.reSizeSpeed;
            zeroSizeChangeSpeed = v.zeroSizeChangeSpeed;
            verticalMoveSizeChangeSpeed = v.verticalMoveSizeChangeSpeed;
            index = v.index;
            psdIndex = v.psdIndex;
        }

        public void CountUp()
        {
            index++;
            if(index == 6) { index = 0; }
        }

        public void CountDown()
        {
            index--;
            if (index == -1) { index = 0; }
        }
    }

    public ViewStage[,] viewStage = new ViewStage[3,6];

    
    void Start()
    {
        //Warningつぶし
        if (pivotCubeSize == null) { pivotCubeSize = Vector3.zero; }
        if (dfImage.Length == 0) { dfImage = new Image[2]; }
        if (selectUIs.Length == 0) { selectUIs = new Image[2]; }
        if (!amehurashiNum) { amehurashiNum = new TextMeshProUGUI(); }
        if (!stageName) { stageName = new TextMeshProUGUI(); }
        if (!increasedWaterVolume) { increasedWaterVolume = new TextMeshProUGUI(); }
        if (referenceImage.Length == 0) { referenceImage = new Sprite[1]; }

        Init();
    }

    void Update()
    {
        if (!SceneLoadManager.Instance.SceneLoadFlg)
        {
            h = ControllerInput.Instance.stick.LStickHorizontal;
            h2 = ControllerInput.Instance.stick.crossHorizontal;
            if (h < 0 || h2 < 0)
            {
                if (isRotation) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                leftAnimator.Play("Idle");
                leftAnimator.SetTrigger("SizeUp");
                selection = Selection.FallBack;
                isRotation = true;
            }
            else if (h > 0 || h2 > 0)
            {
                if (isRotation) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                rightAnimator.Play("Idle");
                rightAnimator.SetTrigger("SizeUp");
                selection = Selection.Forward;
                isRotation = true;
            }

            if (ControllerInput.Instance.buttonDown.circle)
            {
                StageMake.LoadStageData = sData;
                SoundManager.soundManager.StopBgm(0.5f, 1);
                SoundManager.soundManager.StopBgm(0.5f, 0);
                SoundManager.soundManager.PlaySe("btn01", 0.2f);
                SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
            }
            else if (ControllerInput.Instance.buttonDown.cross)
            {
                SoundManager.soundManager.StopBgm(0.5f, 1);
                SoundManager.soundManager.StopBgm(0.5f, 0);
                SoundManager.soundManager.PlaySe("btn01", 0.3f);
                SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Title, false);
            }
        }

    }

    private void FixedUpdate()
    {
        if (!isRotation) { return; }
        StageRotate(selection);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {

        if (!SoundManager.soundManager.BGMnull1) { SoundManager.soundManager.PlayBgm("MusMus-BGM-043", 0.1f, 0.2f, 0); }
        if (!SoundManager.soundManager.BGMnull2) { SoundManager.soundManager.PlayBgm("rain_loop", 0.1f, 0.3f, 1); }

        leftAnimator = selectUIs[0].GetComponent<Animator>();
        rightAnimator = selectUIs[1].GetComponent<Animator>();
        
        //表示順に格納、初期化の最後でリストをクリア
        _psd[2] = psd[0].psd;
        _psd[1] = psd[1].psd;
        _psd[0] = psd[2].psd;
        _psd[3] = psd[5].psd;
        _psd[4] = psd[4].psd;

        //表示順に格納、初期化の最後でリストをクリア
        psdNumber[2] = psd[0].stageNumber;
        psdNumber[1] = psd[1].stageNumber;
        psdNumber[0] = psd[2].stageNumber;
        psdNumber[3] = psd[5].stageNumber;
        psdNumber[4] = psd[4].stageNumber;

        int overCount = 1;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                //表示するのは0内のみ
                if(i == 0)
                {
                    if (i * (psd.Length - 1) + j > (_psd.Count - 1) * overCount)
                    {
                        int _j = (i + (psd.Length - 1) + j) - (psd.Length * overCount);

                        viewStage[i, j].stage = Instantiate(_psd[_j].viewStage);
                        viewStage[i, j].name = _psd[_j].stageName;
                        viewStage[i, j].difficulity = _psd[_j].diificulty;
                        viewStage[i, j].amehurashiNum = _psd[_j].amehurashiNum;
                        viewStage[i, j].increasedWaterVolume = _psd[_j].increasedWaterVolume;
                        StageReSize(_psd[_j], i, j);
                        viewStage[i, j].psdIndex = psdNumber[_j];
                    }
                    else
                    {
                        viewStage[i, j].stage = Instantiate(_psd[j].viewStage);
                        viewStage[i, j].name = _psd[j].stageName;
                        viewStage[i, j].difficulity = _psd[j].diificulty;
                        viewStage[i, j].amehurashiNum = _psd[j].amehurashiNum;
                        viewStage[i, j].increasedWaterVolume = _psd[j].increasedWaterVolume;
                        StageReSize(_psd[j], i, j);
                        viewStage[i, j].psdIndex = psdNumber[j];
                    }



                    viewStage[i, j].stage.transform.localScale = viewStage[i, j].defScale;
                    SetScaleChangeSpeed(i , j);
                    viewStage[i, j].stage.transform.position = defPos;
                    viewStage[i, j].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * (j + 8));
                    viewStage[i, j].index = j;
                    if (j == 2)
                    {
                        if (viewStage[i, j].defScale != Vector3.one)
                        {
                            viewStage[i, j].stage.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                        }
                        else
                        {
                            viewStage[i, j].stage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                        }

                        DifficultyChange(viewStage[i, j]);
                        stageName.text = viewStage[i, j].name;
                        amehurashiNum.text = viewStage[i, j].amehurashiNum.ToString();
                        increasedWaterVolume.text = viewStage[i, j].increasedWaterVolume.ToString();
                        sData = _psd[j].sData;
                    }
                }
                if (i * (psd.Length - 1) + j > (psd.Length - 1) * (overCount + 1)) { overCount++; }
            }
        }
            

        //Listのクリア
        _psd.Clear();
        psdNumber.Clear();
    }

    /// <summary>
    /// ステージのリサイズ
    /// </summary>
    /// <param name="obj">ステージデータアセット</param>
    /// <param name="i">インデックス</param>
    private void StageReSize(PrefabStageData obj, int i, int j)
    {
        if(scaleAdjust.x <= 0 || scaleAdjust.y <= 0) { return; }
        Vector2 scale = new Vector2(obj.gridCells.x, obj.gridCells.z);
        if(scale.x > scaleAdjust.x || scale.y > scaleAdjust.y)
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

            if(x > z) { magni = scaleAdjust.x / x; }
            else { magni = scaleAdjust.y / z; }

            Debug.Log(magni);

            Vector2 reSize = new Vector2((x * magni) / scale.x, (z * magni) / scale.y);
            viewStage[i,j].defScale = new Vector3(reSize.x, 1, reSize.y);
        }
        else
        {
            viewStage[i,j].defScale = Vector3.one;
        }
    }

    /// <summary>
    /// スケールチェンジの速さを設定
    /// </summary>
    /// <param name="i"></param>
    private void SetScaleChangeSpeed(int i, int j)
    {
        if (viewStage[i, j].stage.transform.localScale == Vector3.one)
        {
            Vector2 scale = new Vector2(1.5f - viewStage[i, j].stage.transform.localScale.x,
                                        1.5f - viewStage[i, j].stage.transform.localScale.z);
            viewStage[i, j].reSizeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);
        }
        else
        {
            Vector2 scale = new Vector2(1 - viewStage[i, j].stage.transform.localScale.x,
                                        1 - viewStage[i, j].stage.transform.localScale.z);
            viewStage[i, j].reSizeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);
        }

        Vector3 _scale = viewStage[i, j].stage.transform.localScale;
        viewStage[i, j].zeroSizeChangeSpeed = new Vector3(_scale.x / rotateAngle, _scale.y / rotateAngle, _scale.z / rotateAngle);
    }

    /// <summary>
    /// ステージの回転
    /// </summary>
    private void StageRotate(Selection select)
    {
        StageInstantiate(select);

        for (int i = 0; i < viewStage.GetLength(1); i++)
        {
            float s = speed;
            if (select == Selection.FallBack) { s *= -1; }

            if (select == Selection.Forward)
            {
                switch (viewStage[0, i].index)
                {
                    case 1:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].reSizeSpeed;
                        sData = psd[viewStage[i, i].psdIndex].psd.sData;
                        break;
                    case 2:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].reSizeSpeed;
                        break;
                    case 4:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].zeroSizeChangeSpeed;
                        break;
                    case 5:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].zeroSizeChangeSpeed;
                        break;
                }
            }
            else
            {
                Debug.Log(viewStage[0, i].index);
                switch (viewStage[0, i].index)
                {
                    case 0:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].zeroSizeChangeSpeed;
                        break;
                    case 2:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].reSizeSpeed;
                        break;
                    case 3:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].reSizeSpeed;
                        sData = psd[viewStage[i, i].psdIndex].psd.sData;
                        break;
                    case 5:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].zeroSizeChangeSpeed;
                        break;
                }
            }
            if (viewStage[0, i].stage == null) { continue; }
            viewStage[0, i].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, s);
        }

        sumAngle += speed;

        if (sumAngle >= rotateAngle)
        {
            sumAngle = 0;
            isRotation = false;
            StageDataChange(select);
        }
    }

    private void DifficultyChange(ViewStage vStage)
    {
        for (int i = 0; i < dfImage.Length; i++)
        {
            if(vStage.difficulity == 0) { dfImage[i].sprite = referenceImage[1]; }

            if (i < vStage.difficulity)
            {
                dfImage[i].sprite = referenceImage[0];
            }
            else
            {
                dfImage[i].sprite = referenceImage[1];
            }
        }
    }

    /// <summary>
    /// データ変更
    /// </summary>
    /// <param name="select"></param>
    private void StageDataChange(Selection select)
    {
        string n, ame, iwv;
        n = "";
        ame = "";
        iwv = "";

        if (select == Selection.Forward)
        {
            for (int j = 0; j < viewStage.GetLength(1); j++)
            {
                if(viewStage[0, j].index == 4)
                {
                    Destroy(viewStage[0, j].stage);
                    viewStage[0, j].Init();
                }
                
                if(viewStage[0, j].stage == null) { continue; }
                viewStage[0, j].CountUp();
                if (viewStage[0, j].index == 2) 
                {
                    DifficultyChange(viewStage[0, j]);
                    n = viewStage[0, j].name;
                    ame = viewStage[0, j].amehurashiNum.ToString();
                    iwv = viewStage[0, j].increasedWaterVolume.ToString();
                }
            }
        }
        else if (select == Selection.FallBack)
        {
            for (int j = 0; j < viewStage.GetLength(1); j++)
            {
                if (viewStage[0, j].index == 0)
                {
                    Destroy(viewStage[0, j].stage);
                    viewStage[0, j].Init();
                }

                if (viewStage[0, j].stage == null) { continue; }
                viewStage[0, j].CountDown();
                if (viewStage[0, j].index == 2)
                {
                    DifficultyChange(viewStage[0, j]);
                    n = viewStage[0, j].name;
                    ame = viewStage[0, j].amehurashiNum.ToString();
                    iwv = viewStage[0, j].increasedWaterVolume.ToString();
                }
            }
        }
        stageName.text = n;
        amehurashiNum.text = ame;
        increasedWaterVolume.text = iwv;
    }

    /// <summary>
    /// ステージのインスタンス作成
    /// </summary>
    private void StageInstantiate(Selection select)
    {
        if(select == Selection.Forward && sumAngle == 0)
        {
            if (viewStage[0,5].stage != null)
            {
                int x = 4;

                while (true)
                {
                    viewStage[0, x].TookOver(viewStage[0, x - 1]);
                    x--;
                    if (x == 0)
                    {
                        viewStage[0, x].TookOver(viewStage[0, viewStage.GetLength(1) - 1]);
                        viewStage[0, viewStage.GetLength(1) - 1].Init();
                        break;
                    }
                }

                NextStageCreate(select);
            }
            else
            {
                NextStageCreate(select);
            }
        }
        else if(select == Selection.FallBack && sumAngle == 0)
        {
            if(viewStage[0, 4].stage != null)
            {
                int x = 0;

                while (true)
                {
                    if (x == 0)
                    {
                        viewStage[0, viewStage.GetLength(1) - 1].TookOver(viewStage[0, x]);
                    }
                    x++;
                    viewStage[0, x - 1].TookOver(viewStage[0, x]);
                    if (x == 4) { viewStage[0, x].Init(); break; }
                }

                NextStageCreate(select);
            }
            else
            {
                NextStageCreate(select);
            }
        }
    }

    /// <summary>
    /// 次のステージインスタンスを作成
    /// </summary>
    /// <param name="select"></param>
    private void NextStageCreate(Selection select)
    {
        if (select == Selection.Forward)
        {
            int index = viewStage[0,0].psdIndex + 1;
            if (index > psd.Length - 1) { index = 0; }

            viewStage[0, 5].stage = Instantiate(psd[index].psd.viewStage);
            viewStage[0, 5].name = psd[index].psd.stageName;
            viewStage[0, 5].difficulity = psd[index].psd.diificulty;
            viewStage[0, 5].amehurashiNum = psd[index].psd.amehurashiNum;
            viewStage[0, 5].increasedWaterVolume = psd[index].psd.increasedWaterVolume;
            StageReSize(psd[index].psd, 0, 5);
            viewStage[0, 5].psdIndex = index;
            viewStage[0, 5].stage.transform.localScale = viewStage[0, 5].defScale;
            SetScaleChangeSpeed(0, 5);
            viewStage[0, 5].stage.transform.position = defPos;
            viewStage[0, 5].index = 5;

            viewStage[0, 5].stage.transform.localScale = Vector3.zero;
            viewStage[0, 5].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * 7);
        }
        else if (select == Selection.FallBack)
        {
            int index = viewStage[0, 3].psdIndex - 1;
            if(index < 0) { index = psd.Length - 1; }
            viewStage[0, 4].stage = Instantiate(psd[index].psd.viewStage);
            viewStage[0, 4].name = psd[index].psd.stageName;
            viewStage[0, 4].difficulity = psd[index].psd.diificulty;
            viewStage[0, 4].amehurashiNum = psd[index].psd.amehurashiNum;
            viewStage[0, 4].increasedWaterVolume = psd[index].psd.increasedWaterVolume;
            viewStage[0, 4].index = 5;
            viewStage[0, 4].psdIndex = index;
            StageReSize(psd[index].psd, 0, 4);
            viewStage[0, 4].stage.transform.localScale = viewStage[0, 4].defScale;
            viewStage[0, 4].stage.transform.position = defPos;
            SetScaleChangeSpeed(0, 4);
            viewStage[0, 4].stage.transform.localScale = Vector3.zero;
            viewStage[0, 4].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * 3);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(senterPivot.transform.position, pivotCubeSize);
    }
}
