using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public Transform Planet;
    public float moveSpeed = 5.0f;
    public float rotateSpeed = 5.0f;
    public float gravityStrength = 1.0f;
    private Rigidbody rb;
    private PlayerInputActions inputActions;
    private Vector3 gravityDir;         // 重力方向 & 角色中心到星球中心的方向
    private Vector2 moveInput;          // 保存玩家輸入

    private void Awake()
    {
        // 關閉自動重力
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        inputActions = new PlayerInputActions();
    }
    private void OnEnable()
    {
        // 啟用InputSystem的Player的Action map
        inputActions.Player.Enable();
        // 註冊移動事件
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        // 停用Player事件表
        inputActions.Player.Disable();
        // 取消註冊移動事件
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        RotateToPlanet();
        Move();
    }

    private void ApplyGravity()
    {
        // 計算重力方向
        gravityDir = (Planet.position - transform.position).normalized;
        // 應用重力
        rb.AddForce(gravityDir * gravityStrength, ForceMode.Acceleration);
    }

    private void RotateToPlanet()
    {
        // 計算面朝方向
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, -gravityDir);
        // 應用旋轉
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotateSpeed * Time.fixedDeltaTime);
    }
    
    private void Move()
    {
        Vector3 moveDir = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        rb.MovePosition(transform.position + moveDir * moveSpeed * Time.fixedDeltaTime);
    }

    // 移動事件
    private void OnMove(InputAction.CallbackContext context)
    {
        // 紀錄玩家輸入
        moveInput = context.ReadValue<Vector2>();
    }

}
