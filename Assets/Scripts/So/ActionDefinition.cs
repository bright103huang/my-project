using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NarrativeSandbox/Action")]
public class ActionDefinition : ScriptableObject
{
    public string actionID;

    [Header("效果（状态修改）")]
    public List<StateModifier> modifiers;

    [Header("执行条件（为空=永远可执行）")]
    public Condition availabilityCondition;
}