using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select : MonoBehaviour
{
    [SerializeField]
    private GameObject senterPivot;
    [SerializeField]
    private Vector3 pivotCubeSize;

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
    }

    public ViewStage[] viewStage = new ViewStage[5];

    
    void Start()
    {
        Init();
    }

    void Update()
    {
        
    }

    private void Init()
    {
        psd = Resources.LoadAll<PrefabStageData>("EditData/");
        int overCount = 1;
        for (int i = 0; i < 4; i++)
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
            viewStage[i].stage.transform.localScale = viewStage[i].defScale;
            SetScaleChangeSpeed(i);
            viewStage[i].stage.transform.position = defPos;
            viewStage[i].stage.transform.RotateAround(senterPivot.transform.position, Vector3.up, rotateAngle * (i + 1));
            viewStage[i].index = i;
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
            Vector2 scale = new Vector2(0.8f - viewStage[i].stage.transform.localScale.x,
                                        0.8f - viewStage[i].stage.transform.localScale.z);
            viewStage[i].reSizeSpeed = new Vector3(scale.x / rotateAngle, 0, scale.y / rotateAngle);
        }

        Vector3 _scale = viewStage[i].stage.transform.localScale;
        viewStage[i].zeroScalingSpeed = new Vector3(_scale.x / rotateAngle, _scale.y / rotateAngle, _scale.z / rotateAngle);
    }
}
