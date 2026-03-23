using System.Collections.Generic;
using UnityEngine;

public class StateRuntime : MonoBehaviour
{
    public List<StateDefinition> stateDefinitions;
    private Dictionary<string, float> values = new Dictionary<string, float>();

    void Awake()
    {
        foreach (var def in stateDefinitions)
        {
            values[def.stateID] = def.initialValue;
        }
    }

    public float Get(string id)
    {
        return values.ContainsKey(id) ? values[id] : 0;
    }

    public void Modify(string id, float amount)
    {
        if (values.ContainsKey(id))
        {
            values[id] = Mathf.Clamp(values[id] + amount, 0, 100);
            Debug.Log($"×´Ì¬¸üÐÂ: {id} = {values[id]}");
        }
    }
}
