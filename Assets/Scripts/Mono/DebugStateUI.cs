using UnityEngine;
using System.Text;

public class DebugStateUI : MonoBehaviour
{
    public StateRuntime runtime;

    private string displayText = "";

    void OnEnable()
    {
        if (runtime != null)
            runtime.OnStateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        if (runtime != null)
            runtime.OnStateChanged -= OnStateChanged;
    }

    void Start()
    {
        RefreshAll(); // 初始显示一次
    }

    void OnStateChanged(string id, float value)
    {
        RefreshAll();
    }

    void RefreshAll()
    {
        if (runtime == null) return;

        var states = runtime.GetAllStates();
        if (states == null) return;

        StringBuilder sb = new StringBuilder();

        foreach (var kv in states)
        {
            sb.AppendLine($"{kv.Key}: {kv.Value:F0}");
        }

        displayText = sb.ToString();
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 500), displayText);
    }
}
