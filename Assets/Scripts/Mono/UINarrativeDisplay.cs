using UnityEngine;
using TMPro; // 记得导入 TextMeshPro

public class UINarrativeDisplay : MonoBehaviour
{
    public TextMeshProUGUI hintText;
    public EventManager eventManager;

    void OnEnable()
    {
        // 建议在 NarrativeEvent 里加一个 C# event，或者简单地在 EventManager 里加个委托
        // 这里提供一个简单的逻辑：每当 Event 触发，更新文字
    }
}
