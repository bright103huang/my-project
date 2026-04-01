using UnityEngine;
using System.Collections.Generic;

public class ModifierApplier : MonoBehaviour
{
    public StateRuntime runtime;

    void Awake()
    {
        if (runtime == null) runtime = GetComponent<StateRuntime>();
        if (runtime == null) runtime = GetComponentInParent<StateRuntime>();
    }

    public void Apply(List<StateModifier> modifiers)
    {
        if (runtime == null || modifiers == null) return;

        foreach (var mod in modifiers)
        {
            runtime.Modify(mod.state, mod.value);
        }
    }
}
