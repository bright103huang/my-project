
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NarrativeSandbox/Action")]
public class ActionDefinition : ScriptableObject
{
    public string actionID;
    public List<StateModifier> modifiers;
    public Condition condition;
}