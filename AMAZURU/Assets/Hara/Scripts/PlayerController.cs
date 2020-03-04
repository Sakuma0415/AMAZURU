using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのRigidbody")] private Rigidbody rb = null;
    [SerializeField, Header("移動速度")] private float playerSpeed = 1.0f;

    private Vector3 lastPos;

    /// <summary>
    /// 初期化
    /// </summary>
    private void PlayerInit()
    {
        lastPos = new Vector3(transform.position.x, 0, transform.position.z);
    }

    /// <summary>
    /// 移動を管理する
    /// </summary>
    private void PlayerMove()
    {
        // 入力値を取得
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // 移動処理
        transform.position += new Vector3(moveX, 0, moveZ) * playerSpeed * Time.deltaTime;

        // 移動方向を向く処理
        Vector3 diff = new Vector3(transform.position.x, 0, transform.position.z) - lastPos;
        lastPos = new Vector3(transform.position.x, 0, transform.position.z);
        
        if(diff.magnitude > 0.01f)
        {
            transform.localRotation = Quaternion.LookRotation(diff);
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
        PlayerMove();
    }

    /// <summary>
    /// Editor上で初期化
    /// </summary>
    private void Reset()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
}
