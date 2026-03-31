using UnityEngine;
using System.Collections;

public class AnimationApplier : MonoBehaviour
{
    public Animator animator;

    private AnimationRequest currentRequest;
    private Coroutine currentRoutine;

    // ✅ 对外唯一入口
    public void Apply(AnimationRequest request)
    {
        if (request == null || animator == null)
            return;

        // ❗ 优先级判断
        if (currentRequest != null && request.priority < currentRequest.priority)
            return;

        // ❗ 避免重复播放同一个动画
        if (currentRequest != null && currentRequest.stateName == request.stateName)
            return;

        currentRequest = request;

        // ❗ 停掉旧流程（防止冲突）
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayRoutine(request));
    }

    // ✅ 动画播放流程（核心）
    private IEnumerator PlayRoutine(AnimationRequest request)
    {
        // ▶ 播放目标动画
        animator.Play(request.stateName);

        // ⏳ 等待动画时长（由 Action 决定）
        yield return new WaitForSeconds(request.duration);

        // ❗ 只有当当前动画仍然是自己，才回 Idle（防止被高优先级打断后误回）
        if (currentRequest == request)
        {
            animator.Play("Idle");
            currentRequest = null;
        }
    }

    // ✅ 强制清空（紧急情况）
    public void Clear()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRequest = null;

        if (animator != null)
            animator.Play("Idle");
    }
}