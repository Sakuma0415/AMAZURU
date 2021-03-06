﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController1: MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのRigidbody")] private Rigidbody rb = null;
    [SerializeField, Header("プレイヤーの移動速度"), Range(0, 5)] private float playerSpeed = 0;
    [SerializeField, Header("移動時の起点カメラ")] private Camera playerCamera = null;
    [SerializeField, Header("RayのLayerMask")] private LayerMask layerMask;
    [SerializeField, Header("Rayの長さ"), Range(0, 3)] private float rayLength = 0.5f;
    [SerializeField, Header("足の位置"), Range(0, 1)] private float footHeight = 0;
    public Camera PlayerCamera { set { playerCamera = value; } }

    // コントローラーの入力値
    private float inputX = 0;
    private float inputZ = 0;

    // Y軸方向
    private float Yangle = 90;

    /// <summary>
    /// 初期化
    /// </summary>
    private void PlayerInit()
    {
        if(playerCamera == null) { playerCamera = Camera.main; }
    }

    /// <summary>
    /// コントローラ入力を取得
    /// </summary>
    private void GetInputController()
    {
        bool F = Input.GetKey(KeyCode.W);
        bool R = Input.GetKey(KeyCode.D);
        bool L = Input.GetKey(KeyCode.A);
        bool B = Input.GetKey(KeyCode.S);

        Vector2 fsAngle = new Vector2(0, 0);
        if (F) { fsAngle += new Vector2(0, 1); }
        if (R) { fsAngle += new Vector2(1, 0); }
        if (L) { fsAngle += new Vector2(-1, 0); }
        if (B) { fsAngle += new Vector2(0, -1); }
        // 入力値を取得
        inputX = fsAngle.x;
        inputZ = fsAngle.y;
        if (PlayState.playState.gameMode != PlayState.GameMode.Play)
        {
            inputX = 0;
            inputZ = 0;
        }
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove(bool fixedUpdate)
    {
        float delta = fixedUpdate ? Time.fixedDeltaTime : Time.deltaTime;

        // 入力の最低許容値
        float inputMin = 0.1f;

        // プレイヤーカメラの向いているY軸方向
        float playerCameraAngle = -playerCamera.transform.eulerAngles.y;

        // 入力を検知したら実行
        if(Mathf.Abs(inputX) > inputMin || Mathf.Abs(inputZ) > inputMin)
        {
            // 向きをプレイヤーカメラから見た入力方向へ修正
            float refAngle = 0;
            Yangle = Mathf.SmoothDampAngle(Yangle, Mathf.Atan2(inputZ, inputX) * Mathf.Rad2Deg + playerCameraAngle, ref refAngle, 0.05f);

            // カメラの向いている方向を取得
            Vector3 cameraForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

            // プレイヤーカメラ起点の入力方向
            Vector3 direction = cameraForward * inputZ + playerCamera.transform.right * inputX;

            // 入力方向を向く処理
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
            rot = Quaternion.Slerp(transform.rotation, rot, 15 * delta);
            transform.rotation = rot;

            // Rayを飛ばして進めるかをチェック
            float angleLate = 1;
            float forwardAngle = Yangle;
            Ray playerAround = new Ray(transform.position + new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), 0, Mathf.Sin(forwardAngle * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
            if (Physics.Raycast(playerAround, rayLength, layerMask) == false)
            {
                angleLate = 0;
                for (float f = 0; f < 90; f += 10)
                {
                    bool flag = false;
                    playerAround = new Ray(transform.position + new Vector3(Mathf.Cos((f + Yangle) * Mathf.Deg2Rad), 0, Mathf.Sin((f + Yangle) * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
                    if (Physics.Raycast(playerAround, rayLength, layerMask))
                    {
                        forwardAngle += f;
                        flag = true;
                    }
                    playerAround = new Ray(transform.position + new Vector3(Mathf.Cos((Yangle - f) * Mathf.Deg2Rad), 0, Mathf.Sin((Yangle - f) * Mathf.Deg2Rad)) * playerSpeed * delta, Vector3.down);
                    if (Physics.Raycast(playerAround, rayLength, layerMask))
                    {
                        forwardAngle -= f;
                        flag = true;
                    }
                    if (flag)
                    {
                        angleLate = Mathf.Cos(f * Mathf.Deg2Rad);
                        break;
                    }
                }
            }

            // 坂道にRayを飛ばして進めるかチェック
            float Xangle = 0;
            Ray playerForward = new Ray(transform.position + Vector3.down * footHeight, new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad), 0, Mathf.Sin(forwardAngle * Mathf.Deg2Rad)));
            if (Physics.Raycast(playerForward, angleLate * playerSpeed * delta, layerMask))
            {
                for (float f = 0; f < 90; f += 5)
                {
                    playerForward = new Ray(transform.position + Vector3.down * footHeight, new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad), Mathf.Sin(f * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad)));
                    if (Physics.Raycast(playerForward, angleLate * playerSpeed * delta, layerMask) == false)
                    {
                        Xangle = f;
                        break;
                    }
                }
            }
            else
            {
                for (float f = 0; f > -90; f -= 5)
                {
                    playerForward = new Ray(transform.position + Vector3.down * footHeight, new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad), Mathf.Sin(f * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(f * Mathf.Deg2Rad)));
                    if (Physics.Raycast(playerForward, angleLate * playerSpeed * delta, layerMask))
                    {
                        Xangle = f + 5;
                        break;
                    }
                }
            }

            // Rayを飛ばしてプレイヤーの移動先座標を決定する
            Vector3 movePosition;
            Ray playerFoot = new Ray(transform.position + new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad)) * playerSpeed * delta * angleLate, Vector3.down);
            if (Physics.Raycast(playerFoot, Mathf.Sin(Xangle * Mathf.Deg2Rad) * playerSpeed * delta * angleLate, layerMask))
            {
                movePosition = new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), 0, Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad));
            }
            else
            {
                movePosition = new Vector3(Mathf.Cos(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad), Mathf.Sin(Xangle * Mathf.Deg2Rad), Mathf.Sin(forwardAngle * Mathf.Deg2Rad) * Mathf.Cos(Xangle * Mathf.Deg2Rad));
            }

            // プレイヤーを移動させる
            rb.position += movePosition * playerSpeed * delta * angleLate;
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        PlayerInit();
    }

    // Update is called once per frame
    void Update()
    {

            GetInputController();
        
    }

    /// <summary>
    /// Rigidbody系の処理
    /// </summary>
    private void FixedUpdate()
    {
        PlayerMove(true);
    }

    /// <summary>
    /// Editor上で初期化
    /// </summary>
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
}
