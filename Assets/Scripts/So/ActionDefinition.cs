using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Action")]
public class ActionDefinition : ScriptableObject
{
    [Header("Basic")]
    public string actionID;

    [Header("Timing")]
    public float duration = 1f;

    [Header("Priority")]
    public int priority = 0;

    [Header("Animation")]
    public string animName;

    [Header("UI")]
    [TextArea]
    public string message;
    [TextArea]
    public string failMessage;

    [Header("State Changes")]
    public List<StateModifier> modifiers;

    [Header("Conditions")]
    public Condition startCondition;
    public Condition repeatCondition;

    [Header("Chaining")]
    public ActionDefinition fallbackAction;
}
