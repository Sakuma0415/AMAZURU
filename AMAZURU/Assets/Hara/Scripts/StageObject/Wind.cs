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

        if(waterPos > transform.position.y)
        {
            // 水中に存在する場合は風を出さない
            return;
        }

        upHeight = 0;
        downHeight = 0;

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

        if(upHeight == 0 && downHeight == 0)
        {
            boxCollider.center = Vector3.zero;
            boxCollider.size = Vector3.one;
        }
        else
        {
            float center = (upHeight - downHeight) * 0.5f;
            float totalHeight = upHeight + downHeight;
            Vector3 localUp = transform.InverseTransformDirection(Vector3.up);

            boxCollider.center = localUp * center;
            boxCollider.size = Vector3.one + localUp * totalHeight;
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

            if(Physics.Raycast(ray, out hit, windMaxArea, layerMask))
            {
                hitDistance = Mathf.Floor(hit.distance - 0.5f);
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
