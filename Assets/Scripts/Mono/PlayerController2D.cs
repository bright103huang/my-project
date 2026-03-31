using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float speed = 5f;

    private Rigidbody2D rb;

    // 👉 注入“请求入口”
    public ActionRequester actionRequester;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.F))
        {
            actionRequester.Request("HitTree");
        }

        if (Input.GetKey(KeyCode.R))
        {
            actionRequester.Request("Rest");
        }
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector2 moveDir = new Vector2(h, v).normalized;
        rb.velocity = moveDir * speed;

        // 👉 这里也可以改成请求（后面升级）
    }
}