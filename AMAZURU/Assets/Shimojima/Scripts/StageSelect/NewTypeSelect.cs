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

    private int level = 0;

    [Header("操作負荷"), SerializeField]
    private Vector3 defPos = Vector3.zero;
    private Vector3 dPos;
    [SerializeField]
    private Vector2 scaleAdjust = Vector2.zero;
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
            mimamSizeChangeSpeed = Vector3.zero;
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
            mimamSizeChangeSpeed = v.mimamSizeChangeSpeed;
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
        if (!SceneLoadManager.Instance.SceneLoadFlg)
        {
            h = ControllerInput.Instance.stick.LStickHorizontal;
            h2 = ControllerInput.Instance.stick.crossHorizontal;
            v = ControllerInput.Instance.stick.LStickVertical;
            v2 = ControllerInput.Instance.stick.crossVertical;

            if (h < 0 || h2 < 0)
            {
                if (isRotation || isVerticalMove) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                leftAnimator.Play("Idle");
                leftAnimator.SetTrigger("SizeUp");
                selection = Selection.Left;
                isRotation = true;
            }
            else if (h > 0 || h2 > 0)
            {
                if (isRotation || isVerticalMove) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                rightAnimator.Play("Idle");
                rightAnimator.SetTrigger("SizeUp");
                selection = Selection.Right;
                isRotation = true;
            }

            if (v < 0 || v2 < 0)
            {
                if (isRotation || isVerticalMove) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                //leftAnimator.Play("Idle");
                //leftAnimator.SetTrigger("SizeUp");
                selection = Selection.Down;
                isVerticalMove = true;
            }
            else if (v > 0 || v2 > 0)
            {
                if (isRotation || isVerticalMove) { return; }
                SoundManager.soundManager.PlaySe("cncl05", 1f);
                //leftAnimator.Play("Idle");
                //leftAnimator.SetTrigger("SizeUp");
                selection = Selection.Up;
                isVerticalMove = true;
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
        StageMoveHorizontal(selection, pData);
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

        //一番目のステージが選択された状態にする為の数字
        int initNumber = 2;
        dPos = defPos;

        for (int i = 0; i < 3; i++)
        {
            //バイオームの数が足りない場合は既存のバイオームを使用
            if (allPSD.Count == i) { pData = allPSD[allPSD.Count - 1]; }
            else { pData = allPSD[i]; }
            //上下に配置するための処理
            if (i == 1) { dPos = new Vector3(defPos.x, defPos.y + 72, defPos.z); }
            else if (i == 2) { dPos = new Vector3(defPos.x, defPos.y - 72, defPos.z); }
            //ビューステージの生成とデータの格納
            for (int j = 0; j < 5; j++)
            {
                if (pData.Length - 1 < initNumber)
                {
                    initNumber = pData.Length - 1;
                }
                DataSetforViewStage(i,j,initNumber,j);
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
                    sData = pData[initNumber].psd.sData;
                }

                if(i == 1 || i == 2) { viewStage[i, j].stage.transform.localScale = Vector3.zero; }

                initNumber--;
                if (initNumber == -1) { initNumber = pData.Length - 1; }
            }
        }

        pData = allPSD[0];
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
        viewStage[i, j].mimamSizeChangeSpeed = new Vector3(_Scale.x / rotateAngle, _Scale.y / rotateAngle, _Scale.z / rotateAngle);
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

            if (sel == Selection.Right)
            {
                switch (viewStage[0, i].index)
                {
                    case -1:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].mimamSizeChangeSpeed;
                        break;
                    case 1:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].sizeChangeSpeed;
                        sData = pData[viewStage[0, i].psdIndex].psd.sData;
                        break;
                    case 2:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].sizeChangeSpeed;
                        break;
                    case 4:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].mimamSizeChangeSpeed;
                        break;
                }
            }
            else
            {
                switch (viewStage[0, i].index)
                {
                    case 0:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].mimamSizeChangeSpeed;
                        break;
                    case 2:
                        viewStage[0, i].stage.transform.localScale -= viewStage[0, i].sizeChangeSpeed;
                        break;
                    case 3:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].sizeChangeSpeed;
                        sData = pData[viewStage[0, i].psdIndex].psd.sData;
                        break;
                    case 5:
                        viewStage[0, i].stage.transform.localScale += viewStage[0, i].mimamSizeChangeSpeed;
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
            StageDataChange(sel);
        }
    }

    private void StageMoveVertical(Selection sel, PrefabStageDatas[] pData)
    {
        StageDataHandOver(sel);

        for (int i = 0; i < 5; i++)
        {
            if (sel == Selection.Up)
            {
                if (viewStage[1, i].index == 2 || viewStage[2, i].index == 2)
                {
                    viewStage[1, i].stage.transform.position += viewStage[0, i].verticalMoveSizeChangeSpeed;
                    viewStage[2, i].stage.transform.position -= viewStage[2, i].verticalMoveSizeChangeSpeed;
                }
                else
                {
                    viewStage[1, i].stage.transform.position += viewStage[0, i].mimamSizeChangeSpeed;
                    viewStage[2, i].stage.transform.position -= viewStage[2, i].mimamSizeChangeSpeed;
                }
            }
            else if (sel == Selection.Down)
            {
                if (viewStage[1, i].index == 2 || viewStage[2, i].index == 2)
                {
                    viewStage[0, i].stage.transform.position += viewStage[0, i].verticalMoveSizeChangeSpeed;
                    viewStage[1, i].stage.transform.position -= viewStage[1, i].verticalMoveSizeChangeSpeed;
                }
                else
                {
                    viewStage[1, i].stage.transform.position += viewStage[0, i].mimamSizeChangeSpeed;
                    viewStage[2, i].stage.transform.position -= viewStage[1, i].mimamSizeChangeSpeed;
                }
            }
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

        if (select == Selection.Right)
        {
            for (int i = 0; i < viewStage.GetLength(1); i++)
            {
                if (viewStage[0, i].index == 4)
                {
                    Destroy(viewStage[0, i].stage);
                    viewStage[0, i].Init();
                }

                if (viewStage[0, i].stage == null) { continue; }

                viewStage[0, i].CountUpDown();
                if (viewStage[0, i].index == 2)
                {
                    DifficultyChange(viewStage[0, i]);
                    n = viewStage[0, i].name;
                    ame = viewStage[0, i].amehurashiNum.ToString();
                    iwv = viewStage[0, i].increasedWaterVolume.ToString();
                }
            }
        }
        else if (select == Selection.Left)
        {
            for (int i = 0; i < viewStage.GetLength(1); i++)
            {
                if (viewStage[0, i].index == 0)
                {
                    Destroy(viewStage[0, i].stage);
                    viewStage[0, i].Init();
                }

                if (viewStage[0, i].stage == null) { continue; }

                viewStage[0, i].CountUpDown(1);
                if (viewStage[0, i].index == 2)
                {
                    DifficultyChange(viewStage[0, i]);
                    n = viewStage[0, i].name;
                    ame = viewStage[0, i].amehurashiNum.ToString();
                    iwv = viewStage[0, i].increasedWaterVolume.ToString();
                }
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
        if (sel == Selection.Right && sumAngle == 0)
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
        else if (sel == Selection.Left && sumAngle == 0)
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
        else if (sel == Selection.Up || sel == Selection.Down && sumAngle == 0)
        {
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 6;)
                {
                    if (sel == Selection.Up)
                    {
                        if (i == 0) 
                        {
                            Destroy(viewStage[2, j].stage);
                            viewStage[i, j].HandOver(viewStage[2, j]); 
                        }
                        else if (i == 1) { viewStage[i, j].HandOver(viewStage[0, j]); }
                    }
                    else if (sel == Selection.Down)
                    {
                        if (i == 0) 
                        {
                            Destroy(viewStage[1, j].stage);
                            viewStage[i, j].HandOver(viewStage[1, j]);
                        }
                        else if (i == 1) { viewStage[i + 1, j].HandOver(viewStage[0, j]); }
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
            DataSetforViewStage(0, 0, index, -1);
            viewStage[0, 0].stage.transform.localScale = Vector3.zero;
            viewStage[0, 0].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * 7);
        }
        else if (sel == Selection.Left)
        {
            int index = viewStage[0, 4].psdIndex - 1;
            if (index < 0) { index = pData.Length - 1; }
            DataSetforViewStage(0, 5, index, 5);
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
    private void DataSetforViewStage(int i, int j, int number, int index)
    {
        viewStage[i, j].stage = Instantiate(pData[number].psd.viewStage);
        viewStage[i, j].name = pData[number].psd.stageName;
        viewStage[i, j].difficulity = pData[number].psd.diificulty;
        viewStage[i, j].amehurashiNum = pData[number].psd.amehurashiNum;
        viewStage[i, j].increasedWaterVolume = pData[number].psd.increasedWaterVolume;
        StageReSize(pData[number].psd, i, j);
        viewStage[i, j].psdIndex = pData[number].stageNumber;
        viewStage[i, j].stage.transform.localScale = viewStage[i, j].defScale;
        SetScaleChangeSpeed(i, j);
        viewStage[i, j].stage.transform.localPosition = dPos;
        viewStage[i, j].index = index;
    }
}
