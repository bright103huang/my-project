using UnityEngine;

[CreateAssetMenu(menuName = "Game/Condition/Distance")]
public class DistanceCondition : Condition
{
    public string playerTag = "Player";
    public string targetTag = "Tree";
    public float maxDistance = 0.5f;

    public override bool Evaluate(StateRuntime runtime)
    {
        var player = GameObject.FindGameObjectWithTag(playerTag);
        var target = GameObject.FindGameObjectWithTag(targetTag);
        
        if (player == null || target == null) 
        {
            Debug.LogWarning($"DistanceCondition: Player ({playerTag}) or Target ({targetTag}) not found.");
            return false;
        }

        float dist = Vector3.Distance(player.transform.position, target.transform.position);
        // Debug.Log($"DistanceCondition: dist = {dist}");
        return dist <= maxDistance;
    }
}
