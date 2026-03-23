using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "NarrativeSandbox/Event")]
public class NarrativeEvent : ScriptableObject
{
    public Condition triggerCondition;
    [TextArea] public string hintText;
    public List<StateModifier> consequences;
}
