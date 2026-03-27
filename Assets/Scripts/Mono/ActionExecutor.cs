using UnityEngine;
using System;

public class ActionExecutor : MonoBehaviour
{
    [Header("Core References")]
    public StateRuntime runtime;
    public ModifierApplier applier;

    public event Action<ActionDefinition> OnActionExecuted;

    // ⭐ 新增：是否允许执行
    public bool CanExecute(ActionDefinition action)
    {
        if (action == null)
            return false;

        // 没有条件 = 永远可执行
        if (action.availabilityCondition == null)
            return true;

        return action.availabilityCondition.Evaluate(runtime);
    }

    // ⭐ 执行动作
    public void Execute(ActionDefinition action)
    {
        if (!CanExecute(action))
        {
            Debug.Log($"Action {action.actionID} is not available.");
            return;
        }

        if (applier != null)
            applier.Apply(action.modifiers);

        Debug.Log($"Executed Action: {action.actionID}");

        OnActionExecuted?.Invoke(action);
    }
}
