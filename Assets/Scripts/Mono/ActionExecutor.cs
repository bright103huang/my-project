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

    [Header("Runtime")]
    public StateRuntime runtime;

    public System.Action<string> OnActionStarted;

    private bool isBusy = false;
    private ActionDefinition currentAction;

    public bool IsBusy() => isBusy;

    void Awake()
    {
        if (runtime == null) runtime = GetComponent<StateRuntime>();
        if (runtime == null) runtime = GetComponentInParent<StateRuntime>();

        if (modifierApplier == null) modifierApplier = GetComponent<ModifierApplier>();
        if (animationApplier == null) animationApplier = GetComponent<AnimationApplier>();
        if (uiApplier == null) uiApplier = GetComponent<UIApplier>();

        if (modifierApplier == null) modifierApplier = GetComponentInChildren<ModifierApplier>();
        if (animationApplier == null) animationApplier = GetComponentInChildren<AnimationApplier>();
        if (uiApplier == null) uiApplier = GetComponentInChildren<UIApplier>();
    }

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
        if (action == null)
            return;

        // 如果当前正在执行动作，检查优先级
        if (isBusy && currentAction != null)
        {
            // 如果是同一个动作且当前正在执行，则忽略（实现回合制/等待完成）
            if (action.actionID == currentAction.actionID)
            {
                Debug.Log($"EXECUTOR: 忽略 [{action.actionID}]，因为该动作正在执行中");
                return;
            }

            // 如果新动作优先级更高，则中断当前动作
            if (action.priority > currentAction.priority)
            {
                Debug.Log($"EXECUTOR: 中断 [{currentAction.actionID}] 以执行 [{action.actionID}] (优先级: {action.priority} > {currentAction.priority})");
                ForceExecute(action);
            }
            else
            {
                Debug.Log($"EXECUTOR: 忽略 [{action.actionID}]，因为当前动作 [{currentAction.actionID}] 优先级更高或相等");
            }
            return;
        }

        // 检查开始条件
        if (action.startCondition != null && !action.startCondition.Evaluate(runtime))
        {
            Debug.Log($"EXECUTOR: 动作 [{action.actionID}] 开始条件未满足");
            if (uiApplier != null && !string.IsNullOrEmpty(action.failMessage))
            {
                uiApplier.Apply(new UIRequest
                {
                    message = action.failMessage,
                    duration = 2f,
                    priority = 100
                });
            }
            return;
        }

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
        if (action == null) yield break;

        isBusy = true;
        currentAction = action;

        // 给一点点缓冲，防止在一帧内无限递归
        yield return new WaitForEndOfFrame();

        OnActionStarted?.Invoke(action.actionID);

        // 特殊逻辑：撞树时让树震动
        if (action.actionID == "HitTree")
        {
            TreeVisual tree = FindObjectOfType<TreeVisual>();
            if (tree != null)
            {
                tree.OnHitTree();
            }
        }

        bool shouldLoop = true;
        while (shouldLoop)
        {
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

            // 状态值改变应该在动作执行过程中或结束时？
            // 用户说：动画一个动作没做完，状态值已经改变了几次
            // 原代码是在循环开始就 Apply
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

            // 确保状态显示不会被覆盖
            DebugStateUI debugUI = FindObjectOfType<DebugStateUI>();
            if (debugUI != null)
            {
                debugUI.RefreshAll();
            }

            yield return GetWait(action.duration);

            // Check if we should repeat
            if (action.repeatCondition != null && runtime != null)
            {
                // 重复执行前再次检查开始条件（比如距离）
                if (action.startCondition != null && !action.startCondition.Evaluate(runtime))
                {
                    shouldLoop = false;
                    Debug.Log($"EXECUTOR: 停止重复 [{action.actionID}]，因为开始条件不再满足。");
                }
                else
                {
                    shouldLoop = action.repeatCondition.Evaluate(runtime);
                    if (shouldLoop)
                    {
                        Debug.Log($"EXECUTOR: 重复执行 [{action.actionID}]，因为条件仍然满足。");
                        // 如果是撞树，重复时也震动
                        if (action.actionID == "HitTree")
                        {
                            TreeVisual tree = FindObjectOfType<TreeVisual>();
                            if (tree != null) tree.OnHitTree();
                        }
                    }
                }
            }
            else
            {
                shouldLoop = false;
            }
        }

        isBusy = false;
        ActionDefinition completedAction = currentAction;
        currentAction = null;

        Debug.Log($"EXECUTOR: 完成 [{action.actionID}]");

        // 如果该动作有后续动作 (Fallback)，则尝试执行
        if (completedAction != null && completedAction.fallbackAction != null)
        {
            // 检查后续动作的循环条件（如果有的话）
            bool canChain = completedAction.fallbackAction.repeatCondition == null || 
                           completedAction.fallbackAction.repeatCondition.Evaluate(runtime);
            
            if (canChain)
            {
                Debug.Log($"EXECUTOR: 衔接后续动作 [{completedAction.fallbackAction.actionID}]");
                Execute(completedAction.fallbackAction);
            }
        }
    }

    public void ForceExecute(ActionDefinition action)
    {
        if (action == null)
            return;

        StopAllCoroutines();
        isBusy = false;
        currentAction = null;

        StartCoroutine(RunAction(action));
    }
}
