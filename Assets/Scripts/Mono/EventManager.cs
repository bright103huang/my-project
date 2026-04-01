using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    [Header("Events")]
    public List<NarrativeEvent> events;

    [Header("References")]
    public StateRuntime runtime;
    public ActionExecutor executor;

    private Dictionary<string, bool> lastEventState = new Dictionary<string, bool>();

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

    // ✅ 不再用 Update 轮询！
    void OnStateChanged(string stateID, float value)
    {
        CheckEvents();
    }

    void CheckEvents()
    {
        foreach (var e in events)
        {
            if (e == null || e.triggerCondition == null)
                continue;

            bool current = e.triggerCondition.Evaluate(runtime);

            // ✅ 只在“从false → true”触发
            if (current && (!lastEventState.ContainsKey(e.eventID) || !lastEventState[e.eventID]))
            {
                lastEventState[e.eventID] = true; // 🔥 先标记，防止递归重触发
                TriggerEvent(e);
            }
            else
            {
                lastEventState[e.eventID] = current;
            }
        }
    }

    void TriggerEvent(NarrativeEvent e)
    {
        Debug.Log($"EVENT: 触发 [{e.eventID}]");

        // 🔥 核心：触发 Action，而不是直接改状态！
        if (executor != null && e.action != null)
        {
            executor.ForceExecute(e.action);
        }
    }
}