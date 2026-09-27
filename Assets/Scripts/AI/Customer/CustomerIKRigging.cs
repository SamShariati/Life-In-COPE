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

    private float targetWeight01;

    public CustomerIKRigging(CustomerManager agent)
    {
        this.agent = agent;
    }


    private bool IKRiggingAllowed()
    {
        float distance = Vector3.Distance(agent.transform.position, agent.player.transform.position);

        if (agent.IKRiggingAllowed2 && distance < agent.playerAwernessRange)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    public void UpdateIK()
    {

        targetWeight01 = IKRiggingAllowed() ? 1f - Mathf.Clamp01(CalculatePlayerDotProduct() / DeadZoneDot) : 0f;


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
