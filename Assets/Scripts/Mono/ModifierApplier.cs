using UnityEngine;
using System.Collections.Generic;

public class ModifierApplier : MonoBehaviour
{
    public StateRuntime runtime;

    public void Apply(List<StateModifier> modifiers)
    {
        foreach (var mod in modifiers)
        {
            runtime.Modify(mod.state.stateID, mod.value);
        }
    }
}
