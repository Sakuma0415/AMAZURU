﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// カメラの管理クラス
/// </summary>
public class CameraPos : MonoBehaviour
{
    [Header ("設定項目")]
    //X-Z平面に垂直に交わる平面の注視点から見たカメラの角度
    [SerializeField ,Range(0, 90)]
    private float Yangle = 0;
    //注視点を画面下部にずらす量
    [SerializeField, Range(-10, 10)]
    float lookHi;
    //注視点変更に使う時間の長さ
    [SerializeField]
    float changeTime;
    //カメラが衝突を検知するマスク
    [SerializeField]
    LayerMask layerMask;
    //カメラのコライダー
    [SerializeField]
    SphereCollider sphereCollider;
    //カメラが接触判定をとるかどうかのフラグ
    [SerializeField]
    bool CameraColFlg;
    //プレイヤーの中心をずらす量
    [SerializeField]
    float LookHiSet = 0;
    //カメラ捜査の速度
    [SerializeField]
    float stickSpead = 0;
    //カメラの速度取得用
    [SerializeField]
    ResultControl resultControl;
    //カメラの速度ステージ注視時
    [SerializeField]
    float[] cameraSpS;
    //カメラの速度プレイヤー注視時
    [SerializeField]
    float[] cameraSpP;
    //ゲーム終了時のカメラの距離
    [SerializeField]
    float GameEndDis = 0;
    //ゲーム終了時の演出にかかる時間
    [SerializeField]
    float GameEndEfSpan = 0;

    [Header("以下変更不可")]
    //ステージ注視時のカメラ、ステージ間の距離
    public float CameraDisS;
    //プレイヤー注視時のカメラ、プレイヤー間の距離
    public float CameraDisP;
    //ステージの中心座標
    public Vector3 lookPos;
    //プレイヤーのトランスフォーム
    public Transform PlayerTransform;
    //ゲーム開始時のカメラの角度
    public float fAngle;
    //アメフラシの演出のカメラが移動する時間
    public float rainPotChangeAnimeTimeSpead;

    //private変数
    //現在のカメラ、注視点間の距離
    float CameraDis;
    //現在の注視点の座標
    Vector3 lookObj;
    //X-Z平面上の注視点から見たカメラの角度
    float XZangle = 0;
    //注視点　true=プレイヤー：false=ステージ
    bool lookMode = false;
    //注視点変更アニメーションの経過時間
    float lookAnimeTime = 0;
    //注視点変更アニメーションの開始地点の座標
    Vector3 animePos;
    //マウスを固定＆透明化させるかのフラグ
    bool MouseHack = true;
    //現在の注視点を画面下部にずらす量
    float newLookHi;
    //マウスからの情報を受け取るかどうかのフラグ
    bool MouseCheck = true;
    //カメラがステージに埋まった時のカメラ、注視点の距離
    float endCameraPos;
    //ゲーム開始時の定点ビューを再生するフラグ
    bool startCameraFlg = true;
    //開始時の定点ビューから開始時のカメラの角度へリセットさせるフラグ
    bool startCameraAngleResetFlg = true;
    //開始時の角度リセット時の角度を保存しておくバッファ
    float startCameraAngleResetBf = 0;
    //アメフラシの起動中のフラグ
    bool rainPotChange = false;
    //アメフラシの演出終了後にカメラが戻るステータスの格納先
    float beforeAngleY = 0;
    float beforeAngleXZ = 0;
    float beforeDis = 0;
    float beforeHi = 0;
    Vector3 beforePos;
    //アメフラシの演出の経過時間
    float potAnimeTime;
    //アメフラシの演出の入りか終わりか
    bool outflg = false;
    //アメフラシ起動時に移動する角度
    float lotAngle = 0;
    //ゲームオーバー、クリア時の瞬間を検知するためのシーケンサー
    PlayState.GameMode gameMode;
    //ゲームオーバー、クリア時の演出中かどうかのフラグ
    bool GameEndFlg = false;
    //ゲーム終了からの経過時間
    float EndAfterTime = 0;
    //ゲーム終了時のカメラの距離
    float disToGameEnd = 0;
    //ゲーム終了時の注視点の座標
    Vector3 transformToGameEnd = Vector3.zero;

