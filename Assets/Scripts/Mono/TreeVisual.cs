using UnityEngine;

public class TreeVisual : MonoBehaviour
{
    private Vector3 originalPos;
    private float shakeTimer = 0f;
    public float shakeAmount = 0.15f; // 晃动幅度，觉得不明显可以调大
    public float shakeDuration = 0.2f; // 晃动持续时间

    void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            // 随机位移产生抖动
            transform.position = originalPos + (Vector3)Random.insideUnitCircle * shakeAmount;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.position = originalPos; // 恢复原位
        }
    }

    // 撞树时由玩家代码调用这个方法
    public void Shake()
    {
        shakeTimer = shakeDuration;
    }
}
