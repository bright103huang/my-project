using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DebugStateUI : MonoBehaviour
{
    public StateRuntime runtime;

    void OnGUI()
    {
        if (runtime == null) return;

        GUI.Box(new Rect(10, 10, 200, 150), "玩家状态监控");
        GUI.Label(new Rect(20, 30, 180, 20), "力量: " + runtime.Get("Strength"));
        GUI.Label(new Rect(20, 50, 180, 20), "疲劳: " + runtime.Get("Fatigue"));
        GUI.Label(new Rect(20, 70, 180, 20), "健康: " + runtime.Get("Health"));
        GUI.Label(new Rect(20, 90, 180, 20), "能量: " + runtime.Get("Energy"));
    }
}
