using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [SerializeField, Tooltip("PlayerのRigidbody")] private Rigidbody rb = null;
    [SerializeField, Header("移動速度"), Range(1, 10)] private float playerSpeed = 1.0f;
    [SerializeField, Header("移動時の起点カメラ")] private Camera playerCamera = null;
    public Camera PlayerCamera { set { playerCamera = value; } }

    [SerializeField] private Vector3 playerVec = Vector3.zero;
    private float inputX = 0;
    private float inputZ = 0;

    // 自分の四方に床が存在するか
    int layerMask = 0;
    private bool forwardGround = false;
    private bool backGround = false;
    private bool rightGround = false;
    private bool leftGround = false;

    [SerializeField, Tooltip("透明な壁オブジェクト")] private GameObject hideWallObject = null;
    private BoxCollider wallForward = null;
    private BoxCollider wallBack = null;
    private BoxCollider wallRight = null;
    private BoxCollider wallLeft = null;

    // 目の前に壁が存在するか
    private bool findWallForward = false;

    /// <summary>
    /// 初期化
    /// </summary>
    private void PlayerInit()
    {
        if(playerCamera == null) { playerCamera = Camera.main; }
        layerMask = ~(1 << LayerMask.NameToLayer("Mirror"));

        wallForward = Instantiate(hideWallObject, transform.position, Quaternion.identity).GetComponent<BoxCollider>();
        wallBack = Instantiate(hideWallObject, transform.position, Quaternion.identity).GetComponent<BoxCollider>();
        wallRight = Instantiate(hideWallObject, transform.position, Quaternion.identity).GetComponent<BoxCollider>();
        wallLeft = Instantiate(hideWallObject, transform.position, Quaternion.identity).GetComponent<BoxCollider>();
    }

    /// <summary>
    /// 移動を管理する
    /// </summary>
    private void PlayerMoveControl()
    {
        // 入力値を取得
        inputX = Input.GetAxis("Horizontal");
        inputZ = Input.GetAxis("Vertical");
    }

    /// <summary>
    /// プレイヤーの移動処理
    /// </summary>
    private void PlayerMove()
    {
        // カメラの向いている方向を取得
        Vector3 cameraForward = Vector3.Scale(playerCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // プレイヤーカメラ起点の入力方向
        Vector3 direction = cameraForward * inputZ + playerCamera.transform.right * inputX;

        // 入力方向を向く処理
        if (direction != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(direction, Vector3.up);
            rot = Quaternion.Slerp(transform.rotation, rot, 10.0f * Time.fixedDeltaTime);
            transform.rotation = rot;
        }

        // 床が存在するかをRayでチェック
        RaycastHit hit;
        if (Physics.SphereCast(new Ray(transform.position, -transform.up), 0.1f, out hit))
        {
            float radius = 0.1f;
            forwardGround = false;
            backGround = false;
            rightGround = false;
            leftGround = false;

            //forwardGround = Physics.SphereCast(new Ray(transform.position + Vector3.forward + transform.up * hit.distance, -transform.up), radius, hit.distance * 2.5f, layerMask);
            //backGround = Physics.SphereCast(new Ray(transform.position + Vector3.back + transform.up * hit.distance, -transform.up), radius, hit.distance * 2.5f, layerMask);
            //rightGround = Physics.SphereCast(new Ray(transform.position + Vector3.right + transform.up * hit.distance, -transform.up), radius, hit.distance * 2.5f, layerMask);
            //leftGround = Physics.SphereCast(new Ray(transform.position + Vector3.left + transform.up * hit.distance, -transform.up), radius, hit.distance * 2.5f, layerMask);

            foreach(RaycastHit raycast in Physics.SphereCastAll(new Ray(transform.position + Vector3.forward + transform.up * hit.distance, -transform.up), radius, hit.distance * 3.5f, layerMask))
            {
                if(raycast.transform.gameObject != wallForward.gameObject)
                {
                    forwardGround = true;
                    break;
                }
            }

            foreach (RaycastHit raycast in Physics.SphereCastAll(new Ray(transform.position + Vector3.back + transform.up * hit.distance, -transform.up), radius, hit.distance * 3.5f, layerMask))
            {
                if (raycast.transform.gameObject != wallBack.gameObject)
                {
                    backGround = true;
                    break;
                }
            }

            foreach (RaycastHit raycast in Physics.SphereCastAll(new Ray(transform.position + Vector3.right + transform.up * hit.distance, -transform.up), radius, hit.distance * 3.5f, layerMask))
            {
                if (raycast.transform.gameObject != wallRight.gameObject)
                {
                    rightGround = true;
                    break;
                }
            }

            foreach (RaycastHit raycast in Physics.SphereCastAll(new Ray(transform.position + Vector3.left + transform.up * hit.distance, -transform.up), radius, hit.distance * 3.5f, layerMask))
            {
                if (raycast.transform.gameObject != wallLeft.gameObject)
                {
                    leftGround = true;
                    break;
                }
            }

            if (forwardGround == false)
            {
                wallForward.transform.position = new Vector3(transform.position.x, transform.position.y, wallForward.transform.position.z);
                wallForward.size = transform.localScale;
                wallForward.gameObject.SetActive(true);
            }
            else
            {
                wallForward.transform.position = transform.position + Vector3.forward * 1.2f;
                wallForward.gameObject.SetActive(false);
            }

            if (backGround == false)
            {
                wallBack.transform.position = new Vector3(transform.position.x, transform.position.y, wallBack.transform.position.z);
                wallBack.size = transform.localScale;
                wallBack.gameObject.SetActive(true);
            }
            else
            {
                wallBack.transform.position = transform.position + Vector3.back * 1.2f;
                wallBack.gameObject.SetActive(false);
            }

            if (rightGround == false)
            {
                wallRight.transform.position = new Vector3(wallRight.transform.position.x, transform.position.y, transform.position.z);
                wallRight.size = transform.localScale;
                wallRight.gameObject.SetActive(true);
            }
            else
            {
                wallRight.transform.position = transform.position + Vector3.right * 1.2f;
                wallRight.gameObject.SetActive(false);
            }

            if (leftGround == false)
            {
                wallLeft.transform.position = new Vector3(wallLeft.transform.position.x, transform.position.y, transform.position.z);
                wallLeft.size = transform.localScale;
                wallLeft.gameObject.SetActive(true);
            }
            else
            {
                wallLeft.transform.position = transform.position + Vector3.left * 1.2f;
                wallLeft.gameObject.SetActive(false);
            }

            // 壁が存在するかRayでチェック
            findWallForward = Physics.BoxCast(transform.position, new Vector3(0.5f * transform.localScale.x, 0.5f * transform.localScale.y, 0.5f * transform.localScale.z) * 0.5f, transform.forward, transform.rotation, hit.distance * 0.5f);
        }

        // 移動処理
        if ((Mathf.Abs(inputX) > 0.1f || Mathf.Abs(inputZ) > 0.1f) && findWallForward == false)
        {
            float moveVec = Mathf.Abs(inputX) >= Mathf.Abs(inputZ) ? inputZ / inputX : inputX / inputZ;
            moveVec = 1.0f / Mathf.Sqrt(1.0f + moveVec * moveVec);
            playerVec = direction * moveVec;
            rb.position += playerVec * playerSpeed * Time.fixedDeltaTime;
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
        PlayerMoveControl();
    }

    /// <summary>
    /// Rigidbody系の処理
    /// </summary>
    private void FixedUpdate()
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
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
}
