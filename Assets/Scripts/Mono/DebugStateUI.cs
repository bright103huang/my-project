using UnityEngine;
using TMPro;
using System.Text;
using System.Collections.Generic;

public class DebugStateUI : MonoBehaviour
{
    public StateRuntime runtime;
    public TextMeshProUGUI debugText;

    [Header("UI Layout Settings")]
    public float fontSize = 14f; // 调小字体
    public Vector2 offset = new Vector2(150, -200); // 向下向右移动
    public Vector2 uiSize = new Vector2(500, 800); // 增加宽度和高度以显示更多行

    private string[] displayOrder = {
        "Health", "Energy", "Fatigue",
        "Strength", "Skill", "Speed", "Spirit"
    };

    void Awake()
    {
        SetupUILayout();
    }

    // 自动配置 UI 位置和样式，确保它在左上角且字体适中
    void SetupUILayout()
    {
        if (debugText == null) debugText = GetComponent<TextMeshProUGUI>();
        if (debugText == null) return;

        RectTransform rt = debugText.GetComponent<RectTransform>();
        
        // 设置到左上角 (Top-Left)
        rt.anchorMin = new Vector2(0, 1); 
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);
        rt.anchoredPosition = offset; 
        rt.sizeDelta = uiSize;

        debugText.fontSize = fontSize; 
        debugText.alignment = TextAlignmentOptions.TopLeft;
        debugText.color = Color.white;
        debugText.richText = true;
        
        // 增加一行阴影效果（可选，提升可读性）
        if (debugText.fontMaterial != null)
            debugText.fontMaterial.EnableKeyword("UNDERLAY_ON");
    }

    void OnEnable()
    {
        if (runtime != null) runtime.OnStateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        if (runtime != null) runtime.OnStateChanged -= OnStateChanged;
    }

    void Start()
    {
        RefreshAll(); 
    }

    void OnStateChanged(string id, float value)
    {
        RefreshAll();
    }

    private string currentActionHint = ""; // 当前动作提示

    public void RefreshAll()
    {
        if (runtime == null || debugText == null) return;

        var states = runtime.GetAllStates();
        if (states == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("<size=110%><b>[ 角色状态 ]</b></size>");
        sb.AppendLine("----------------------------------");

        foreach (var key in displayOrder)
        {
            if (states.ContainsKey(key))
            {
                float val = states[key];
                // 优化间距和显示
                sb.AppendLine($"{GetLabel(key)}: {val,5:F1}  {GetBar(val)}");
            }
        }

        // 添加动作提示行（始终显示在状态下方）
        if (!string.IsNullOrEmpty(currentActionHint))
        {
            sb.AppendLine(); // 空行分隔
            sb.AppendLine($"<b>▶ {currentActionHint}</b>"); // 改为白色加粗（默认白）
        }

        debugText.text = sb.ToString();
    }

    // 新增：显示动作提示
    public void ShowActionHint(string message)
    {
        currentActionHint = message;
        RefreshAll(); // 刷新显示以包含动作提示
    }

    // 新增：隐藏动作提示
    public void HideActionHint()
    {
        currentActionHint = "";
        RefreshAll(); // 刷新显示以移除动作提示
    }

    string GetLabel(string id)
    {
        switch (id)
        {
            case "Health":   return "生命值";
            case "Energy":   return "能量值";
            case "Fatigue":  return "疲劳度";
            case "Strength": return "力量  ";
            case "Skill":    return "技术  ";
            case "Speed":    return "速度  ";
            case "Spirit":   return "精神  ";
            default: return id;
        }
    }

    string GetBar(float value)
    {
        int segments = 10;
        int filled = Mathf.RoundToInt(Mathf.Clamp(value, 0, 100) / 100f * segments);
        string bar = "<color=#555555>[</color>";
        for (int i = 0; i < segments; i++)
        {
            if (i < filled)
                bar += "<color=#00FF00>|</color>"; // 使用更细的符号减少空间占用
            else
                bar += "<color=#333333>.</color>";
        }
        bar += "<color=#555555>]</color>";
        return bar;
    }
}
