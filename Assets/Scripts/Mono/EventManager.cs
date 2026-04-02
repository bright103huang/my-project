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
    private bool isChecking = false; // 重入锁（防止死循环）

    void OnEnable()
    {
        if (runtime != null)
            runtime.OnStateChanged += OnStateChanged;
        
        if (executor != null)
            executor.OnActionStarted += OnActionStartedInExecutor;
    }

    void OnDisable()
    {
        if (runtime != null)
            runtime.OnStateChanged -= OnStateChanged;

        if (executor != null)
            executor.OnActionStarted -= OnActionStartedInExecutor;
    }

    void Start()
    {
        // 核心修复：初始化时先同步一次状态
        if (runtime == null) return;

        foreach (var e in events)
        {
            if (e == null || e.triggerCondition == null) continue;
            lastEventState[e.eventID] = e.triggerCondition.Evaluate(runtime);
        }
        Debug.Log("EVENT: 初始状态同步完成，防止开局误触发。");
    }

    void OnActionStartedInExecutor(string actionID)
    {
        // 核心逻辑：只要开始的动作不是 Twitch 本身（比如休息、砍树、苏醒），
        // 我们就允许“抽搐”事件重新评估。这样如果能量依然为 0，系统会立即再次触发抽搐。
        if (actionID != "Twitch")
        {
            lastEventState.Remove("FatigueTwitch");
        }
    }

    void OnStateChanged(string stateID, float value)
    {
        // 如果当前已经在检查中，说明是由某个动作的 Modifier 触发的同步调用，忽略它，防止递归死锁
        if (isChecking) return; 
        
        CheckEvents();
    }

    void CheckEvents()
    {
        if (isChecking) return;
        isChecking = true;

        try
        {
            foreach (var e in events)
            {
                if (e == null || e.triggerCondition == null)
                    continue;

                bool current = e.triggerCondition.Evaluate(runtime);

                // 只在“从 false 变为 true”时触发
                if (current && (!lastEventState.ContainsKey(e.eventID) || !lastEventState[e.eventID]))
                {
                    lastEventState[e.eventID] = true;
                    TriggerEvent(e);
                }
                else
                {
                    lastEventState[e.eventID] = current;
                }
            }
        }
        finally
        {
            isChecking = false;
        }
    }

    void TriggerEvent(NarrativeEvent e)
    {
        Debug.Log($"EVENT: 触发 [{e.eventID}]，请求动作 [{e.action?.actionID}]");

        if (executor != null && e.action != null)
        {
            // 总是通过 Execute 提交，让它在下一帧（由于我们加了 WaitForEndOfFrame）或根据优先级处理
            executor.Execute(e.action);
        }
    }
}
