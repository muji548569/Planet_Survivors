using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform planet;
    [SerializeField] private Transform cameraTransform;
    [Header("Move")]
    [SerializeField] private float rotateSpeed = 20f;
    [Header("Jump")]
    [SerializeField] private float gravityStrength = 20f;
    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [Header("Animation")]
    [SerializeField] private Animator animator;
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpHash = Animator.StringToHash("Jump");

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    
    private Vector3 gravityDir;         // 重力方向 & 角色中心到星球中心的方向
    private Vector2 moveInput;          // 保存玩家輸入
    private int remainJumpTimes;
    private bool wasGrounded;
    private bool isGrounded;
    [SerializeField] private PlayerData playerData;
    private bool isInitialized;

    private void Awake()
    {
        // 關閉自動重力
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        inputActions = new PlayerInputActions();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
            Debug.Log("[PlayerController] 找不到 Main Camera。");
        }
        if(planet == null)
        {
            Debug.LogError("[PlayerController] Planet 尚未指定。");
        }
        if (groundCheckPoint == null)
        {
            Debug.LogError("[PlayerController] GroundCheckPoint 尚未指定。");
        }

    }

    public void Init(PlayerData data)
    {
        if (data == null)
        {
            Debug.LogError("[PlayerController] PlayerData 不可為 null。");
            return;
        }
        playerData = data;
        remainJumpTimes = playerData.Stat.maxJumpTimes;
        isInitialized = true;   
    }

    private void OnEnable()
    {
        // 啟用InputSystem的Player的Action map
        inputActions.Player.Enable();
        // 註冊移動事件
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
        // 註冊跳躍事件
        inputActions.Player.Jump.performed += OnJump;
    }

    private void OnDisable()
    {
        // 取消註冊移動事件
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
        // 取消註冊跳躍事件
        inputActions.Player.Jump.performed -= OnJump;

        // 停用Player事件表
        inputActions.Player.Disable();
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        UpdateGravityDir();
        GroundCheck();
        ApplyGravity();
        Move();
        RotateToPlanet();
    }

    private void Update()
    {
        if (!isInitialized) return;

        UpdateAnimation();
    }

    private void UpdateGravityDir()
    {
        // 計算重力方向
        gravityDir = (planet.position - transform.position).normalized;
    }

    private void ApplyGravity()
    {
        // 應用重力
        rb.AddForce(gravityDir * gravityStrength, ForceMode.Acceleration);
    }

    /// <summary>
    /// 根據攝像機相對位置獲取移動方向
    /// </summary>
    /// <returns></returns>
    private Vector3 GetCameraRelativeMoveDirection()
    {
        if (cameraTransform == null)
        {
            return Vector3.zero;
        }

        Vector3 planetUp = -gravityDir;

        // 將攝像機方向投影到角色所在地的星球切線平面
        Vector3 cameraForward = Vector3.ProjectOnPlane(cameraTransform.forward, planetUp);
        if (cameraForward.sqrMagnitude < 0.001f)
        {
            cameraForward = Vector3.ProjectOnPlane(transform.forward, planetUp).normalized;
        }
        if (cameraForward.sqrMagnitude < 0.001f)
        {
            cameraForward = Vector3.zero;
        }
        cameraForward.Normalize();

        // 由目前球面 Up 和畫面 Forward 建立穩定的右方向
        Vector3 cameraRight = Vector3.Cross(planetUp, cameraForward).normalized;

        // 根據輸入組合移動方向
        Vector3 moveDir = cameraForward * moveInput.y + cameraRight * moveInput.x;

        return moveDir.normalized;
    }

    private void RotateToPlanet()
    {
        Vector3 planetUp = -gravityDir;
        Vector3 moveDir = GetCameraRelativeMoveDirection();
        
        Vector3 targetForward;
        if (moveDir.sqrMagnitude > 0.001f)
        {
            // 有輸入時朝移動方向旋轉
            targetForward = moveDir;
        }
        else
        {
            // 沒有輸入時維持目前方向，但仍貼合星球表面
            targetForward = Vector3.ProjectOnPlane(transform.forward, planetUp).normalized;
        }

        if(targetForward.sqrMagnitude < 0.001f)
            return;

        // 計算旋轉量
        Quaternion targetRotation = Quaternion.LookRotation(targetForward, planetUp);
        // 應用旋轉
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime));
    }

    private void Move()
    {
        // 計算移動方向
        Vector3 moveDir = GetCameraRelativeMoveDirection();
        // 計算水平方向的力
        Vector3 targetVelocity = moveDir * playerData.Stat.moveSpeed;
        // 保留朝向或遠離星球中心的垂直速度
        Vector3 verticalVelocity = Vector3.Project(rb.linearVelocity, gravityDir);
        // 剛體加力
        rb.linearVelocity = targetVelocity + verticalVelocity;
    }

    // 移動事件
    private void OnMove(InputAction.CallbackContext context)
    {
        // 紀錄玩家輸入
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Jump();
        }
    }

    private void Jump()
    {
        // 重新計算重力方向 避免按下跳躍時使用到上一幀或未更新的方向
        gravityDir = (planet.position - transform.position).normalized;

        if (remainJumpTimes <= 0)
            return;

        // 清空垂直速度 避免力的疊加
        // 把目前速度投影到重力方向 取得現在垂直速度
        Vector3 verticalVelocity = Vector3.Project(rb.linearVelocity, gravityDir);
        // 清除垂直速度
        rb.linearVelocity -= verticalVelocity;

        // 施加往上的力
        rb.AddForce(-gravityDir * playerData.Stat.jumpStrength, ForceMode.Impulse);

        // 減少剩餘跳躍次數
        remainJumpTimes--;

        // 跳躍動畫條件觸發
        if (animator != null)
        {
            animator.SetTrigger(JumpHash);
        }
    }

    private void GroundCheck()
    {
        wasGrounded = isGrounded;

        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer, QueryTriggerInteraction.Ignore);

        if (!isGrounded && wasGrounded)
        {
            Debug.Log("離開地面");
        }

        if (isGrounded && !wasGrounded)
        {
            remainJumpTimes = playerData.Stat.maxJumpTimes;
            Debug.Log($"落地，重置跳躍次數：{remainJumpTimes}");
        }
    }

    /// <summary>
    /// 畫出地面檢測範圍
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (groundCheckPoint == null)
        {
            return;
        }

        Gizmos.color = isGrounded ? Color.green : Color.red;

        Gizmos.DrawWireSphere(
            groundCheckPoint.position,
            groundCheckRadius
        );
    }

    private void UpdateAnimation()
    {
        if (animator == null) return;

        Vector3 planetUp = -gravityDir;

        Vector3 surfaceVelocity = Vector3.ProjectOnPlane(rb.linearVelocity, planetUp);

        // 如果 playerData.Stat.moveSpeed 等於 0 
        // normalizedSpeed 就會變成無限 所以需要做檢查
        float normalizedSpeed = 0;
        if (playerData.Stat.moveSpeed > 0f)
        {
            // 正規化速度
            normalizedSpeed = Mathf.Clamp01(surfaceVelocity.magnitude / playerData.Stat.moveSpeed);
        }
        else
        {
            normalizedSpeed = 0f;
        }

        // 參數1: 條件參數id
        // 參數2: 設置數值
        // 參數3: 阻尼時間
        // 參數4: 本次經過時間
        animator.SetFloat(SpeedHash, normalizedSpeed, 0.1f, Time.deltaTime);

        // 每幀更新地面狀態
        animator.SetBool(IsGroundedHash, isGrounded);
    }
}
