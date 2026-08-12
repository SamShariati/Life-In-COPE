
using UnityEngine;

public class GetHitConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.getHitStateActivated = true;
            CalculateImpactDotProduct(agent);
            agent.C_Functions.CalculateRotationAngle();

            return NodeState.SUCCESS;
        }

        else if (agent.getHitStateActivated)
        {

            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    private bool StateConditions(CustomerManager agent)
    {
        if (agent.getHitStateAllowed && !agent.getHitStateActivated && agent.gotHitByBox &&
            agent.C_Functions.CheckGetHitStateCD())
        {
            return true;
        }
        else
        {
            agent.gotHitByBox = false;
            return false;
        }
    }

    private void CalculateImpactDotProduct(CustomerManager agent)
    {

        Vector3 boxVelocity = agent.collidingBoxRB.linearVelocity;
        boxVelocity.y = 0f;

        Vector3 agentForward = agent.transform.forward;
        agentForward.y = 0f;

        boxVelocity.Normalize();
        agentForward.Normalize();

        agent.dotProduct = Vector3.Dot(boxVelocity, agentForward);

        if (agent.dotProduct < 0) // - är träffad framifrån, + är träffad bakifrån
        {
            agent.isCurrFallingBackward = true;
        }
        else
        {
            agent.isCurrFallingForward = true;
        }
    }

}
