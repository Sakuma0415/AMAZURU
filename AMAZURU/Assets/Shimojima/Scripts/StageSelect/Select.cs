using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Select : MonoBehaviour
{
    public StageData[] SLoadStageData;
    [SerializeField]
    private StageData sData;

    [SerializeField]
    private GameObject senterPivot;
    [SerializeField]
    private Vector3 pivotCubeSize;

    [SerializeField]
    private Text stageNameText;

    private List<GameObject> stages = new List<GameObject>();
    private PrefabStageData[] psd;
    [SerializeField]
    private Vector3 defPos;
    [SerializeField]
    private Vector2 scaleAdjust;
    [SerializeField, Range(1, 10)]
    private float speed = 1;
    [SerializeField]
    private int rotateAngle;
    private float sumAngle;
    private bool isRotation = false;
    
    private enum Selection
    {
        Forwerd = 0,
        FallBack
    }

    [Tooltip("選択方向")]
    private Selection selection;

    private string selectStage;

    [System.Serializable]
    public struct ViewStage
    {
        public string name;
        public GameObject stage;
        public Vector3 defScale;
        public Vector3 reSizeSpeed;
        public Vector3 zeroScalingSpeed;
        public int index;
        public int psdIndex;

        public void Init()
        {
            name = "";
            stage = null;
            defScale = Vector3.zero;
            reSizeSpeed = Vector3.zero;
            zeroScalingSpeed = Vector3.zero;
            index = 0;
            psdIndex = 0;
        }

        public void TookOver(ViewStage v)
        {
            name = v.name;
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
        Init();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        if (h < 0)
        {
            if (isRotation) { return; }
            selection = Selection.Forwerd;
            isRotation = true;
        }
        else if (h > 0)
        {
            if (isRotation) { return; }
            selection = Selection.FallBack;
            isRotation = true;
        }

        if (Input.GetButtonDown("Circle"))
        {
            StageMake.LoadStageData = sData;
            SoundManager.soundManager.StopBgmBAG();
            SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
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
        psd = Resources.LoadAll<PrefabStageData>("EditData/");
        int overCount = 1;
        for (int i = 0; i < 5; i++)
        {
            if(i > (psd.Length - 1) * overCount)
            {
                int _i = i - (psd.Length * overCount); ;
                
                viewStage[i].stage = Instantiate(psd[_i].viewStage);
                viewStage[i].name = psd[_i].stageName;
                StageReSize(psd[_i], i);
                viewStage[i].psdIndex = _i;
            }
            else
            {
                viewStage[i].stage = Instantiate(psd[i].viewStage);
                viewStage[i].name = psd[i].stageName;
                StageReSize(psd[i], i);
                viewStage[i].psdIndex = i;
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
                    viewStage[i].stage.transform.localScale = Vector3.one;
                }
                else
                {
                    viewStage[i].stage.transform.localScale = new Vector3(1.5f, 1, 1.5f);
                }

                stageNameText.text = viewStage[i].name;
                sData = psd[i].sData;
            }

            if (i > (psd.Length - 1) * (overCount + 1)) { overCount++; }
        }
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

            if(select == Selection.Forwerd)
            {
                switch (viewStage[i].index) 
                {
                    case 1:
                        viewStage[i].stage.transform.localScale += viewStage[i].reSizeSpeed;
                        sData = psd[viewStage[i].psdIndex].sData;
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
                        sData = psd[viewStage[i].psdIndex].sData;
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

    /// <summary>
    /// データ変更
    /// </summary>
    /// <param name="select"></param>
    private void StageDataChange(Selection select)
    {
        //ステージ名
        string n = "";

        if (select == Selection.Forwerd)
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
                if (viewStage[i].index == 2) { n = viewStage[i].name; }
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
                if (viewStage[i].index == 2) { n = viewStage[i].name; }
            }
        }

        stageNameText.text = n;
    }

    /// <summary>
    /// ステージのインスタンス作成
    /// </summary>
    private void StageInstantiate(Selection select)
    {
        if(select == Selection.Forwerd && sumAngle == 0)
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
        if (select == Selection.Forwerd)
        {
            int index = viewStage[0].psdIndex + 1;
            if (index > psd.Length - 1) { index = 0; }

            viewStage[5].stage = Instantiate(psd[index].viewStage);
            viewStage[5].name = psd[index].stageName;
            viewStage[5].index = 5;
            viewStage[5].psdIndex = index;
            StageReSize(psd[index], 5);
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
            viewStage[4].stage = Instantiate(psd[index].viewStage);
            viewStage[4].name = psd[index].stageName;
            viewStage[4].index = 5;
            viewStage[4].psdIndex = index;
            StageReSize(psd[index], 4);
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
