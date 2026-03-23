using UnityEngine;

[CreateAssetMenu(menuName = "NarrativeSandbox/State")]
public class StateDefinition : ScriptableObject
{
    public string stateID;
    public float initialValue;
    public float maxValue = 100f;
}
