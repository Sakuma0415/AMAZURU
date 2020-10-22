using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemainsScaffold : MonoBehaviour
{
    // ステージの水面情報
    private WaterHi waterHi = null;

    // このオブジェクトのコライダー情報
    private BoxCollider boxCollider = null;

    [SerializeField, Tooltip("地面取得用のLayerMask")] private LayerMask groundLayerMask;

    private PlayState.GameMode gameMode = PlayState.GameMode.Play;

    private Vector3 minPosition = Vector3.zero;
    private Vector3 maxPosition = Vector3.zero;
    private Vector3 waterPosition = Vector3.zero;

    private bool isFall = false;
    private bool isLock = false;

    [SerializeField, Tooltip("水中フラグ")] private bool isInWater = false;
    /// <summary>
    /// 足場が水に浸かっているときのフラグ
    /// </summary>
    public bool IsInWater { get { return isInWater; } }

    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        try
        {
            gameMode = PlayState.playState.gameMode;
        }
        catch
        {
            gameMode = PlayState.GameMode.Play;
        }

        if (waterHi != null)
        {
            // 足場が移動できる範囲を取得
            Ray ray = new Ray(transform.position, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 200f, groundLayerMask))
            {
                minPosition = hit.point + Vector3.up * boxCollider.size.y * 0.5f;
            }
            else
            {
                minPosition = new Vector3(transform.position.x, boxCollider.size.y * 0.5f, transform.position.z);
            }

            waterPosition = new Vector3(transform.position.x, waterHi.max - (boxCollider.size.y * 0.5f - 0.06f), transform.position.z);

            ray = new Ray(transform.position, Vector3.up);
            if (Physics.Raycast(ray, out hit, 200f, groundLayerMask))
            {
                maxPosition = hit.point + Vector3.down * boxCollider.size.y * 0.5f;
            }
            else
            {
                maxPosition = new Vector3(transform.position.x, StageMake.LoadStageData.stageSize.y + boxCollider.size.y * 0.5f, transform.position.z);
            }

            isLock = gameMode == PlayState.GameMode.RotationPot;
            isFall = PlayState.playState.IsFallBox && isLock;
            

            if (isFall)
            {
                Vector3 target = minPosition.y > waterPosition.y ? minPosition : waterPosition;
                if (Vector3.Distance(transform.position, target) > 0.1f)
                {
                    if (gameMode != PlayState.GameMode.Pause)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, target, 5.0f * Time.deltaTime);
                    }
                }
                else
                {
                    isFall = false;
                    transform.position = target;
                }
            }
            else
            {
                if(isLock == false)
                {
                    if (minPosition.y + (boxCollider.size.y * 0.5f - 0.06f) > waterHi.max)
                    {
                        transform.position = minPosition;
                    }
                    else if (maxPosition.y + (boxCollider.size.y * 0.5f - 0.06f) < waterHi.max)
                    {
                        transform.position = maxPosition;
                    }
                    else
                    {
                        transform.position = new Vector3(transform.position.x, waterHi.max - (boxCollider.size.y * 0.5f - 0.06f), transform.position.z);
                    }
                }
            }
        }
        else
        {
            try
            {
                waterHi = Progress.progress.waterHi;
            }catch
            {

            }
        }

        isInWater = waterHi != null && transform.position.y < waterHi.max;
    }
}
