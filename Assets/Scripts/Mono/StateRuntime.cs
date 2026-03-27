using System.Collections.Generic;
using UnityEngine;

public class StateRuntime : MonoBehaviour
{
    public List<StateDefinition> stateDefinitions;

    private Dictionary<string, float> values;

    void Awake()
    {
        values = new Dictionary<string, float>();

        if (stateDefinitions == null || stateDefinitions.Count == 0)
        {
            Debug.LogError("❌ StateRuntime: 没有配置 stateDefinitions！");
            return;
        }

        foreach (var def in stateDefinitions)
        {
            if (def == null)
            {
                Debug.LogError("❌ 有空的 StateDefinition！");
                continue;
            }

            if (values.ContainsKey(def.stateID))
            {
                Debug.LogWarning($"⚠ 重复状态ID: {def.stateID}");
                continue;
            }

            values[def.stateID] = def.initialValue;
        }
    }

    public float Get(string id)
    {
        if (values == null)
        {
            Debug.LogError("❌ values 未初始化！");
            return 0f;
        }

        if (!values.ContainsKey(id))
        {
            Debug.LogError($"❌ 状态不存在: {id}");
            return 0f;
        }

        return values[id];
    }

    public void Modify(string id, float amount)
    {
        if (values == null || !values.ContainsKey(id))
        {
            Debug.LogError($"❌ Modify失败，状态不存在: {id}");
            return;
        }

        values[id] += amount;
        values[id] = Mathf.Clamp(values[id], 0, 100);
    }

    public Dictionary<string, float> GetAllStates()
    {
        return values;
    }
}