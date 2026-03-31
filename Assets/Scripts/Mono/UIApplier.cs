using UnityEngine;
using TMPro;
using System.Collections;

public class UIApplier : MonoBehaviour
{
    public TextMeshProUGUI hintText;

    private UIRequest currentRequest;
    private Coroutine currentRoutine;

    public void Apply(UIRequest request)
    {
        // 优先级判断
        if (currentRequest != null && request.priority < currentRequest.priority)
            return;

        currentRequest = request;

        // 停掉之前的显示
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessage(request));
    }

    private IEnumerator ShowMessage(UIRequest request)
    {
        hintText.text = request.message;
        hintText.gameObject.SetActive(true);

        yield return new WaitForSeconds(request.duration);

        hintText.gameObject.SetActive(false);
        currentRequest = null;
    }
}
