using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class UIApplier : MonoBehaviour
{
    public TextMeshProUGUI hintText;

    private UIRequest currentRequest;
    private Coroutine currentRoutine;

    public void Apply(UIRequest request)
    {
        // ���ȼ��ж�
        if (currentRequest != null && request.priority < currentRequest.priority)
            return;

        currentRequest = request;

        // ͣ��֮ǰ����ʾ
        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowMessage(request));
    }

    private Dictionary<float, WaitForSeconds> waitCache = new Dictionary<float, WaitForSeconds>();

    private WaitForSeconds GetWait(float seconds)
    {
        if (!waitCache.ContainsKey(seconds))
            waitCache[seconds] = new WaitForSeconds(seconds);
        return waitCache[seconds];
    }

    private IEnumerator ShowMessage(UIRequest request)
    {
        hintText.text = request.message;
        hintText.gameObject.SetActive(true);

        yield return GetWait(request.duration);

        hintText.gameObject.SetActive(false);
        currentRequest = null;
    }
}
