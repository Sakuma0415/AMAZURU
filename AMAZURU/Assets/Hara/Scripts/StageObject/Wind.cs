using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class Wind : MonoBehaviour
{
    [SerializeField, Tooltip("風のエフェクト")] private ParticleSystem windEffect = null;
    [SerializeField, Tooltip("判定用のLayerMask")] private LayerMask layerMask;

    [SerializeField, Header("風の有効範囲(マス)"), Range(1, 5)] private int windMaxArea = 1;
    [SerializeField, Header("風の吹き飛ばし範囲(有効範囲 + nマス)"), Range(1, 10)] private int blowAwayArea = 1;
    [SerializeField, Header("風圧"), Range(1, 10)] private float windPower = 1.0f;

    private Coroutine coroutine = null;
    private BoxCollider boxCollider = null;
    private float upHeight = 0;
    private float downHeight = 0;
    private PlayState.GameMode gameMode = PlayState.GameMode.Stop;

    // Start is called before the first frame update
    void Start()
    {
        WindInit();
    }

    // Update is called once per frame
    void Update()
    {
        ShotWind();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void WindInit()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    /// <summary>
    /// 風を出す処理
    /// </summary>
    private void ShotWind()
    {
        try
        {
            gameMode = PlayState.playState.gameMode;
        }
        catch
        {
            gameMode = PlayState.GameMode.Play;
        }

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

        if (gameMode == PlayState.GameMode.Play)
        {
            upHeight = 0;
            downHeight = 0;

            if (inWater == false)
            {
                // 対象オブジェクトに風が当たったかをチェック
                CreateWind(transform.forward);
            }

            if ((upHeight > 0 || downHeight > 0) && inWater == false)
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
        WindEffectControl(gameMode, inWater == false);
    }

    /// <summary>
    /// 風の生成
    /// </summary>
    /// <param name="direction">風向き</param>
    private void CreateWind(Vector3 direction)
    {
        RaycastHit hit;
        if(direction == Vector3.up || direction == Vector3.down)
        {
            float hitDistance;
            Ray ray = new Ray(transform.position, direction);

            if(Physics.Raycast(ray, out hit, windMaxArea + 0.5f, layerMask) && hit.collider.isTrigger == false)
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
            if (coroutine != null) { return; }

            if (Physics.BoxCast(transform.position, Vector3.one * 0.4f, direction, out hit, Quaternion.identity, windMaxArea) && hit.collider.isTrigger == false)
            {
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    // プレイヤーに当たった場合
                    CharacterMaster.Instance.Player.WindAction(direction, transform.position + direction * (windMaxArea + blowAwayArea), windPower);
                    coroutine = StartCoroutine(IntervalCoroutine());
                }
            }
        }
    }

    /// <summary>
    /// 風のインターバルコルーチン
    /// </summary>
    /// <returns></returns>
    private IEnumerator IntervalCoroutine()
    {
        float time = 0;

        while(time < blowAwayArea / windPower)
        {
            if(PlayState.playState.gameMode == PlayState.GameMode.Play)
            {
                time += Time.deltaTime;
            }
            yield return null;
        }

        coroutine = null;
    }

    /// <summary>
    /// 風エフェクトの制御
    /// </summary>
    /// <param name="mode">ゲームモード</param>
    /// <param name="activeFlag">風の有効状態</param>
    private void WindEffectControl(PlayState.GameMode mode, bool activeFlag)
    {
        if (activeFlag)
        {
            if(mode != PlayState.GameMode.Pause)
            {
                if(windEffect.isPlaying == false)
                {
                    windEffect.Play();
                }
            }
            else
            {
                if (windEffect.isPlaying)
                {
                    windEffect.Pause();
                }
            }
        }
        else
        {
            if(windEffect.isStopped == false) { windEffect.Stop(); }
        }
    }
}
