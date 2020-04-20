using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select2 : MonoBehaviour
{
    [SerializeField]
    private GameObject senterPivot;
    [SerializeField]
    private Vector3 pivotCubeSize;

    private List<GameObject> stages = new List<GameObject>();
    private PrefabStageData[] psd;
    [SerializeField]
    private Vector3 pos;
    [SerializeField]
    private Vector2 scaleAdjust;
    [SerializeField,Range(1,10)]
    private float speed = 1;
    private float angle = 0;
    private bool isRotation = false;
    private int angleIndex = 0;
    private string selectStage;

    [System.Serializable]
    public struct ViewStage
    {
        public string name;
        public GameObject stage;
        private Vector3 defScale;
        public Vector3 DefScale
        {
            get
            {
                return defScale;
            }

            set
            {
                defScale = value;
            }
        }
        public Vector3 reSizeSpeed;
        public Vector3 zeroScalingSpeed;
        public bool isSelect;
        public bool isSmall;
        public bool isInvisible;
        public bool isView;
    }

    public ViewStage[] viewStage = new ViewStage[10];

    void Start()
    {
        StageSelectorInit();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (isRotation) { return; }
            isRotation = true;
            angleIndex = 0;
            CheakAngle(angleIndex);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (isRotation) { return; }
            isRotation = true;
            angleIndex = 1;
            CheakAngle(angleIndex);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //ココダヨ～ [selectStage ← 選択中のステージ名が格納されてる変数]
            SceneLoadManager.Instance.LoadScene(SceneLoadManager.SceneName.Action);
        }
    }

    void FixedUpdate()
    {
        if (!isRotation) { return; }
        StageRotation(angleIndex);
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void StageSelectorInit()
    {
        psd = Resources.LoadAll<PrefabStageData>("EditData/");
        float angle = 360 / 10;
        int overCount = 1;
        //int i = 0;
        for (int i = 0; i < 10; i++)
        {
            if(i > (psd.Length - 1) * overCount)
            {
                int _i = i - psd.Length;
                stages.Add(psd[_i].stage);
                StageReSize(psd[_i], i);    
            }
            else
            {
                stages.Add(psd[i].stage);
                StageReSize(psd[i], i);
            }

            viewStage[i].stage = Instantiate(stages[i]);
            viewStage[i].name = "shimojima1";
            viewStage[i].stage.transform.localScale = viewStage[i].DefScale;
            if (viewStage[i].stage.transform.localScale == Vector3.one)
            {
                Vector2 scale = new Vector2(1.5f - viewStage[i].stage.transform.localScale.x,
                                            1.5f - viewStage[i].stage.transform.localScale.z);
                viewStage[i].reSizeSpeed = new Vector3(scale.x / 36, 0 ,scale.y / 36);
            }
            else
            {
                Vector2 scale = new Vector2(0.8f - viewStage[i].stage.transform.localScale.x,
                                            0.8f - viewStage[i].stage.transform.localScale.z);
                viewStage[i].reSizeSpeed = new Vector3(scale.x / 36, 0, scale.y / 36);
            }

            Vector3 _scale = viewStage[i].stage.transform.localScale;
            viewStage[i].zeroScalingSpeed = new Vector3(_scale.x / 36, _scale.y / 36, _scale.z / 36);

            if (i > 3 && i < 9)
            {
                viewStage[i].stage.transform.localScale = Vector3.zero;
            }

            viewStage[i].stage.transform.position = pos;
            
            viewStage[i].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, angle * (i + 1));
            if(angle * (i + 1) == 72) 
            { 
                viewStage[i].stage.transform.localScale = Vector3.one * 0.8f;
                viewStage[i].isSelect = true;
                selectStage = "shimojima1";
            }

            if(i > (psd.Length - 1) * (overCount + 1)) { overCount++; }
        }
    }

    /// <summary>
    /// ステージのリサイズ
    /// </summary>
    /// <param name="obj">ステージデータアセット</param>
    /// <param name="i">インデックス</param>
    private void StageReSize(PrefabStageData obj, int i)
    {
        if (scaleAdjust.x  <= 0 || scaleAdjust.y <= 0) { return; }
        Vector2 scale = new Vector2(obj.gridCells.x, obj.gridCells.z);
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
            viewStage[i].DefScale = new Vector3(reSize.x, 1, reSize.y);
        }
        else
        {
            viewStage[i].DefScale = Vector3.one;
        }
    }

    /// <summary>
    /// ステージの回転
    /// </summary>
    /// <param name="i"></param>
    private void StageRotation(int i)
    {
        for (int j = 0; j < viewStage.Length; j++)
        {
            if (viewStage[j].isSelect) 
            {
                viewStage[j].stage.transform.localScale += viewStage[j].reSizeSpeed;
            }
            if(viewStage[j].isSmall)
            {
                viewStage[j].stage.transform.localScale -= viewStage[j].reSizeSpeed;
            }
            if (viewStage[j].isInvisible)
            {
                viewStage[j].stage.transform.localScale -= viewStage[j].zeroScalingSpeed;
            }
            else if(viewStage[j].isView)
            {
                viewStage[j].stage.transform.localScale += viewStage[j].zeroScalingSpeed;
            }

            float s = speed;
            if(i == 1) { s *= -1; }
            viewStage[j].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, s);
        }

        angle += speed;

        if(angle >= 36)
        {
            angle = 0;
            isRotation = false;
            for (int k = 0; k < viewStage.Length; k++)
            {
                if (viewStage[k].isView) { viewStage[k].isView = false; }
                if (viewStage[k].isInvisible) { viewStage[k].isInvisible = false; }
                if (viewStage[k].isSmall) { viewStage[k].isSmall = false; }
            }
        }
    }

    private void CheakAngle(int j)
    {
        for (int i = 0; i < viewStage.Length; i++)
        {
            if (viewStage[i].isSelect) { viewStage[i].isSelect = false; 
                                         viewStage[i].isSmall = true; }

            Quaternion q = viewStage[i].stage.transform.rotation;
            float adjust = 36;

            if(j == 1){ adjust *= -1; }

            float y = Mathf.Round(adjust + q.eulerAngles.y);
            if(y > 180) { y = Mathf.Round(180 - y); }
            Debug.Log(viewStage[i].stage.name + ":" + y);
            if (y == 72)
            {
                viewStage[i].isSelect = true;
                selectStage = viewStage[i].name;
            }
            else if (y == 180)
            {
                if (j == 1) { continue; }
                viewStage[i].isInvisible = true;
            }
            else if (y == -180)
            {
                if(j == 1) { continue; }
                viewStage[i].isView = true;
            }

            if(j != 1) { continue; }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(senterPivot.transform.position, pivotCubeSize);
    }
}
