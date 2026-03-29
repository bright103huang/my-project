using UnityEngine;
using System.Collections;

public class OutputController : MonoBehaviour
{
    [Header("Core References")]
    public ActionExecutor executor;
    public EventManager eventManager;
    public PlayerController2D player;
    public StateRuntime runtime;
    public Animator anim;

    private bool isConvulsing = false;

    void OnEnable()
    {
        // 订阅动作和事件
        if (executor != null) executor.OnActionExecuted += OnActionTriggered;
        if (eventManager != null) eventManager.OnEventTriggered += OnEventTriggered;
    }

    void OnDisable()
    {
        if (executor != null) executor.OnActionExecuted -= OnActionTriggered;
        if (eventManager != null) eventManager.OnEventTriggered -= OnEventTriggered;
    }

    private void OnActionTriggered(ActionDefinition action)
    {
        PlayOutput(action.animTrigger, action.humorousHints);
    }

    private void OnEventTriggered(NarrativeEvent evt)
    {
        // 事件优先显示核心剧情文字
        string finalMsg = evt.narrativeText;
        if (evt.humorousHints != null && evt.humorousHints.Length > 0)
        {
            finalMsg += "\n<i>" + evt.humorousHints[Random.Range(0, evt.humorousHints.Length)] + "</i>";
        }
        
        player.SetMessage(finalMsg);

        if (!string.IsNullOrEmpty(evt.animTrigger))
            anim.SetTrigger(evt.animTrigger);
    }

    private void PlayOutput(string trigger, string[] hints)
    {
        // 播放动画
        if (!string.IsNullOrEmpty(trigger))
            anim.SetTrigger(trigger);

        // 挑选台词
        if (hints != null && hints.Length > 0)
        {
            player.SetMessage(hints[Random.Range(0, hints.Length)]);
        }
    }

    // 处理特殊的状态阈值（如抽搐）
    void Update()
    {
        if (runtime == null) return;
        float fatigue = runtime.Get("Fatigue");

        if (fatigue >= 90 && !isConvulsing)
        {
            isConvulsing = true;
            player.SetLocked(true);
            anim.SetTrigger("Convulse");
            player.SetMessage("生活不只有撞树，还有躺平。");
        }
        else if (isConvulsing && fatigue < 40)
        {
            isConvulsing = false;
            player.SetLocked(false);
            anim.Play("Idle");
            player.SetMessage("你居然活过来了，医学奇迹。");
        }
    }
}
