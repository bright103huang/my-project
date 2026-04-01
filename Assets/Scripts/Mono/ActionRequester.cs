using UnityEngine;

public class ActionRequester : MonoBehaviour
{
    public ActionExecutor executor;

    void Awake()
    {
        if (executor == null) executor = GetComponent<ActionExecutor>();
        if (executor == null) executor = GetComponentInParent<ActionExecutor>();
    }

    public void Request(string actionID)
    {
        if (executor != null)
            executor.ExecuteByID(actionID);
    }
}
