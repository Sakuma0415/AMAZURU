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

    public List<PrefabStageDatas[]> allPSD = new List<PrefabStageDatas[]>();
    private PrefabStageDatas[] pData;

    [Header("操作負荷"), SerializeField]
    private Vector3 defPos = Vector3.zero;
    private Vector3 dPos;
    [SerializeField]
    private Vector2 scaleAdjust = Vector2.zero;
    private int lineIndex = 0;
    [SerializeField, Range(0, 10), Tooltip("回転速度")]
    private int speed = 1;
    [SerializeField]
    private int rotateAngle = 0;
    private int sumAngle = 0;
    private float h, h2 = 0;
    private float v, v2 = 0;

    private Animator rightAnimator, leftAnimator;

    private bool isRotation, isVerticalMove = false;

    private enum Selection
    {
        Right,
        Left,
        Up,
        Down
    }

    [Tooltip("選択方向")]
    private Selection selection;
    
    [System.Serializable]
    public struct ViewStage
    {
        public string name;
        public int difficulity;
        public int amehurashiNum;
        public int increasedWaterVolume;
        public GameObject stage;
        public Vector3 defScale;
        public Vector3 sizeChangeSpeed;
        public Vector3 minimamSizeChangeSpeed;
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
            sizeChangeSpeed = Vector3.zero;
            minimamSizeChangeSpeed = Vector3.zero;
            verticalMoveSizeChangeSpeed = Vector3.zero;
            index = 0;
            psdIndex = 0;
        }

        /// <summary>
        /// データの引き渡し
        /// </summary>
        /// <param name="v"></param>
        public void HandOver(ViewStage v)
        {
            name = v.name;
            difficulity = v.difficulity;
            amehurashiNum = v.amehurashiNum;
            increasedWaterVolume = v.increasedWaterVolume;
            stage = v.stage;
            defScale = v.defScale;
            sizeChangeSpeed = v.sizeChangeSpeed;
            minimamSizeChangeSpeed = v.minimamSizeChangeSpeed;
            verticalMoveSizeChangeSpeed = v.verticalMoveSizeChangeSpeed;
            index = v.index;
            psdIndex = v.psdIndex;
        }

        /// <summary>
        /// インデックスの増減
        /// <para>i = 0 カウントアップ</para>
        /// <para>i = 1 カウンドダウン</para>
        /// </summary>
        /// <param name="i"></param>
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
    private Vector3 vMoveSpeed = Vector3.zero;

    [System.Serializable]
    public struct SaveSelectStageData
    {
        public bool doRetention;
        public int initNumber;
        public int level;
    }

    private SaveSelectStageData sssd;
    public int level;
    [SerializeField]
    Config config;

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


        sssd = config.save;
        vMoveSpeed = new Vector3(0, speed * 2, 0);
        Init();
    }

    private void Update()
    {
        if (!SceneLoadManager.Instance.SceneLoadFlg)
        {
            level = sssd.level;

            h = ControllerInput.Instance.stick.LStickHorizontal;
            h2 = ControllerInput.Instance.stick.crossHorizontal;
            v = ControllerInput.Instance.stick.LStickVertical;
            v2 = ControllerInput.Instance.stick.crossVertical;

            if (!isRotation && !isVerticalMove) 
            {
                if (h < 0 || h2 < 0)
                {
                    SoundManager.soundManager.PlaySe("cncl05", 1f);
                    leftAnimator.Play("Idle");
                    leftAnimator.SetTrigger("SizeUp");
                    selection = Selection.Left;
                    isRotation = true;
                }
                else if (h > 0 || h2 > 0)
                {
                    SoundManager.soundManager.PlaySe("cncl05", 1f);
                    rightAnimator.Play("Idle");
                    rightAnimator.SetTrigger("SizeUp");
                    selection = Selection.Right;
                    isRotation = true;
                }
                else if (v > 0 || v2 > 0)
                {
                    SoundManager.soundManager.PlaySe("cncl05", 1f);
                    //leftAnimator.Play("Idle");
                    //leftAnimator.SetTrigger("SizeUp");
                    selection = Selection.Up;
                    isVerticalMove = true;
                    sssd.level++;
                    if (sssd.level > allPSD.Count - 1) { sssd.level = 0; }
                    pData = allPSD[sssd.level];
                }
                else if (v < 0 || v2 < 0)
                {
                    SoundManager.soundManager.PlaySe("cncl05", 1f);
                    //leftAnimator.Play("Idle");
                    //leftAnimator.SetTrigger("SizeUp");
                    selection = Selection.Down;
                    isVerticalMove = true;
                    sssd.level--;
                    if (sssd.level < 0) { sssd.level = allPSD.Count - 1; }
                    pData = allPSD[sssd.level];
                }
            }

            if (ControllerInput.Instance.buttonDown.circle)
            {
                StageMake.LoadStageData = sData;
                
                SoundManager.soundManager.StopBgm(0.5f, 1);
                SoundManager.soundManager.StopBgm(0.5f, 0);
                SoundManager.soundManager.PlaySe("btn01", 0.2f);

                if (!sssd.doRetention) { sssd.doRetention = true; }

                config.save = sssd;
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
        if (isRotation)
        {
            StageMoveHorizontal(selection, pData);
        }
        else if (isVerticalMove)
        {
            StageMoveVertical(selection, pData);
        }
    }

    /// <summary>
    /// ステージセレクトの初期化
    /// </summary>
    private void Init()
    {
        if (!SoundManager.soundManager.BGMnull1) { SoundManager.soundManager.PlayBgm("MusMus-BGM-043", 0.1f, 0.2f, 0); }
        if (!SoundManager.soundManager.BGMnull2) { SoundManager.soundManager.PlayBgm("rain_loop", 0.1f, 0.3f, 1); }

        leftAnimator = selectUI[0].GetComponent<Animator>();
        rightAnimator = selectUI[1].GetComponent<Animator>();
        allPSD.Add(psdNormal);
        allPSD.Add(psdExtra);

        if (!sssd.doRetention)
        {
            //一番目のステージが選択された状態にする為の数字
            sssd.initNumber = 2;
            sssd.level = 0;
        }
        
        dPos = defPos;

        for (int i = 0; i < 3; i++)
        {
            pData = allPSD[sssd.level];

            //上下に配置するための処理
            if (i == 1) { dPos = new Vector3(defPos.x, defPos.y + 72, defPos.z); }
            else if (i == 2) { dPos = new Vector3(defPos.x, defPos.y - 72, defPos.z); }

            //ビューステージの生成とデータの格納
            for (int j = 0; j < 5; j++)
            {
                if (pData.Length - 1 < sssd.initNumber)
                {
                    sssd.initNumber = pData.Length - 1;
                }
                DataSetforViewStage(i,j,sssd.initNumber,j, pData);
                viewStage[i, j].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * (j + 8));

                if (i == 0 && j == 2)
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
                    sData = pData[sssd.initNumber].psd.sData;
                }

                if(i == 1 || i == 2) { viewStage[i, j].stage.transform.localScale = Vector3.zero; }

                sssd.initNumber--;
                if (sssd.initNumber == -1) { sssd.initNumber = pData.Length - 1; }
            }
            sssd.initNumber = 2;
            if(i == 0) 
            { 
                sssd.level++; 
                if (sssd.level == allPSD.Count) { sssd.level = allPSD.Count - 1; } 
            }
            else if (i == 1)
            {
                sssd.level = allPSD.Count - 1;
            }
        }

        pData = allPSD[0];
        sssd.level = config.save.level;
        dPos = defPos;
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
            else { magni = scaleAdjust.y / z; }

            Vector2 reSize = new Vector2((x * magni) / scale.x, (z * magni) / scale.y);
            viewStage[i, j].defScale = new Vector3(reSize.x, 1, reSize.y);
        }
        else
        {
            viewStage[i, j].defScale = Vector3.one;
        }
    }

    /// <summary>
    /// ステージの横回転、縦回転で変動するステージのサイズ変化度合いを計算
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    private void SetScaleChangeSpeed(int i, int j)
    {
        if (viewStage[i, j].stage.transform.localScale == Vector3.one)
        {
            Vector2 scale = new Vector2(1.5f - viewStage[i, j].stage.transform.localScale.x,
                                        1.5f - viewStage[i, j].stage.transform.localScale.z);
            viewStage[i, j].sizeChangeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);

            Vector3 scale2 = new Vector2(1.5f, 1.5f);
            viewStage[i, j].verticalMoveSizeChangeSpeed = new Vector3(scale2.x / rotateAngle, scale2.y / rotateAngle, scale2.z / rotateAngle);
        }
        else
        {
            Vector2 scale = new Vector2(0.9f - viewStage[i, j].stage.transform.localScale.x,
                                        0.9f - viewStage[i, j].stage.transform.localScale.z);
            viewStage[i, j].sizeChangeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);

            Vector3 scale2 = new Vector3(0.9f, 0.9f, 0.9f);
            viewStage[i, j].verticalMoveSizeChangeSpeed = new Vector3(scale2.x / rotateAngle, scale2.y / rotateAngle, scale2.z / rotateAngle);
        }

        Vector3 _Scale = viewStage[i, j].defScale;
        viewStage[i, j].minimamSizeChangeSpeed = new Vector3(_Scale.x / rotateAngle, _Scale.y / rotateAngle, _Scale.z / rotateAngle);
    }

    /// <summary>
    /// 表示する難易度の変更
    /// </summary>
    /// <param name="vStage"></param>
    private void DifficultyChange(ViewStage vStage)
    {
        for (int i = 0; i < dfImage.Length; i++)
        {
            if (vStage.difficulity == 0) { dfImage[i].sprite = referenceImage[1]; }

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
    /// ステージ選択
    /// </summary>
    /// <param name="sel">入力方向</param>
    private void StageMoveHorizontal(Selection sel, PrefabStageDatas[] pData)
    {
        StageDataHandOver(selection);

        for (int i = 0; i < viewStage.GetLength(1); i++)
        {
            int s = speed;
            if (sel == Selection.Left) { s *= -1; }
            if (viewStage[0, i].stage == null) { continue; }

            if (sel == Selection.Right)
            {
                switch (viewStage[0, i].index)
                {
                    case -1:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].minimamSizeChangeSpeed;
                        break;
                    case 1:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].sizeChangeSpeed;
                        sData = pData[viewStage[0, i].psdIndex].psd.sData;
                        if (viewStage[0, i].psdIndex + 2 > allPSD[sssd.level].Length - 1)
                        {
                            sssd.initNumber = viewStage[0, i].psdIndex + 2 - allPSD[sssd.level].Length;
                        }
                        else
                        {
                            sssd.initNumber = viewStage[0, i].psdIndex + 2;
                        }
                        break;
                    case 2:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].sizeChangeSpeed;
                        break;
                    case 4:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].minimamSizeChangeSpeed;
                        break;
                }
            }
            else
            {
                switch (viewStage[0, i].index)
                {
                    case 0:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].minimamSizeChangeSpeed;
                        break;
                    case 2:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].sizeChangeSpeed;
                        break;
                    case 3:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].sizeChangeSpeed;
                        sData = pData[viewStage[0, i].psdIndex].psd.sData;
                        if (viewStage[0, i].psdIndex + 2 > allPSD[sssd.level].Length - 1)
                        {
                            sssd.initNumber = viewStage[0, i].psdIndex + 2 - allPSD[sssd.level].Length;
                        }
                        else
                        {
                            sssd.initNumber = viewStage[0, i].psdIndex + 2;
                        }
                        break;
                    case 5:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].minimamSizeChangeSpeed;
                        break;
                }
            }
            viewStage[0, i].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, s);
        }

        sumAngle += speed;

        if (sumAngle >= rotateAngle)
        {
            sumAngle = 0;
            isRotation = false;
            StageDataChange(sel);
        }
    }

    private void StageMoveVertical(Selection sel, PrefabStageDatas[] pData)
    {
        StageDataHandOver(sel);
        for (int j = 0; j < 6; j++)
        {
            //行番号の変更
            if (sel == Selection.Up) { lineIndex = 2; }
            else if (sel == Selection.Down) { lineIndex = 1; }

            //スケールの変更
            if (viewStage[0, j].stage != null)
            {
                if (viewStage[0, j].index == 2)
                {
                    viewStage[0, j].stage.transform.localScale += viewStage[0, j].verticalMoveSizeChangeSpeed;
                }
                else
                {
                    viewStage[0, j].stage.transform.localScale += viewStage[0, j].minimamSizeChangeSpeed;
                }
            }

            if (viewStage[lineIndex, j].stage != null)
            {
                if (viewStage[lineIndex, j].index == 2)
                {
                    viewStage[lineIndex, j].stage.transform.localScale -= viewStage[lineIndex, j].verticalMoveSizeChangeSpeed;
                }
                else
                {
                    viewStage[lineIndex, j].stage.transform.localScale -= viewStage[lineIndex, j].minimamSizeChangeSpeed;
                }
            }

            if (viewStage[0, j].index == 2) { sData = pData[viewStage[0, j].psdIndex].psd.sData; }

            //縦移動の処理
            if (sel == Selection.Up)
            {
                if (viewStage[0, j].stage != null) { viewStage[0, j].stage.transform.position -= vMoveSpeed; }
                if (viewStage[2, j].stage != null) { viewStage[2, j].stage.transform.position -= vMoveSpeed; }
            }
            else if (sel == Selection.Down)
            {
                if (viewStage[0, j].stage != null) { viewStage[0, j].stage.transform.position += vMoveSpeed; }
                if (viewStage[1, j].stage != null) { viewStage[1, j].stage.transform.position += vMoveSpeed; }
            }
        }

        sumAngle += speed;
        if(sumAngle >= rotateAngle)
        {
            sumAngle = 0;
            isVerticalMove = false;
            StageDataChange(sel);
            int initNumber = 2;
            int le = sssd.level;
            int pDataIndex = 0;
            PrefabStageDatas[] p;
            if(sel == Selection.Up)
            {
                pDataIndex = 1;
                //for (int i = 0; i < 2; i++)
                //{
                //    if(le + i > allPSD.Count - 1) { le = 0; }
                //    else { le += i; }
                //}

                le += 1;
                if (le  > allPSD.Count - 1) { le = 0; }

                dPos = new Vector3(defPos.x, defPos.y + 72, defPos.z);
            }
            else if (sel == Selection.Down)
            {
                pDataIndex = 2;

                //for (int i = 2; i < 2; i++)
                //{
                //    if (le - i < 0) { le = allPSD.Count - 1; }
                //    else { le -= i; }
                //}

                le -= 1;
                if (le  < 0) { le = allPSD.Count - 1; }

                dPos = new Vector3(defPos.x, defPos.y - 72, defPos.z);
            }

            p = allPSD[le];

            for (int j = 0; j < 5; j++)
            {
                if (p.Length - 1 < initNumber)
                {
                    initNumber = p.Length - 1;
                }
                DataSetforViewStage(pDataIndex, j, initNumber, j, p);
                viewStage[pDataIndex, j].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * (j + 8));

                viewStage[pDataIndex, j].stage.transform.localScale = Vector3.zero;

                initNumber--;
                if (initNumber == -1) { initNumber = p.Length - 1; }
            }
            dPos = defPos;
        }
    }

    /// <summary>
    /// ステージデータの変更
    /// </summary>
    /// <param name="select"></param>
    private void StageDataChange(Selection select)
    {
        string n, ame, iwv;
        n = "";
        ame = "";
        iwv = "";

        for (int i = 0; i < viewStage.GetLength(1); i++)
        {
            if (select == Selection.Right)
            {
                if (viewStage[0, i].index == 4)
                {
                    Destroy(viewStage[0, i].stage);
                    viewStage[0, i].Init();
                }

                if (viewStage[0, i].stage == null) { continue; }

                viewStage[0, i].CountUpDown();
            }
            else if (select == Selection.Left)
            {
                if (viewStage[0, i].index == 0)
                {
                    Destroy(viewStage[0, i].stage);
                    viewStage[0, i].Init();
                }

                if (viewStage[0, i].stage == null) { continue; }

                viewStage[0, i].CountUpDown(1);
            }

            if (viewStage[0, i].index == 2)
            {
                DifficultyChange(viewStage[0, i]);
                n = viewStage[0, i].name;
                ame = viewStage[0, i].amehurashiNum.ToString();
                iwv = viewStage[0, i].increasedWaterVolume.ToString();
            }
        }

        stageName.text = n;
        amehurashiNum.text = ame;
        increasedWaterVolume.text = iwv;
    }

    /// <summary>
    /// ステージのデータ受け渡し
    /// </summary>
    /// <param name="sel"></param>
    private void StageDataHandOver(Selection sel)
    {
        if (sumAngle == 0)
        {
            if (sel == Selection.Right)
            {
                if (viewStage[0, 0].stage != null)
                {
                    for (int i = 5; i > 0; i--)
                    {
                        viewStage[0, i].HandOver(viewStage[0, i - 1]);
                        if (i == 0)
                        {
                            viewStage[0, i].Init();
                        }
                    }

                    CreateNextStage(selection);
                }
                else
                {
                    CreateNextStage(selection);
                }
            }
            else if (sel == Selection.Left)
            {
                if (viewStage[0, 5].stage != null)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        viewStage[0, i].HandOver(viewStage[0, i + 1]);
                        if (i == 5)
                        {
                            viewStage[0, i].Init();
                        }
                    }

                    CreateNextStage(selection);
                }
                else
                {
                    CreateNextStage(selection);
                }
            }

            if (sel == Selection.Up || sel == Selection.Down)
            {
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 6; j++)
                    {
                        if (sel == Selection.Up)
                        {
                            if (i == 0)
                            {
                                Destroy(viewStage[i + 2, j].stage);
                                viewStage[i + 2, j].Init();
                                viewStage[i + 2, j].HandOver(viewStage[i, j]);
                            }
                            else if (i == 1) 
                            {
                                viewStage[0, j].Init();
                                viewStage[0, j].HandOver(viewStage[i, j]); 
                            }
                        }
                        else if (sel == Selection.Down)
                        {
                            if (i == 0)
                            {
                                Destroy(viewStage[1, j].stage);
                                viewStage[1, j].Init();
                                viewStage[1, j].HandOver(viewStage[0, j]);
                            }
                            else if (i == 1) 
                            {
                                viewStage[0, j].Init();
                                viewStage[0, j].HandOver(viewStage[i + 1, j]); 
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 移動時にステージを生成
    /// </summary>
    /// <param name="sel"></param>
    private void CreateNextStage(Selection sel)
    {
        if (sel == Selection.Right)
        {
            int index = viewStage[0, 1].psdIndex + 1;
            if (index > pData.Length - 1) { index = 0; }
            DataSetforViewStage(0, 0, index, -1, pData);
            viewStage[0, 0].stage.transform.localScale = Vector3.zero;
            viewStage[0, 0].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * 7);
        }
        else if (sel == Selection.Left)
        {
            int index = viewStage[0, 4].psdIndex - 1;
            if (index < 0) { index = pData.Length - 1; }
            DataSetforViewStage(0, 5, index, 5, pData);
            viewStage[0, 5].stage.transform.localScale = Vector3.zero;
            viewStage[0, 5].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * 3);
        }
    }

    /// <summary>
    /// ビューステージのデータをセットする
    /// <para>i = 列</para>
    /// <para>j = 行</para>
    /// <para>number = ステージ番号</para>
    /// <para>index = ビューステージインデックス</para>
    /// </summary>
    /// <param name="i">列</param>
    /// <param name="j">行</param>
    /// <param name="number">ステージ番号</param>
    /// <param name="index">ビューステージのインデックス</param>
    private void DataSetforViewStage(int i, int j, int number, int index, PrefabStageDatas[] p)
    {
        try
        {
            viewStage[i, j].stage = Instantiate(p[number].psd.viewStage);
        }
        catch
        {
            Debug.Log("psdIndex:" + viewStage[0, 4].psdIndex + "," + number);
        }

        viewStage[i, j].name = p[number].psd.stageName;
        viewStage[i, j].difficulity = p[number].psd.diificulty;
        viewStage[i, j].amehurashiNum = p[number].psd.amehurashiNum;
        viewStage[i, j].increasedWaterVolume = p[number].psd.increasedWaterVolume;
        StageReSize(p[number].psd, i, j);
        viewStage[i, j].psdIndex = p[number].stageNumber;
        viewStage[i, j].stage.transform.localScale = viewStage[i, j].defScale;
        SetScaleChangeSpeed(i, j);
        viewStage[i, j].stage.transform.localPosition = dPos;
        viewStage[i, j].index = index;
    }
}
