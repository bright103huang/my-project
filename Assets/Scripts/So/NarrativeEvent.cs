using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NarrativeSandbox/Event")]
public class NarrativeEvent : ScriptableObject
{
    public string eventID;

    public Condition triggerCondition;
    public List<StateModifier> modifiers;

    [TextArea]
    public string narrativeText; // 核心剧情文字

    [Header("Output")]
    public string animTrigger; 
    [TextArea]
    public string[] humorousHints; // 触发时的额外吐槽
}
