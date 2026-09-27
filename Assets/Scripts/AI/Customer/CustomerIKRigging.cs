using UnityEngine;
using UnityEngine.Animations.Rigging;

public class CustomerIKRigging
{
    private readonly CustomerManager agent;

    // "Full" weight each constraint should reach when the player isn't behind the agent.
    // Pull these from CustomerManager instead if you want them tunable per-customer/in the Inspector.
    private const float ChestBaseWeight = 0.4f;
    private const float HeadBaseWeight = 1f;
    private const float DeadZoneDot = 0.9f;

    // How quickly the weight chases its target value, in seconds. Higher = smoother/slower.
    private const float SmoothTime = 0.15f;

    private float chestWeightVelocity;
    private float headWeightVelocity;

    public CustomerIKRigging(CustomerManager agent)
    {
        this.agent = agent;
    }

    /// <summary>
    /// Call this once per frame (e.g. from CustomerManager.Update()).
    /// CustomerIKRigging is a plain class, not a MonoBehaviour, so nothing calls
    /// this automatically - the hub has to drive it.
    /// </summary>
    public void UpdateIK()
    {
        if (agent.chestRig == null || agent.headRig == null) return;

        // Uses agent.CalculatePlayerDotProduct() as currently written, where:
        // dot == 1  -> player considered "behind"          -> weight should be 0
        // dot <= 0  -> player considered "in front/to the side" -> weight should be 1
        //
        // If you switch to the position-based dot product instead (1 = front, -1 = behind),
        // use: float targetWeight01 = Mathf.Clamp01(dot); (no "1 - ")
        float dot = CalculatePlayerDotProduct();
        float targetWeight01 = 1f - Mathf.Clamp01(dot/ DeadZoneDot);

        agent.chestRig.weight = Mathf.SmoothDamp(
            agent.chestRig.weight,
            targetWeight01 * ChestBaseWeight,
            ref chestWeightVelocity,
            SmoothTime);

        agent.headRig.weight = Mathf.SmoothDamp(
            agent.headRig.weight,
            targetWeight01 * HeadBaseWeight,
            ref headWeightVelocity,
            SmoothTime);
    }

    private float CalculatePlayerDotProduct()
    {
        Vector3 toPlayer = agent.player.transform.position - agent.transform.position;
        toPlayer.y = 0f;
        toPlayer.Normalize();

        Vector3 agentForward = agent.transform.forward;
        agentForward.y = 0f;
        agentForward.Normalize();

        // Dot with -agentForward instead of agentForward so the sign matches
        // your original convention: 1 = player behind, -1 = player in front.
        return Vector3.Dot(toPlayer, -agentForward);
    }
}
