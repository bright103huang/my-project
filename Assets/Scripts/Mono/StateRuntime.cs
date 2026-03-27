using System.Collections.Generic;
using UnityEngine;

public class StateRuntime : MonoBehaviour
{
    public List<StateDefinition> stateDefinitions;

    private Dictionary<string, float> values;
    private bool fatigueLocked = false;

    void Awake()
    {
        values = new Dictionary<string, float>();

        foreach (var def in stateDefinitions)
        {
            values[def.stateID] = def.initialValue;
        }
    }

    public float Get(string id)
    {
        return values.ContainsKey(id) ? values[id] : 0f;
    }

    public void Modify(string id, float amount)
    {
        if (!values.ContainsKey(id)) return;

        values[id] += amount;
        values[id] = Mathf.Clamp(values[id], 0, 100);

        CheckThreshold(id, values[id]);
    }

    public Dictionary<string, float> GetAllStates()
    {
        return values;
    }

    void CheckThreshold(string id, float value)
    {
        if (id == "Fatigue")
        {
            var player = FindObjectOfType<PlayerController2D>();
            if (player == null) return;

            if (value >= 90 && !fatigueLocked)
            {
                fatigueLocked = true;
                player.LockPlayer("Too tired!");
            }

            if (value < 40)
            {
                fatigueLocked = false;
            }
        }
    }
}