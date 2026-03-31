using UnityEngine;

public class ActionRequester : MonoBehaviour
{
    public ActionExecutor executor;

    public void Request(string actionID)
    {
        executor.ExecuteByID(actionID);
    }
}
