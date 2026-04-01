using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationApplier : MonoBehaviour
{
    public Animator animator;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
    }

    private AnimationRequest currentRequest;
    private Coroutine currentRoutine;

    public void Apply(AnimationRequest request)
    {
        if (request == null || animator == null)
            return;

        if (currentRequest != null && request.priority < currentRequest.priority)
            return;

        if (currentRequest != null && currentRequest.stateName == request.stateName)
            return;

        currentRequest = request;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(PlayRoutine(request));
    }

    private Dictionary<float, WaitForSeconds> waitCache = new Dictionary<float, WaitForSeconds>();

    private WaitForSeconds GetWait(float seconds)
    {
        if (!waitCache.ContainsKey(seconds))
            waitCache[seconds] = new WaitForSeconds(seconds);
        return waitCache[seconds];
    }

    private IEnumerator PlayRoutine(AnimationRequest request)
    {
        animator.Play(request.stateName);

        yield return GetWait(request.duration);

        if (currentRequest == request)
        {
            animator.Play("Idle");
            currentRequest = null;
        }
    }

    public void Clear()
    {
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRequest = null;

        if (animator != null)
            animator.Play("Idle");
    }
}
