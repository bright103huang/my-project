using UnityEngine;

public class OutputController : MonoBehaviour
{
    public StateRuntime runtime;
    public PlayerController2D player;
    public Animator anim;

    private bool isConvulsing = false;

    void Update()
    {
        if (runtime == null || player == null || anim == null) return;

        float fatigue = runtime.Get("Fatigue");
        float energy = runtime.Get("Energy");

        // ⭐ 抽搐（最高优先级）
        if (fatigue >= 90)
        {
            if (!isConvulsing)
            {
                isConvulsing = true;

                player.SetLocked(true);

                anim.ResetTrigger("Convulse");
                anim.SetTrigger("Convulse");

                player.SetMessage("生活不只有撞树，还有躺平。");
            }
            return;
        }

        // ⭐ 恢复
        if (isConvulsing && fatigue < 40)
        {
            isConvulsing = false;

            player.SetLocked(false);

            anim.Play("Idle", 0, 0f);

            player.SetMessage("你居然活过来了，医学奇迹。");
        }

        // ⭐ 能量不足
        if (energy < 10)
        {
            player.SetMessage($"你现在只有 {energy:F0} 点能量，人是铁，饭是钢。");
        }
        else if (fatigue > 80)
        {
            player.SetMessage("你离抽搐只差一棵树的距离。");
        }
        else
        {
            player.SetMessage("Standing still is the only safe move.");
        }
    }
}
