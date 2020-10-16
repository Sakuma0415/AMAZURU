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
            if(gameMode != PlayState.GameMode.RotationPot)
            {
                Ray ray = new Ray(transform.position, Vector3.down);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 200f, groundLayerMask))
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

                if (isFall)
                {
                    Vector3 target = minPosition.y > waterPosition.y ? minPosition : waterPosition;
                    if (Vector3.Distance(transform.position, target) > 0.1f)
                    {
                        if(gameMode != PlayState.GameMode.Pause)
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
            else
            {
                isFall = true;
            }
        }
        else
        {
            waterHi = Progress.progress.waterHi;
        }
    }
}
