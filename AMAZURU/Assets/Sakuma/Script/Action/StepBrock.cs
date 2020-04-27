using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 段差ブロックを管理するクラス
/// </summary>
public class StepBrock : MonoBehaviour
{

    [Header("設定項目")]

    //アニメの全体の再生時間
    [SerializeField]
    float stepAnimeSpan = 0;
    //アニメの前後の余白
    [SerializeField]
    float stepAnimeBlank = 0;
    //ジャンプ上昇量
    [SerializeField]
    float jump = 0;

    //段差ブロック全体で共有する段差アニメーションのフラグ
    static public bool stepAnime=false;


    //private

    //段差に乗っているかどうかのフラグ
    bool onStep=false;
    //アニメ再生中かどうかのフラグ
    bool stepAnimeFlg = false;
    //アニメ再生からの経過時間
    float animeTime=0;
    //アニメの再生段階
    int stepAnimeStep = 0;
    //アニメのプレイヤーが移動する時間
    float animeMoveTime = 0;
    //プレイヤーのトランスフォーム
    Transform playerTransform;
    //プレイヤーのキャラコン
    CharacterController character;
    //アニメ再生時のプレイヤーの座標
    Vector3 PlayerStartPos=Vector3.zero;
    //アニメ終了後のプレイヤーの座標
    Vector3 PlayerEndPos = Vector3.zero;
    //アニメ再生時のプレイヤーの座標
    float PlayerStartAngle = 0;
    //アニメ終了後のプレイヤーの座標
    float PlayerEndAngle = 0;

    void Start()
    {
        
    }
    
    void Update()
    {
        //段差アニメ開始時の初期化
        if(Input.GetKeyDown (KeyCode.Space )&&onStep && !StepBrock.stepAnime)
        {
            onStep = false;
            stepAnimeFlg = true;
            StepBrock.stepAnime = true;
            animeTime = 0;
            stepAnimeStep = 0;
            animeMoveTime = stepAnimeSpan - (stepAnimeBlank * 2);
            PlayerStartPos = playerTransform.position;
            character = playerTransform.gameObject.GetComponent<CharacterController >();
            character.enabled =false ;
            PlayerEndPos = new Vector3(
                Mathf.Cos(Mathf.Deg2Rad*(-transform.eulerAngles.y + 90)) +transform.parent.transform.position.x,
                PlayerStartPos.y-1f,
                Mathf.Sin(Mathf.Deg2Rad * (-transform.eulerAngles.y+90)) + transform.parent.transform.position.z
            );
            PlayerStartAngle= playerTransform.eulerAngles .y;
            PlayerEndAngle = transform.eulerAngles.y;
            playerTransform.gameObject.GetComponent<PlayerType2>().CliffFlag = true;
            
        }

        //アニメ再生時
        if (stepAnimeFlg)
        {
            animeTime += Time.deltaTime;

            //アニメーションの挙動を段階分け
            switch (stepAnimeStep)
            {
                case 0:

                    //旋回処理
                    playerTransform.eulerAngles = new Vector3(
                        playerTransform.eulerAngles.x,
                        Mathf.LerpAngle (PlayerStartAngle, PlayerEndAngle,(animeTime/ stepAnimeBlank)),
                        playerTransform.eulerAngles.z
                        );
                    if (animeTime >stepAnimeBlank)
                    {
                        stepAnimeStep += 1;
                    }
                    break;
                case 1:

                    //移動処理
                    Vector3 MovePos= Vector3.Lerp(PlayerStartPos, PlayerEndPos, (animeTime - stepAnimeBlank) / (animeMoveTime));
                    float jumpLate = (((animeTime - stepAnimeBlank) / (animeMoveTime)) * 2) - 1;
                    Vector3 jumpPos = new Vector3(0,((Mathf.Pow( jumpLate,2)*-1)+1)*jump, 0);
                    playerTransform.position = MovePos + jumpPos;
                    if (animeTime > stepAnimeSpan - stepAnimeBlank)
                    {
                        playerTransform.position = PlayerEndPos;
                        stepAnimeStep += 1;
                        
                    }
                    break;
                case 2:
                    if (animeTime > stepAnimeSpan)
                    {

                        //アニメ再生終了時処理
                        stepAnimeFlg = false;
                        StepBrock.stepAnime = false;
                        character.enabled = true;
                        playerTransform.gameObject.GetComponent<PlayerType2>().CliffFlag = false ;
                    }
                    break;
            }




        }




        Debug.Log(onStep);
    }




    //接触判定取得
    private void OnTriggerEnter(Collider other)
    {
        //プレイヤー接触時
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            onStep = true;
            playerTransform = other.gameObject.transform;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (LayerMask.LayerToName(other.gameObject.layer) == "Player")
        {
            onStep = false;
        }
    }
    
}
