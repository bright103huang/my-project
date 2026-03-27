using UnityEngine;

public class DebugStateUI : MonoBehaviour
{
    public StateRuntime runtime;

    void OnGUI()
    {
        if (runtime == null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "runtime Œ¥∞Û∂®£°");
            return;
        }

        var states = runtime.GetAllStates();

        if (states == null)
        {
            GUI.Label(new Rect(10, 10, 300, 20), "states Œ¥≥ı ºªØ£°");
            return;
        }

        int y = 10;

        foreach (var kv in states)
        {
            GUI.Label(new Rect(10, y, 300, 20), $"{kv.Key}: {kv.Value:F0}");
            y += 20;
        }
    }
}