    void Start()
    {
        //初期化
        startCameraFlg = true;
        XZangle = fAngle;
        CameraDis =lookMode ? CameraDisP: CameraDisS;
        MouseCheck = true;
        gameMode = PlayState.playState.gameMode;
    }

    private void LateUpdate()
    {

        //カメラの移動可能なゲームモード＆アメフラシの演出中ではない
        if ((PlayState.playState.gameMode == PlayState.GameMode.Play|| PlayState.playState.gameMode == PlayState.GameMode.StartEf ) || rainPotChange)
        {

            //注視点変更アニメーション中
            if (lookAnimeTime > 0)
            {
                transform.position = Vector3.Lerp(animePos, (new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad) + newLookHi, Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * (!lookMode ? CameraDisS : CameraDisP)) + lookObj, 1 - (lookAnimeTime/ changeTime));
            }
            else
            {

                //注視点がプレイヤー
                if (lookMode)
                {

                    //カメラがステージに埋まっているかどうかの処理
                    if (Physics.OverlapSphere((new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad) + newLookHi, Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * CameraDis) + lookObj, sphereCollider.radius, layerMask).Length == 0)
                    {
                        endCameraPos = CameraDis;
                    }
                    else
                    {
                        Ray ray = new Ray(PlayerTransform.position + new Vector3(0, LookHiSet, 0), Vector3.Normalize(transform.position - (PlayerTransform.position + new Vector3(0, LookHiSet, 0))));
                        RaycastHit hit;
                        if (Physics.SphereCast (ray, sphereCollider.radius, out hit, CameraDis, layerMask)&& CameraColFlg)
                        {
                            endCameraPos = Vector3.Distance(hit.point, PlayerTransform.position + new Vector3(0, LookHiSet, 0));
                        }
                        else
                        {
                            endCameraPos = CameraDis;
                        }

                    }

                }
                else
                {
                    endCameraPos = CameraDis;
                }

                //座標移動
                transform.position = (new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad) + newLookHi, Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * endCameraPos) + lookObj;
            }

            //角度変更
            transform.localEulerAngles = new Vector3(Yangle, -XZangle - 90, 0);
        }
        else if(GameEndFlg)
        {
            if (EndAfterTime < GameEndEfSpan)
            {
                transform.position = Vector3.Lerp(
                    transformToGameEnd,
                    (new Vector3(Mathf.Cos(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad), Mathf.Sin(Yangle * Mathf.Deg2Rad) + newLookHi, Mathf.Sin(XZangle * Mathf.Deg2Rad) * Mathf.Cos(Yangle * Mathf.Deg2Rad)) * GameEndDis) + PlayerTransform.position + new Vector3(0, LookHiSet, 0),
                    (EndAfterTime / GameEndEfSpan)
                );
                EndAfterTime += Time.deltaTime;
            }

        }

        //ゲームの終了を検知するシーケンサー
        if(gameMode != PlayState.playState.gameMode)
        {
            if (PlayState.playState.gameMode==PlayState.GameMode.GameOver || PlayState.playState.gameMode == PlayState.GameMode.Clear )
            {
                //ゲーム終了後の演出開始時の処理
                GameEndFlg = true;
                EndAfterTime = 0;
                transformToGameEnd = transform.position;
                if (lookAnimeTime > 0)
                {
                    disToGameEnd = !lookMode ? CameraDisS : CameraDisP;
                }
                else
                {
                    disToGameEnd = endCameraPos;
                }
            }
            gameMode = PlayState.playState.gameMode;
        }


    }
    

    void Update()
    {
        //ゲーム開始時の定点カメラの特殊挙動時のステータス更新
        if (startCameraFlg )
        {
            lookObj = lookMode ? PlayerTransform.position+new Vector3 (0,LookHiSet,0)  : lookPos;
            XZangle += Time.deltaTime*3;

            //通常のカメラ処理に戻る
            if (ControllerInput .Instance.buttonDown.circle && !SceneLoadManager.Instance.SceneLoadFlg && !PlayState.playState.Tutorial)
            {
                startCameraAngleResetBf = XZangle;
                startCameraFlg = false;
                PlayState.playState.gameMode = PlayState.GameMode.Play;
                lookMode = !lookMode;
                lookAnimeTime = changeTime;
                animePos = transform.position;
            }
        }

        //プレイ中の処理
        if (PlayState.playState.gameMode == PlayState.GameMode.Play)
        {

            //マウスカーソルの設定
            Cursor.visible = !MouseHack;

            //マウスカーソルの切り替え
            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                MouseHack = !MouseHack;
            }

            if (MouseHack)
            {
                if (MouseCheck)
                {
                    if (!lookMode)
                    {
                        switch (resultControl.SpeedType)
                        {
                            case CameraSpeed.Slow:
                                stickSpead = cameraSpS[0];
                                break;
                            case CameraSpeed.Nomal:
                                stickSpead = cameraSpS[1];
                                break;
                            case CameraSpeed.Quick:
                                stickSpead = cameraSpS[2];
                                break;
                        }
                    }
                    else
                    {
                        switch (resultControl.SpeedType)
                        {
                            case CameraSpeed.Slow:
                                stickSpead = cameraSpP[0];
                                break;
                            case CameraSpeed.Nomal:
                                stickSpead = cameraSpP[1];
                                break;
                            case CameraSpeed.Quick:
                                stickSpead = cameraSpP[2];
                                break;
                        }
                    }






                    //マウスの移動情報を角度の変更量に変換
                    float mouse_x_delta = Mathf.Abs(ControllerInput .Instance .stick .RStickHorizontal )<0.1f?0: ControllerInput.Instance.stick.LStickHorizontal * stickSpead * Time.deltaTime ;
                    float mouse_y_delta = Mathf.Abs(ControllerInput.Instance.stick.RStickVertical ) < 0.1f ? 0 : ControllerInput.Instance.stick.LStickVertical * stickSpead * Time.deltaTime;

                    XZangle -= mouse_x_delta ;
                    Yangle -= mouse_y_delta ;

                    if (Yangle > 85)
                    {
                        Yangle = 85;
                    }
                    if (Yangle < -85)
                    {
                        Yangle = -85;
                    }
                }
            }
            
            //注視点変更
            if (ControllerInput .Instance .buttonDown .triangle  && lookAnimeTime == 0&&!startCameraFlg )
            {
                lookMode = !lookMode;
                lookAnimeTime = changeTime;
                animePos = transform.position;
            }

            //注視点変更アニメーション中の処理
            if (lookAnimeTime > 0)
            {
                MouseCheck = false;
                lookAnimeTime -= Time.deltaTime;
                if (lookAnimeTime <= 0)
                {
                    lookAnimeTime = 0;
                    MouseCheck = true;
                }
                CameraDis = Mathf.Lerp(lookMode ? CameraDisS : CameraDisP, lookMode ? CameraDisP : CameraDisS, 1 - (lookAnimeTime/ changeTime));
                newLookHi = Mathf.Lerp(lookMode ? 0 : lookHi, lookMode ? lookHi  : 0, 1 - (lookAnimeTime / changeTime));

                //ゲーム開始時の定点から通常カメラに戻るときの要素設定
                if (startCameraAngleResetFlg)
                {
                    XZangle = Mathf.Lerp(startCameraAngleResetBf  , fAngle , 1 - (lookAnimeTime / changeTime));
                }
            }
            else
            {
                if(startCameraAngleResetFlg)
                {
                    startCameraAngleResetFlg = false;
                }
            }

            //注視点の座標を更新
            lookObj = lookMode ? PlayerTransform.position + new Vector3(0, LookHiSet, 0) : lookPos;

        }
        else
        {

            //プレイ中以外にはマウスの設定を初期化
            Cursor.visible = true ;
            Cursor.lockState = CursorLockMode.None;
        }

        //アメフラシの演出中のカメラの処理
        if (rainPotChange)
        {
            potAnimeTime += Time.deltaTime;

            if (outflg)
            {

                //カメラの情報を直接上書き
                if (potAnimeTime < rainPotChangeAnimeTimeSpead)
                {
                    XZangle = Mathf.LerpAngle(lotAngle, beforeAngleXZ, potAnimeTime/ rainPotChangeAnimeTimeSpead);
                    Yangle = Mathf.LerpAngle(35, beforeAngleY, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    CameraDis = Mathf.Lerp(25, beforeDis, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookObj = Vector3.Lerp(lookPos, beforePos , potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookHi = Mathf.Lerp( -0.1f, beforeHi, potAnimeTime / rainPotChangeAnimeTimeSpead);
                }
                else
                {
                    XZangle = Mathf.LerpAngle(lotAngle, beforeAngleXZ, 1);
                    Yangle = Mathf.LerpAngle(35, beforeAngleY, 1);
                    CameraDis = Mathf.Lerp(25, beforeDis, 1);
                    lookObj = Vector3.Lerp(lookPos, beforePos, 1);
                    lookHi = Mathf.Lerp(-0.1f, beforeHi, 1);
                    rainPotChange = false;
                }

            }
            else
            {

                //カメラの情報を直接上書き
                if (potAnimeTime < rainPotChangeAnimeTimeSpead)
                {
                    XZangle = Mathf.LerpAngle(beforeAngleXZ, lotAngle, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    Yangle = Mathf.LerpAngle(beforeAngleY, 35, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    CameraDis = Mathf.Lerp(beforeDis, 25, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookObj = Vector3.Lerp(beforePos , lookPos, potAnimeTime / rainPotChangeAnimeTimeSpead);
                    lookHi = Mathf.Lerp(beforeHi, -0.1f, potAnimeTime / rainPotChangeAnimeTimeSpead);
                }
                else
                {
                    XZangle = Mathf.LerpAngle(beforeAngleXZ, lotAngle, 1);
                    Yangle = Mathf.LerpAngle(beforeAngleY, 35, 1);
                    CameraDis = Mathf.Lerp(beforeDis, 25, 1);
                    lookObj = Vector3.Lerp(beforePos, lookPos, 1);
                    lookHi = Mathf.Lerp(beforeHi, -0.1f, 1);
                }
            }

        }
    }

    //カメラをアメフラシ起動の状態にする
    public void RainPotChange()
    {
        //  ステータスを初期化
        beforeHi = lookHi;
        rainPotChange = true;
        outflg = false;
        beforePos = lookObj;
        beforeAngleXZ = XZangle;
        beforeAngleY = Yangle;
        beforeDis = lookMode ? CameraDisP : CameraDisS;
        potAnimeTime = 0;
        float bfAngle = XZangle;

        //角度を0-360に
        while (bfAngle > 360)
        {
            bfAngle -= 360;
        }
        while (bfAngle < 0)
        {
            bfAngle += 360;
        }

        //アメフラシ起動中に向かう角度設定
        if(bfAngle < 90)
        {
            lotAngle = 45;
        }
        else if(bfAngle < 180)
        {
            lotAngle = 135;
        }
        else if (bfAngle < 270)
        {
            lotAngle = 225;
        }
        else
        {
            lotAngle = 315;
        }
    }

    //カメラをアメフラシ終了の状態にする
    public void RainPotChangeOut()
    {
        potAnimeTime = 0;
        outflg = true;
    }

}
