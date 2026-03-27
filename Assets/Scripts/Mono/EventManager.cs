using UnityEngine;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public List<NarrativeEvent> events;
    public StateRuntime runtime;
    public ModifierApplier applier;

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

            // ⭐ 只在 false → true 时触发
            if (!lastEventState[e.eventID] && current)
            {
                Debug.Log(e.narrativeText);
                applier.Apply(e.modifiers);
            }

            lastEventState[e.eventID] = current;
        }
    }
}
