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
    
    [SerializeField,Tooltip("初期位置")]
    private Vector3 defPos = Vector3.zero;
    [SerializeField,Tooltip("サイズ調整")]
    private Vector2 scaleAdjust = Vector2.zero;
    [SerializeField, Range(1, 10),Tooltip("回転速度")]
    private float speed = 1;
    [SerializeField]
    private int rotateAngle = 0;
    private float sumAngle = 0;

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
        public Vector3 zeroScalingSpeed;
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
            zeroScalingSpeed = Vector3.zero;
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
            zeroScalingSpeed = v.zeroScalingSpeed;
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

    public ViewStage[] viewStage = new ViewStage[6];

    
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
            float h = Input.GetAxis("Horizontal");
            float h2 = Input.GetAxis("Horizontal3");
            if (h < 0 || h2 < 0)
            {
                if (isRotation) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                selection = Selection.FallBack;
                selectUIs[0].GetComponent<Animator>().SetBool("SizeUp", true);
                isRotation = true;
            }
            else if (h > 0 || h2 > 0)
            {
                if (isRotation) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                selection = Selection.Forward;
                selectUIs[1].GetComponent<Animator>().SetBool("SizeUp", true);
                isRotation = true;
            }

            if (h == 0)
            {
                selectUIs[0].GetComponent<Animator>().SetBool("SizeUp", false);
                selectUIs[1].GetComponent<Animator>().SetBool("SizeUp", false);
            }

            if (Input.GetButtonDown("Circle"))
            {
                StageMake.LoadStageData = sData;
                SoundManager.soundManager.StopBgm(0.5f, 1);
                SoundManager.soundManager.StopBgm(0.5f, 0);
                SoundManager.soundManager.PlaySe("btn01", 0.2f);
                SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
            }
            else if (Input.GetButtonDown("Cross") )
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
        for (int i = 0; i < 5; i++)
        {
            if(i > (_psd.Count - 1) * overCount)
            {
                int _i = i - (psd.Length * overCount); ;
                
                viewStage[i].stage = Instantiate(_psd[_i].viewStage);
                viewStage[i].name = _psd[_i].stageName;
                viewStage[i].difficulity = _psd[_i].diificulty;
                viewStage[i].amehurashiNum = _psd[_i].amehurashiNum;
                viewStage[i].increasedWaterVolume = _psd[_i].increasedWaterVolume;
                StageReSize(_psd[_i], i);
                viewStage[i].psdIndex = psdNumber[_i];
            }
            else
            {
                viewStage[i].stage = Instantiate(_psd[i].viewStage);
                viewStage[i].name = _psd[i].stageName;
                viewStage[i].difficulity = _psd[i].diificulty;
                viewStage[i].amehurashiNum = _psd[i].amehurashiNum;
                viewStage[i].increasedWaterVolume = _psd[i].increasedWaterVolume;
                StageReSize(_psd[i], i);
                viewStage[i].psdIndex = psdNumber[i];
            }

            
            
            viewStage[i].stage.transform.localScale = viewStage[i].defScale;
            SetScaleChangeSpeed(i);
            viewStage[i].stage.transform.position = defPos;
            viewStage[i].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * (i + 8));
            viewStage[i].index = i;
            if(i == 2) 
            {
                if (viewStage[i].defScale != Vector3.one)
                {
                    viewStage[i].stage.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
                }
                else
                {
                    viewStage[i].stage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                }

                DifficultyChange(viewStage[i]);
                stageName.text = viewStage[i].name;
                amehurashiNum.text = viewStage[i].amehurashiNum.ToString();
                increasedWaterVolume.text = viewStage[i].increasedWaterVolume.ToString();
                sData = _psd[i].sData;
            }

            if (i > (psd.Length - 1) * (overCount + 1)) { overCount++; }
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
    private void StageReSize(PrefabStageData obj, int i)
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

            Vector2 reSize = new Vector2((x * magni) / scale.x, (z * magni) / scale.y);
            viewStage[i].defScale = new Vector3(reSize.x, 1, reSize.y);
        }
        else
        {
            viewStage[i].defScale = Vector3.one;
        }
    }

    /// <summary>
    /// スケールチェンジの速さを設定
    /// </summary>
    /// <param name="i"></param>
    private void SetScaleChangeSpeed(int i)
    {
        if (viewStage[i].stage.transform.localScale == Vector3.one)
        {
            Vector2 scale = new Vector2(1.5f - viewStage[i].stage.transform.localScale.x,
                                        1.5f - viewStage[i].stage.transform.localScale.z);
            viewStage[i].reSizeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);
        }
        else
        {
            Vector2 scale = new Vector2(1 - viewStage[i].stage.transform.localScale.x,
                                        1 - viewStage[i].stage.transform.localScale.z);
            viewStage[i].reSizeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);
        }

        Vector3 _scale = viewStage[i].stage.transform.localScale;
        viewStage[i].zeroScalingSpeed = new Vector3(_scale.x / rotateAngle, _scale.y / rotateAngle, _scale.z / rotateAngle);
    }

    /// <summary>
    /// ステージの回転
    /// </summary>
    private void StageRotate(Selection select)
    {
        StageInstantiate(select);

        for (int i = 0; i < viewStage.Length; i++)
        {
            float s = speed;
            if(select == Selection.FallBack) { s *= -1; }

            if(select == Selection.Forward)
            {
                switch (viewStage[i].index) 
                {
                    case 1:
                        viewStage[i].stage.transform.localScale += viewStage[i].reSizeSpeed;
                        sData = psd[viewStage[i].psdIndex].psd.sData;
                        break;
                    case 2:
                        viewStage[i].stage.transform.localScale -= viewStage[i].reSizeSpeed;
                        break;
                    case 4:
                        viewStage[i].stage.transform.localScale -= viewStage[i].zeroScalingSpeed;
                        break;
                    case 5:
                        viewStage[i].stage.transform.localScale += viewStage[i].zeroScalingSpeed;
                        break;
                }
            }
            else
            {
                switch (viewStage[i].index)
                {
                    case 0:
                        viewStage[i].stage.transform.localScale -= viewStage[i].zeroScalingSpeed;
                        break;
                    case 2:
                        viewStage[i].stage.transform.localScale -= viewStage[i].reSizeSpeed;
                        break;
                    case 3:
                        viewStage[i].stage.transform.localScale += viewStage[i].reSizeSpeed;
                        sData = psd[viewStage[i].psdIndex].psd.sData;
                        break;
                    case 5:
                        viewStage[i].stage.transform.localScale += viewStage[i].zeroScalingSpeed;
                        break;
                }
            }
            if(viewStage[i].stage == null) { continue; }
            viewStage[i].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, s) ;
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
            for (int i = 0; i < viewStage.Length; i++)
            {
                if(viewStage[i].index == 4)
                {
                    Destroy(viewStage[i].stage);
                    viewStage[i].Init();
                }
                
                if(viewStage[i].stage == null) { continue; }
                viewStage[i].CountUp();
                if (viewStage[i].index == 2) 
                {
                    DifficultyChange(viewStage[i]);
                    n = viewStage[i].name;
                    ame = viewStage[i].amehurashiNum.ToString();
                    iwv = viewStage[i].increasedWaterVolume.ToString();
                }
            }
        }
        else if (select == Selection.FallBack)
        {
            for (int i = 0; i < viewStage.Length; i++)
            {
                if (viewStage[i].index == 0)
                {
                    Destroy(viewStage[i].stage);
                    viewStage[i].Init();
                }

                if (viewStage[i].stage == null) { continue; }
                viewStage[i].CountDown();
                if (viewStage[i].index == 2)
                {
                    DifficultyChange(viewStage[i]);
                    n = viewStage[i].name;
                    ame = viewStage[i].amehurashiNum.ToString();
                    iwv = viewStage[i].increasedWaterVolume.ToString();
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
            if (viewStage[5].stage != null)
            {
                int x = 4;

                while (true)
                {
                    viewStage[x].TookOver(viewStage[x - 1]);
                    x--;
                    if (x == 0)
                    {
                        viewStage[x].TookOver(viewStage[viewStage.Length - 1]);
                        viewStage[viewStage.Length - 1].Init();
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
            if(viewStage[4].stage != null)
            {
                int x = 0;

                while (true)
                {
                    if (x == 0)
                    {
                        viewStage[viewStage.Length - 1].TookOver(viewStage[x]);
                    }
                    x++;
                    viewStage[x - 1].TookOver(viewStage[x]);
                    if (x == 4) { viewStage[x].Init(); break; }
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
            int index = viewStage[0].psdIndex + 1;
            if (index > psd.Length - 1) { index = 0; }

            viewStage[5].stage = Instantiate(psd[index].psd.viewStage);
            viewStage[5].name = psd[index].psd.stageName;
            viewStage[5].difficulity = psd[index].psd.diificulty;
            viewStage[5].amehurashiNum = psd[index].psd.amehurashiNum;
            viewStage[5].increasedWaterVolume = psd[index].psd.increasedWaterVolume;
            viewStage[5].index = 5;
            viewStage[5].psdIndex = index;
            StageReSize(psd[index].psd, 5);
            viewStage[5].stage.transform.localScale = viewStage[5].defScale;
            viewStage[5].stage.transform.position = defPos;
            SetScaleChangeSpeed(5);
            viewStage[5].stage.transform.localScale = Vector3.zero;
            viewStage[5].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * 7);
        }
        else if (select == Selection.FallBack)
        {
            int index = viewStage[3].psdIndex - 1;
            if(index < 0) { index = psd.Length - 1; }
            viewStage[4].stage = Instantiate(psd[index].psd.viewStage);
            viewStage[4].name = psd[index].psd.stageName;
            viewStage[4].difficulity = psd[index].psd.diificulty;
            viewStage[4].amehurashiNum = psd[index].psd.amehurashiNum;
            viewStage[4].increasedWaterVolume = psd[index].psd.increasedWaterVolume;
            viewStage[4].index = 5;
            viewStage[4].psdIndex = index;
            StageReSize(psd[index].psd, 4);
            viewStage[4].stage.transform.localScale = viewStage[4].defScale;
            viewStage[4].stage.transform.position = defPos;
            SetScaleChangeSpeed(4);
            viewStage[4].stage.transform.localScale = Vector3.zero;
            viewStage[4].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * 3);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(senterPivot.transform.position, pivotCubeSize);
    }
}
