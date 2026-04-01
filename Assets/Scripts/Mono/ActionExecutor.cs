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

    void Awake()
    {
        if (modifierApplier == null) modifierApplier = GetComponent<ModifierApplier>();
        if (animationApplier == null) animationApplier = GetComponent<AnimationApplier>();
        if (uiApplier == null) uiApplier = GetComponent<UIApplier>();

        if (modifierApplier == null) modifierApplier = GetComponentInChildren<ModifierApplier>();
        if (animationApplier == null) animationApplier = GetComponentInChildren<AnimationApplier>();
        if (uiApplier == null) uiApplier = GetComponentInChildren<UIApplier>();
    }

    private bool isBusy = false;

    public void ExecuteByID(string actionID)
    {
        if (actions == null) return;
        ActionDefinition action = actions.Find(a => a.actionID == actionID);

        if (action == null)
        {
            Debug.LogWarning($"ActionExecutor: 找不到 Action [{actionID}]");
            return;
        }

        Execute(action);
    }

    public void Execute(ActionDefinition action)
    {
        if (action == null || isBusy)
            return;

        StartCoroutine(RunAction(action));
    }

    private Dictionary<float, WaitForSeconds> waitCache = new Dictionary<float, WaitForSeconds>();

    private WaitForSeconds GetWait(float seconds)
    {
        if (!waitCache.ContainsKey(seconds))
            waitCache[seconds] = new WaitForSeconds(seconds);
        return waitCache[seconds];
    }

    private IEnumerator RunAction(ActionDefinition action)
    {
        isBusy = true;

        yield return null;

        Debug.Log($"EXECUTOR: 执行 [{action.actionID}]");

        if (animationApplier != null && !string.IsNullOrEmpty(action.animName))
        {
            AnimationRequest request = new AnimationRequest(
                action.animName,
                action.priority,
                action.duration
            );

            animationApplier.Apply(request);
        }

        if (modifierApplier != null && action.modifiers != null && action.modifiers.Count > 0)
        {
            modifierApplier.Apply(action.modifiers);
        }

        if (uiApplier != null && !string.IsNullOrEmpty(action.message))
        {
            uiApplier.Apply(new UIRequest
            {
                message = action.message,
                duration = action.duration,
                priority = action.priority
            });
        }

        yield return GetWait(action.duration);

        isBusy = false;

        Debug.Log($"EXECUTOR: 完成 [{action.actionID}]");
    }

    public void ForceExecute(ActionDefinition action)
    {
        if (action == null)
            return;

        StopAllCoroutines();
        isBusy = false;

        StartCoroutine(RunAction(action));
    }
}
