using UnityEngine;
using System.Collections.Generic;

public abstract class Condition : ScriptableObject
{
    public abstract bool Evaluate(StateRuntime runtime);
}
