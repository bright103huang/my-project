using UnityEngine;

public class TreeVisual : MonoBehaviour
{
    private Vector3 originalPos;
    private float shakeTimer = 0f;

    public float shakeAmount = 0.15f;
    public float shakeDuration = 0.2f;

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = originalPos;
        }
    }

    // ✅ 改名：不再对外暴露“Shake”，而是响应事件
    public void OnHitTree()
    {
        shakeTimer = shakeDuration;
    }
}
