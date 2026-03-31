using UnityEngine;

public class AnimationRequest
{
    public string stateName;
    public int priority;
    public float duration; // ⭐ 必须加这个！

    public AnimationRequest(string stateName, int priority, float duration)
    {
        this.stateName = stateName;
        this.priority = priority;
        this.duration = duration;
    }
}
