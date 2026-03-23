using UnityEngine;
using System.Collections.Generic;


[CreateAssetMenu(menuName = "NarrativeSandbox/Conditions/StateCheck")]
public class StateCheckCondition : Condition
{
    public StateDefinition state;
    public enum CompareType { GreaterEqual, LessEqual }
    public CompareType type;
    public float threshold;

    public override bool Evaluate(StateRuntime runtime)
    {
        float val = runtime.Get(state.stateID);
        return type == CompareType.GreaterEqual ? val >= threshold : val <= threshold;
    }
}
