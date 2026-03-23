using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events; // <--- 必须加上这一行！

public class EventManager : MonoBehaviour
{
    public StateRuntime runtime;
    public List<NarrativeEvent> events;
    public UnityEvent<string> OnEventTriggered;
    private HashSet<NarrativeEvent> triggeredOnce = new HashSet<NarrativeEvent>();

    void Update()
    {
        foreach (var e in events)
        {
            if (!triggeredOnce.Contains(e) && e.triggerCondition.Evaluate(runtime))
            {
                OnEventTriggered.Invoke(e.hintText);
                foreach (var mod in e.consequences) runtime.Modify(mod.state.stateID, mod.value);
                triggeredOnce.Add(e);
            }
        }
    }
}
