using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NarrativeSandbox/Action")]
public class ActionDefinition : ScriptableObject
{
    public string actionID;

    [Header("Logic")]
    public List<StateModifier> modifiers;
    public Condition availabilityCondition;

    [Header("Output (Visuals & Audio)")]
    public string animTrigger; // 动画 Trigger 名称
    [TextArea]
    public string[] humorousHints; // 该动作触发时的幽默提示词库
}
