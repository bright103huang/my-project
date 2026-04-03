using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 1440f;
    public float rotationOffset = 180f; 

    private Rigidbody2D rb;
    private Animator animator;
    private bool isMoving = false;

    public ActionRequester actionRequester;
    public ActionExecutor actionExecutor; 

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        // 强制禁用根运动，防止动画文件中的位移数据将角色拉回原点
        if (animator != null) animator.applyRootMotion = false;

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (actionRequester == null) actionRequester = GetComponent<ActionRequester>();
        if (actionRequester == null) actionRequester = GetComponentInChildren<ActionRequester>();
        if (actionExecutor == null) actionExecutor = FindObjectOfType<ActionExecutor>();
    }

    void Update()
    {
        HandleInput();
        
        // 只有当没有其他特殊动作（如撞树、休息、抽搐）在执行时，才处理移动动画
        if (actionExecutor != null && !actionExecutor.IsBusy())
        {
            if (isMoving)
            {
                // 如果当前没有在播放 Walk 动画，再请求，防止每帧请求
                if (animator != null && !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    actionRequester.Request("Walk");
                }
            }
            else
            {
                // 如果停止移动，且当前在播放 Walk，则切回 Idle
                if (animator != null && animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
                {
                    animator.Play("Idle");
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            actionRequester.Request("HitTree");
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            actionRequester.Request("Rest");
        }
    }

    void HandleInput()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(h, v).normalized;
        rb.velocity = moveDir * speed;

        isMoving = moveDir.magnitude > 0.1f;

        // --- 全方位面部跟随 ---
        if (isMoving)
        {
            // 标准角度计算
            float angle = Mathf.Atan2(moveDir.x, moveDir.y) * Mathf.Rad2Deg;
            
            // 使用偏移量，默认 180。如果脸还是反的，请在 Inspector 里把 rotationOffset 改为 0
            float targetAngle = rotationOffset + angle;
            
            Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
