using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField, Tooltip("判定用のLayerMask")] private LayerMask layerMask;

    [SerializeField, Header("風の有効範囲(マス)"), Range(1, 5)] private int windMaxArea = 1;
    [SerializeField, Header("風の発生する間隔(秒)"), Range(1, 10)] private float windInterval = 1.0f;
    [SerializeField, Header("風の持続時間(秒)"), Range(1, 10)] private float windDuration = 1.0f;
    [SerializeField, Header("風圧"), Range(1, 10)] private float windPower = 1.0f;

    [SerializeField, Header("正面")] private bool forward = true;
    [SerializeField, Header("背面")] private bool back = false;
    [SerializeField, Header("右")] private bool right = false;
    [SerializeField, Header("左")] private bool left = false;

    private Coroutine coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        WindInit();
    }

    // Update is called once per frame
    void Update()
    {
        if(PlayState.playState.gameMode == PlayState.GameMode.Play && coroutine == null)
        {
            ShotWind();
        }
    }

    /// <summary>
    /// 初期化と風を飛ばせる範囲を設定する
    /// </summary>
    private void WindInit()
    {
        windMaxArea = Mathf.Min(Mathf.Max(1, windMaxArea), 5);
    }

    /// <summary>
    /// 風を出す処理
    /// </summary>
    private void ShotWind()
    {
        // 対象オブジェクトに風が当たったかをチェック
        if (forward)
        {
            CreateWind(Vector3.forward);
        }

        if (back)
        {
            CreateWind(Vector3.back);
        }

        if (right)
        {
            CreateWind(Vector3.right);
        }

        if (left)
        {
            CreateWind(Vector3.left);
        }
        
    }

    
    private void HitWind(Vector3 direction)
    {
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(WindActionCoroutine(direction));
    }

    /// <summary>
    /// 風を生成
    /// </summary>
    /// <param name="direction">風向き</param>
    private void CreateWind(Vector3 direction)
    {
        if (Physics.BoxCast(transform.position, Vector3.one * 0.25f, direction, out RaycastHit hit, transform.rotation, windMaxArea))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                // プレイヤーに当たった場合
                HitWind(direction);
            }
        }
    }

    private IEnumerator WindActionCoroutine(Vector3 direction)
    {
        CharacterMaster.Instance.Player.IsWind = true;
        CharacterMaster.Instance.Player.SpecialMoveDirection = direction * windPower;

        float time = 0;
        while(time < windDuration)
        {
            time += Time.deltaTime;
            yield return null;
        }

        CharacterMaster.Instance.Player.IsWind = false;
        coroutine = null;
    }
}
