using UnityEngine;

[CreateAssetMenu(menuName = "Game/Event")]
public class NarrativeEvent : ScriptableObject
{
    public string eventID;

    public Condition triggerCondition;

    // 🔥 关键：事件触发的是 Action
    public ActionDefinition action;
}
