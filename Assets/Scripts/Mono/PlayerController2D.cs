using UnityEngine;
using System.Collections;
using TMPro;
using System.Text;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("--- 物理 & 动作参数 ---")]
    public float speed = 4f;
    public float shakeDelay = 0.2f;
    public float actionCooldown = 1.0f;
    public float fallYOffset = -0.75f;

    [Header("--- 资源引用 (SO/UI/Anim) ---")]
    public TextMeshProUGUI statusText;
    public Animator anim;
    public Transform modelTransform;
    public ActionExecutor executor;
    public StateRuntime runtime;
    public ActionDefinition hitTreeAction; // 撞树SO
    public ActionDefinition restAction;    // 休息SO

    private Rigidbody2D rb;
    private bool isLocked = false;      // 崩溃锁
    private bool isBusy = false;        // 动作锁
    private bool nearTree = false;
    private TreeVisual currentTree;
    private string logMessage = "Standing still is the only safe move.";

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (modelTransform == null && anim != null) modelTransform = anim.transform;
        if (statusText == null) statusText = GameObject.FindObjectOfType<TextMeshProUGUI>();

        InitLayout();
    }

    void InitLayout()
    {
        if (statusText == null) return;
        statusText.enableAutoSizing = false;
        statusText.fontSize = 15;
        statusText.alignment = TextAlignmentOptions.TopLeft;

        RectTransform textRt = statusText.GetComponent<RectTransform>();
        textRt.anchorMin = new Vector2(0, 1);
        textRt.anchorMax = new Vector2(0, 1);
        textRt.pivot = new Vector2(0, 1);
        textRt.sizeDelta = new Vector2(280, 450);
        textRt.anchoredPosition = new Vector2(100, -75);

        Image bgImage = statusText.GetComponentInParent<Image>();
        if (bgImage != null)
        {
            RectTransform bgRt = bgImage.GetComponent<RectTransform>();
            bgRt.sizeDelta = new Vector2(bgRt.sizeDelta.x, 320); // 延伸黑玻璃高度
            bgRt.pivot = new Vector2(0.5f, 1);
        }
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        UpdateDisplay();

        // 逻辑闸门：崩溃状态
        if (isLocked)
        {
            rb.velocity = Vector2.zero;
            HandleRestLogic();
            return;
        }

        // 逻辑闸门：动作状态
        if (isBusy)
        {
            rb.velocity = Vector2.zero;
            return;
        }

        HandleMovement();
        HandleAction();
    }

    void UpdateDisplay()
    {
        if (statusText == null || runtime == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<color=#FFD700><b>[ BIOMONITOR ]</b></color>");
        sb.AppendLine("STR: " + runtime.Get("Strength").ToString("F0") + " <color=#888>(Power)</color>");
        sb.AppendLine("FAT: " + runtime.Get("Fatigue").ToString("F0") + " <color=#FF4444>(Lethal)</color>");
        sb.AppendLine("ENG: " + runtime.Get("Energy").ToString("F0"));

        sb.AppendLine("\n<color=#00FF00><b>[ NEURAL LOG ]</b></color>");

        if (isLocked)
        {
            sb.AppendLine("<color=red>SYSTEM CRITICAL!</color>");
            sb.AppendLine("<i>Body collapsed due to extreme FAT.</i>");
            sb.AppendLine("<color=yellow>Press R to beg for mercy (Rest).</color>");
        }
        else
        {
            sb.AppendLine("<size=90%>" + logMessage + "</size>");
        }

        statusText.text = sb.ToString();
    }

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        rb.velocity = new Vector2(h, v).normalized * speed;

        if (anim != null) anim.SetBool("isWalking", rb.velocity.magnitude > 0.1f);
        if (h != 0) transform.localScale = new Vector3(Mathf.Sign(h), 1, 1);

        if (rb.velocity.magnitude > 0.1f && !nearTree)
            logMessage = "Wandering in the void...";
    }

    void HandleAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // 策划校验 1：是否在范围内
            if (!nearTree)
            {
                logMessage = "<color=orange>Tree not found. Fighting shadows?</color>";
                return;
            }

            // 策划校验 2：数值超标判定 (Energy不足)
            if (runtime.Get("Energy") < 10)
            {
                logMessage = "<color=red>Too weak to swing. Go home.</color>";
                return;
            }

            StartCoroutine(PerformHitRoutine());
        }
    }

    IEnumerator PerformHitRoutine()
    {
        isBusy = true;
        logMessage = "Hit initiated. Farewell, joints!";

        if (anim != null) anim.SetTrigger("HitTree");
        yield return new WaitForSeconds(shakeDelay);

        if (currentTree != null) currentTree.Shake();
        if (executor != null) executor.Execute(hitTreeAction);

        logMessage = "Impact! Strength increased.";

        yield return new WaitForSeconds(actionCooldown - shakeDelay);
        isBusy = false;
        logMessage = "Ready for the next self-abuse.";
    }

    void HandleRestLogic()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 调用 SO 的后果：减少疲劳
            if (executor != null && restAction != null) executor.Execute(restAction);

            // 状态机触发：从 Convulse 切换到 Resting
            if (anim != null) anim.SetTrigger("Rest");

            logMessage = "Resting... Regretting life choices.";
        }

        // 自动苏醒判定
        if (runtime.Get("Fatigue") < 40)
        {
            isLocked = false;
            if (modelTransform != null) modelTransform.localPosition = Vector3.zero;

            // 状态机触发：从 Resting 切换到 Idle
            if (anim != null) anim.SetTrigger("WakeUp");

            logMessage = "Back on your feet. Go hit that tree!";
        }
    }

    // 由后端系统检测到 Fatigue=100 时调用
    public void LockPlayer(string message)
    {
        if (isLocked) return;
        isLocked = true;
        isBusy = false;
        rb.velocity = Vector2.zero;

        // 强制进入抽搐状态
        if (anim != null) anim.Play("Convulse", 0, 0f);
        if (modelTransform != null) modelTransform.localPosition = new Vector3(0, fallYOffset, 0);

        logMessage = "FATAL ERROR: Muscle failure.";
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Tree") && !isLocked)
        {
            nearTree = true;
            currentTree = other.GetComponent<TreeVisual>();
            logMessage = "Target acquired. Hit it to gain STR.";
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tree"))
        {
            nearTree = false;
            currentTree = null;
            if (!isLocked) logMessage = "Target lost. Feel lonely?";
        }
    }
}