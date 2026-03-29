using UnityEngine;
using System;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public List<NarrativeEvent> events;
    public StateRuntime runtime;
    public ModifierApplier applier;

    // ⭐ 新增：通知表现层的事件
    public event Action<NarrativeEvent> OnEventTriggered;

    private Dictionary<string, bool> lastEventState = new Dictionary<string, bool>();

    void Update()
    {
        CheckEvents();
    }

    void CheckEvents()
    {
        foreach (var e in events)
        {
            if (e.triggerCondition == null) continue;

            bool current = e.triggerCondition.Evaluate(runtime);

            if (!lastEventState.ContainsKey(e.eventID))
                lastEventState[e.eventID] = false;

            if (!lastEventState[e.eventID] && current)
            {
                if (applier != null) applier.Apply(e.modifiers);
                
                // ⭐ 发送广播
                OnEventTriggered?.Invoke(e);
            }

            lastEventState[e.eventID] = current;
        }
    }
}
