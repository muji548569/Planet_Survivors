using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Transform planet;
    [Header("Move")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float rotateSpeed = 20f;
    [Header("Jump")]
    [SerializeField] private float gravityStrength = 20f;
    [SerializeField] private float jumpStrength = 6f;
    [SerializeField] private int maxJumpTimes = 1;
    [Header("GroundCheck")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private WeaponController weaponController;
    private Vector3 gravityDir;         // 重力方向 & 角色中心到星球中心的方向
    private Vector2 moveInput;          // 保存玩家輸入
    private int remainJumpTimes;
    private bool isGrounded;

    private void Awake()
    {
        // 關閉自動重力
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        weaponController = GetComponent<WeaponController>();

        inputActions = new PlayerInputActions();

        // 測試代碼
        weaponController.AddWeapon(new SwordWeapon(Resources.Load<WeaponData>("WeaponData/SwordWeapon"),this.transform));
        weaponController.AddWeapon(new FireballWeapon(Resources.Load<WeaponData>("WeaponData/FireballWeapon"), transform, planet));
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
        GroundCheck();
        ApplyGravity();
        RotateToPlanet();
        Move();
    }

    private void ApplyGravity()
    {
        // 計算重力方向
        gravityDir = (planet.position - transform.position).normalized;
        // 應用重力
        rb.AddForce(gravityDir * gravityStrength, ForceMode.Acceleration);
    }

    private void RotateToPlanet()
    {
        // 計算面朝方向
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, gravityDir);
        // 計算旋轉量
        Quaternion targetRotation = Quaternion.LookRotation(forward, -gravityDir);
        // 應用旋轉
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
    }

    private void Move()
    {
        // 計算移動方向
        Vector3 moveDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        // 計算水平方向的力
        Vector3 targetVelocity = moveDir * moveSpeed;
        // 計算垂直方向有的力
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

        // 在地面
        if (isGrounded)
        {
            remainJumpTimes = maxJumpTimes;
        }
        // 還有跳躍次數
        if (remainJumpTimes > 0)
        {
            // 清空垂直速度 避免力的疊加
            // 把目前速度投影到重力方向 取得現在垂直速度
            Vector3 verticalVelocity = Vector3.Project(rb.linearVelocity, gravityDir);
            // 清除垂直速度
            rb.linearVelocity -= verticalVelocity;
            // 施加往上的力
            rb.AddForce(-gravityDir * jumpStrength, ForceMode.Impulse);
            // 減少剩餘跳躍次數
            remainJumpTimes--;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheckPoint.position, groundCheckRadius, groundLayer);
    }
}
