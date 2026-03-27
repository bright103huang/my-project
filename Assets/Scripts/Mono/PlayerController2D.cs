using UnityEngine;
using System.Collections;
using TMPro;
using System.Text;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("--- 基础设置 ---")]
    public float speed = 4f;
    public float shakeDelay = 0.2f;
    public float actionCooldown = 1.0f;
    public float fallYOffset = -0.75f;

    [Header("--- 资源引用 ---")]
    public TextMeshProUGUI statusText;
    public Animator anim;
    public Transform modelTransform;
    public ActionExecutor executor;
    public StateRuntime runtime;
    public ActionDefinition hitTreeAction;
    public ActionDefinition restAction;

    private Rigidbody2D rb;
    private bool isLocked = false;
    private bool isBusy = false;
    private bool nearTree = false;
    private TreeVisual currentTree;

    private string logMessage = "Standing still is the only safe move.";

    private const string HIT_TREE_TRIGGER = "HitTree";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (modelTransform == null && anim != null) modelTransform = anim.transform;
        if (statusText == null) statusText = FindObjectOfType<TextMeshProUGUI>();

        InitLayout();
    }

    void InitLayout()
    {
        if (statusText == null) return;

        statusText.enableAutoSizing = true;
        statusText.fontSizeMin = 10;
        statusText.fontSizeMax = 16;
        statusText.alignment = TextAlignmentOptions.TopLeft;
        statusText.enableWordWrapping = true;
        statusText.overflowMode = TextOverflowModes.Overflow;

        RectTransform textRt = statusText.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0, 1);
        textRt.anchorMax = new Vector2(0, 1);
        textRt.pivot = new Vector2(0, 1);

        textRt.sizeDelta = new Vector2(280, 600);
        textRt.anchoredPosition = new Vector2(100, -75);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        UpdateDisplay();

        if (isLocked)
        {
            rb.velocity = Vector2.zero;
            HandleRestLogic();
            return;
        }

        if (isBusy)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        HandleMovement();
        HandleAction();
    }

    // ⭐ 休息与恢复逻辑（无 WakeUp，直接回 Idle）
    void HandleRestLogic()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (executor != null && restAction != null)
                executor.Execute(restAction);

            if (anim != null)
                anim.SetTrigger("Rest");

            logMessage = "Standing still is the only safe move.";
        }

        // ⭐ 恢复判断
        if (runtime.Get("Fatigue") < 40)
        {
            isLocked = false;

            if (modelTransform != null)
                modelTransform.localPosition = Vector3.zero;

            // ⭐ 关键：强制回 Idle（没有 WakeUp）
            if (anim != null)
                anim.Play("Idle", 0, 0f);

            logMessage = "Back on your feet. Ready!";
        }
    }

    // ⭐ 抽搐锁定
    public void LockPlayer(string message)
    {
        if (isLocked) return;

        isLocked = true;
        isBusy = false;
        rb.velocity = Vector2.zero;

        if (anim != null)
        {
            anim.ResetTrigger("Convulse");
            anim.SetTrigger("Convulse");
        }

        if (modelTransform != null)
            modelTransform.localPosition = new Vector3(0, fallYOffset, 0);

        logMessage = "SYSTEM FAILURE: Press R to Rest.";
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(h, v).normalized * speed;

        if (anim != null)
            anim.SetBool("isWalking", rb.velocity.magnitude > 0.1f);

        if (h != 0)
            transform.localScale = new Vector3(Mathf.Sign(h), 1, 1);
    }

    void HandleAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Debug.Log($"F pressed | nearTree={nearTree} | isBusy={isBusy}");

            if (nearTree && !isBusy)
            {
                StartCoroutine(PerformHitRoutine());
            }
        }
    }

    IEnumerator PerformHitRoutine()
    {
        isBusy = true;

        if (anim != null)
            anim.SetTrigger(HIT_TREE_TRIGGER);

        yield return new WaitForSeconds(shakeDelay);

        if (currentTree != null)
            currentTree.Shake();

        if (executor != null)
            executor.Execute(hitTreeAction);

        yield return new WaitForSeconds(Mathf.Max(0.1f, actionCooldown - shakeDelay));

        isBusy = false;
    }

    void UpdateDisplay()
    {
        if (statusText == null || runtime == null) return;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<color=#FFD700><b>[ MONITOR ]</b></color>");

        var states = runtime.GetAllStates();

        foreach (var kv in states)
        {
            sb.AppendLine($"{kv.Key}: {kv.Value:F0}");
        }

        if (isLocked)
            sb.AppendLine("<color=red><b>RECOVERY MODE: Press R</b></color>");
        else
            sb.AppendLine(logMessage);

        statusText.text = sb.ToString();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && !isLocked)
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