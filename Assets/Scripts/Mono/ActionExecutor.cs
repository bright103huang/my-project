using UnityEngine;
using System;

public class ActionExecutor : MonoBehaviour
{
    public StateRuntime runtime;
    public void Execute(ActionDefinition action)
    {
        if (action.condition == null || action.condition.Evaluate(runtime))
        {
            foreach (var mod in action.modifiers)
                runtime.Modify(mod.state.stateID, mod.value);
        }
    }
}
