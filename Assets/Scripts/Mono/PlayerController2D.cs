using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("基础设置")]
    public float speed = 5f;
    public Animator anim;
    public ActionExecutor executor;
    public StateRuntime runtime;
    public ActionDefinition hitTreeAction;
    public ActionDefinition restAction;

    private Rigidbody2D rb;
    private bool isLocked = false;
    private bool nearTree = false;
    private TreeVisual currentTree;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (transform.position.z != 0)
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        if (isLocked)
        {
            // 锁定期间强制清除所有移动速度和行走参数
            rb.velocity = Vector2.zero;
            if (anim != null) anim.SetBool("isWalking", false);

            HandleRecoveryInput();
            return;
        }

        HandleMovementPhysics();
        HandleInteraction();
    }

    void HandleMovementPhysics()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(h, v).normalized * speed;

        if (anim != null) anim.SetBool("isWalking", rb.velocity.magnitude > 0.1f);

        if (h < 0) transform.localScale = new Vector3(-1, 1, 1);
        else if (h > 0) transform.localScale = new Vector3(1, 1, 1);
    }

    void HandleInteraction()
    {
        if (Input.GetKeyDown(KeyCode.F) && nearTree)
        {
            if (anim != null) anim.SetTrigger("HitTree");
            if (currentTree != null) currentTree.Shake();
            if (executor != null) executor.Execute(hitTreeAction);
        }
    }

    void HandleRecoveryInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (anim != null) anim.SetTrigger("Rest");
            if (executor != null) executor.Execute(restAction);

            if (runtime.Get("Fatigue") < 50)
            {
                isLocked = false;
                Debug.Log("【恢复】锁定解除。");
            }
        }
    }

    public void LockPlayer(string message)
    {
        if (isLocked) return;
        isLocked = true;

        // 核心：先停掉物理，再发信号
        rb.velocity = Vector2.zero;
        if (anim != null)
        {
            anim.SetBool("isWalking", false); // 关掉走路开关
            anim.SetTrigger("Convulse");      // 触发抽搐动作
        }

        Debug.Log("【系统】疲劳度过高，发送 Convulse 信号！");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            nearTree = true;
            currentTree = other.GetComponent<TreeVisual>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            nearTree = false;
            currentTree = null;
        }
    }
}
