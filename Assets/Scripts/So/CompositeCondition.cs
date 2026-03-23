
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "NarrativeSandbox/Conditions/Composite")]
public class CompositeCondition : Condition
{
    public enum LogicType { AND, OR }
    public LogicType logic;
    public List<Condition> conditions;

    public override bool Evaluate(StateRuntime runtime)
    {
        if (logic == LogicType.AND)
        {
            foreach (var c in conditions) if (!c.Evaluate(runtime)) return false;
            return true;
        }
        else
        {
            foreach (var c in conditions) if (c.Evaluate(runtime)) return true;
            return false;
        }
    }
}