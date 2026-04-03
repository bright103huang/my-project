using UnityEngine;
using TMPro;

public class UIMessageDisplay : MonoBehaviour
{
    [Header("文字内容")]
    [TextArea(3, 5)]
    public string message = "你太弱了，快去撞树炼体吧，只有力量达到32，进入炼体一级，才有资格进入莽荒森林,按F!";
    
    [Header("样式设置")]
    public float fontSize = 18f;
    public Color textColor = Color.white;
    
    [Header("位置与大小")]
    public Vector2 offset = new Vector2(-20, -50); 
    public Vector2 boxSize = new Vector2(300, 200); 

    private TextMeshProUGUI textMesh;

    private void Start()
    {
        CreateUI();
    }

    private void OnValidate()
    {
        if (textMesh != null) UpdateUI();
    }

    private void CreateUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("MessageCanvas");
            canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasGo.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }

        GameObject textGo = new GameObject("TopRightMessage");
        textGo.transform.SetParent(canvas.transform, false);
        textMesh = textGo.AddComponent<TextMeshProUGUI>();
        UpdateUI();
    }

    private void UpdateUI()
    {
        textMesh.text = message;
        textMesh.fontSize = fontSize;
        textMesh.color = textColor;
        textMesh.alignment = TextAlignmentOptions.TopRight;
        textMesh.enableWordWrapping = true;

        RectTransform rect = textMesh.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.anchoredPosition = offset;
        rect.sizeDelta = boxSize;
    }
}
