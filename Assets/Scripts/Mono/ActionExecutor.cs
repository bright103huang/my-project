using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ActionExecutor : MonoBehaviour
{
    [Header("Action List")]
    public List<ActionDefinition> actions;

    [Header("Appliers")]
    public ModifierApplier modifierApplier;
    public AnimationApplier animationApplier;
    public UIApplier uiApplier;

    private bool isBusy = false;

    // ✅ 根据ID执行（给 ActionRequester 用）
    public void ExecuteByID(string actionID)
    {
        ActionDefinition action = actions.Find(a => a.actionID == actionID);

        if (action == null)
        {
            Debug.LogWarning($"ActionExecutor: 找不到 Action [{actionID}]");
            return;
        }

        Execute(action);
    }

    // ✅ 执行 Action
    public void Execute(ActionDefinition action)
    {
        if (action == null || isBusy)
            return;

        StartCoroutine(RunAction(action));
    }

    // ✅ 核心执行流程（只负责“发请求”）
    private IEnumerator RunAction(ActionDefinition action)
    {
        isBusy = true;

        Debug.Log($"EXECUTOR: 执行 [{action.actionID}]");

        // 1️⃣ 动画请求（不直接播放！）
        // 🔥 发送动画请求（修复版）
        if (animationApplier != null && !string.IsNullOrEmpty(action.animName))
        {
            AnimationRequest request = new AnimationRequest(
                action.animName,
                action.priority,
                action.duration
            );

            animationApplier.Apply(request);
        }

        // 2️⃣ 数值修改请求
        if (modifierApplier != null && action.modifiers != null && action.modifiers.Count > 0)
        {
            modifierApplier.Apply(action.modifiers);
        }

        // 3️⃣ UI请求
        if (uiApplier != null && !string.IsNullOrEmpty(action.message))
        {
            uiApplier.Apply(new UIRequest
            {
                message = action.message,
                duration = action.duration,
                priority = action.priority
            });
        }

        // 4️⃣ 等待动作时间（只控制节奏）
        yield return new WaitForSeconds(action.duration);

        isBusy = false;

        Debug.Log($"EXECUTOR: 完成 [{action.actionID}]");
    }

    // ✅ 强制执行（用于高优先级事件，例如“抽搐”）
    public void ForceExecute(ActionDefinition action)
    {
        if (action == null)
            return;

        StopAllCoroutines();
        isBusy = false;

        StartCoroutine(RunAction(action));
    }
}