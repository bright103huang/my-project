using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float speed = 5f;

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
                actionRequester.Request("Walk");
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

        // --- 修正转向逻辑：使用 Y 轴旋转 180 度 ---
        if (h > 0.1f)
        {
            // 向右移，旋转设为 0 度
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (h < -0.1f)
        {
            // 向左移，旋转设为 180 度
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }
}
