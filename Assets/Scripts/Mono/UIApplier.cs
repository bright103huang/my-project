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
        DebugStateUI debugUI = FindObjectOfType<DebugStateUI>();
        
        if (debugUI != null)
        {
            // 如果有 DebugStateUI，将提示信息显示在调试界面中
            debugUI.ShowActionHint(request.message);
        }
        else if (hintText != null)
        {
            // 如果没有 DebugStateUI，使用传统的 hintText
            hintText.text = request.message;
            hintText.gameObject.SetActive(true);
        }

        yield return GetWait(request.duration);

        if (debugUI != null)
        {
            debugUI.HideActionHint();
        }
        else if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }

        currentRequest = null;
    }
}
