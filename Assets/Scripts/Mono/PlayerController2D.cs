using UnityEngine;
using System.Collections;
using TMPro;
using System.Text;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float speed = 4f;
    public float shakeDelay = 0.2f;
    public float actionCooldown = 1.0f;

    public TextMeshProUGUI statusText;
    public Animator anim;
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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        if (anim == null) anim = GetComponentInChildren<Animator>();
        if (statusText == null) statusText = FindObjectOfType<TextMeshProUGUI>();

        InitLayout();
    }

    void InitLayout()
    {
        statusText.enableAutoSizing = true;
        statusText.fontSizeMin = 10;
        statusText.fontSizeMax = 16;
        statusText.alignment = TextAlignmentOptions.TopLeft;

        RectTransform rt = statusText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.sizeDelta = new Vector2(280, 600);
        rt.anchoredPosition = new Vector2(100, -75);
    }

    void Update()
    {
        UpdateDisplay();

        if (isLocked)
        {
            rb.velocity = Vector2.zero;
            HandleRest();
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

    void HandleMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        rb.velocity = new Vector2(h, v).normalized * speed;

        if (anim != null)
            anim.SetBool("isWalking", rb.velocity.magnitude > 0.1f);
    }

    void HandleAction()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!nearTree)
            {
                SetMessage("你试图隔空打牛，牛表示毫无压力。");
                return;
            }

            if (isBusy)
            {
                SetMessage("你还没缓过来，又想自残？");
                return;
            }

            if (executor != null && !executor.CanExecute(hitTreeAction))
            {
                float energy = runtime.Get("Energy");
                SetMessage($"你现在只有 {energy:F0} 点能量，树都懒得理你。");
                return;
            }

            StartCoroutine(PerformHit());
        }
    }

    void HandleRest()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (executor != null && restAction != null)
                executor.Execute(restAction);

            if (anim != null)
                anim.Play("Rest", 0, 0f);
        }
    }

    IEnumerator PerformHit()
    {
        isBusy = true;

        yield return new WaitForSeconds(shakeDelay);

        if (currentTree != null)
            currentTree.Shake();

        if (executor != null)
            executor.Execute(hitTreeAction);

        yield return new WaitForSeconds(actionCooldown);

        isBusy = false;
    }

    void UpdateDisplay()
    {
        if (runtime == null) return;

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("<b>[ MONITOR ]</b>");

        foreach (var kv in runtime.GetAllStates())
        {
            sb.AppendLine($"{kv.Key}: {kv.Value:F0}");
        }

        sb.AppendLine(logMessage);

        statusText.text = sb.ToString();
    }

    public void SetLocked(bool value)
    {
        isLocked = value;
    }

    public void SetMessage(string msg)
    {
        logMessage = msg;
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