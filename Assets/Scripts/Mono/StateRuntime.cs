using System;
using System.Collections.Generic;
using UnityEngine;

public class StateRuntime : MonoBehaviour
{
    public List<StateDefinition> stateDefinitions;

    private Dictionary<string, float> values;

    // ✅ 关键：状态变化事件（核心升级点）
    public event Action<string, float> OnStateChanged;

    void Awake()
    {
        values = new Dictionary<string, float>();

        if (stateDefinitions == null || stateDefinitions.Count == 0)
        {
            Debug.LogError("❌ StateRuntime: stateDefinitions not configured!");
            return;
        }

        foreach (var def in stateDefinitions)
        {
            if (def == null)
            {
                Debug.LogError("❌ Empty StateDefinition found!");
                continue;
            }

            if (values.ContainsKey(def.stateID))
            {
                Debug.LogWarning($"⚠ Duplicate State ID: {def.stateID}");
                continue;
            }

            values[def.stateID] = def.initialValue;
        }
    }

    public float Get(string id)
    {
        if (values == null)
        {
            Debug.LogError("❌ values not initialized!");
            return 0f;
        }

        if (!values.ContainsKey(id))
        {
            Debug.LogError($"❌ State does not exist: {id}");
            return 0f;
        }

        return values[id];
    }

    public void Modify(string id, float amount)
    {
        if (values == null || !values.ContainsKey(id))
        {
            Debug.LogError($"❌ Modify failed, state does not exist: {id}");
            return;
        }

        values[id] += amount;
        values[id] = Mathf.Clamp(values[id], 0, 100);

        // 🔥 核心：广播状态变化
        OnStateChanged?.Invoke(id, values[id]);
    }

    public Dictionary<string, float> GetAllStates()
    {
        return values;
    }
}