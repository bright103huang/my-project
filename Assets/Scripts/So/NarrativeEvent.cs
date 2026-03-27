using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NarrativeSandbox/Event")]
public class NarrativeEvent : ScriptableObject
{
    public string eventID; // ⭐ 加这一行（关键）

    public Condition triggerCondition;
    public List<StateModifier> modifiers;

    [TextArea]
    public string narrativeText;
}
