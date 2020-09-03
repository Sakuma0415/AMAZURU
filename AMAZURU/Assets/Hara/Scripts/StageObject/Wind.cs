using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField, Tooltip("判定用のLayerMask")] private LayerMask layerMask;

    [SerializeField, Header("風の有効範囲(マス)"), Range(1, 5)] private int windMaxArea = 1;
    [SerializeField, Header("風圧"), Range(1, 10)] private float windPower = 1.0f;
    [SerializeField, Header("操作無効時間"), Range(1, 10)] private float dontInputDuration = 1.0f;

    [SerializeField, Header("正面")] private bool forward = true;
    [SerializeField, Header("背面")] private bool back = false;
    [SerializeField, Header("右")] private bool right = false;
    [SerializeField, Header("左")] private bool left = false;

    private Coroutine[] coroutines = null;
    private BoxCollider boxCollider = null;
    private float upHeight = 0;
    private float downHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        WindInit();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayState.playState.gameMode == PlayState.GameMode.Play)
        {
            ShotWind();
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void WindInit()
    {
        coroutines = new Coroutine[4];
        boxCollider = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// 風を出す処理
    /// </summary>
    private void ShotWind()
    {
        // このオブジェクトが水中に存在するかをチェック
        float waterPos;
        try
        {
            waterPos = Progress.progress.waterHi.max;
        }
        catch
        {
            waterPos = 0;
        }

        bool inWater = waterPos > transform.position.y;

        upHeight = 0;
        downHeight = 0;

        if (inWater == false)
        {
            // 対象オブジェクトに風が当たったかをチェック
            if (forward)
            {
                CreateWind(transform.forward, 0);
            }

            if (back)
            {
                CreateWind(-transform.forward, 1);
            }

            if (right)
            {
                CreateWind(transform.right, 2);
            }

            if (left)
            {
                CreateWind(-transform.right, 3);
            }
        }

        if((upHeight > 0 || downHeight > 0) && inWater == false)
        {
            float center = (upHeight - downHeight) * 0.5f;
            float totalHeight = upHeight + downHeight + 1;
            Vector3 localUp = transform.InverseTransformDirection(Vector3.up).normalized;

            Vector3 boxSize = new Vector3(Mathf.Abs(localUp.x) < 1 ? 1 : totalHeight, Mathf.Abs(localUp.y) < 1 ? 1 : totalHeight, Mathf.Abs(localUp.z) < 1 ? 1 : totalHeight);
            Vector3 boxCenter = new Vector3(Mathf.Abs(localUp.x) < 1 ? 0 : localUp.x * center, Mathf.Abs(localUp.y) < 1 ? 0 : localUp.y * center, Mathf.Abs(localUp.z) < 1 ? 0 : localUp.z * center);

            boxCollider.center = boxCenter;
            boxCollider.size = boxSize;
        }
        else
        {
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.one;
        }
    }

    /// <summary>
    /// 風の生成
    /// </summary>
    /// <param name="direction">風向き</param>
    /// <param name="windID">チェック用のID番号</param>
    private void CreateWind(Vector3 direction, int windID)
    {
        RaycastHit hit;
        if(direction == Vector3.up || direction == Vector3.down)
        {
            float hitDistance;
            Ray ray = new Ray(transform.position, direction);

            if(Physics.Raycast(ray, out hit, windMaxArea + 0.5f, layerMask))
            {
                hitDistance = Mathf.Floor(Mathf.Abs(hit.distance - 0.5f));
            }
            else
            {
                hitDistance = windMaxArea;
            }

            if (direction == Vector3.up)
            {
                upHeight = hitDistance;
            }
            else
            {
                downHeight = hitDistance;
            }
        }
        else
        {
            if (coroutines[windID] != null) { return; }

            if (Physics.BoxCast(transform.position, Vector3.one * 0.3f, direction, out hit, Quaternion.identity, windMaxArea))
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    // プレイヤーに当たった場合
                    CharacterMaster.Instance.Player.WindAction(direction * windPower, dontInputDuration);
                    coroutines[windID] = StartCoroutine(IntervalCoroutine(windID));
                }
            }
        }
    }

    /// <summary>
    /// 風のインターバルコルーチン
    /// </summary>
    /// <param name="id">ID番号</param>
    /// <returns></returns>
    private IEnumerator IntervalCoroutine(int id)
    {
        float time = 0;

        while(time < dontInputDuration * 2)
        {
            if(PlayState.playState.gameMode == PlayState.GameMode.Play)
            {
                time += Time.deltaTime;
            }
            yield return null;
        }

        coroutines[id] = null;
    }
}
