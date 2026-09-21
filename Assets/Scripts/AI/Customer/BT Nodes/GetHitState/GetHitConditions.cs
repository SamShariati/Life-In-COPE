
using UnityEngine;

public class GetHitConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.getHitConditions;

        if (StateConditions(agent))
        {
            agent.getHitStateActivated = true;
            agent.C_Functions.GetHitResetFlags();
            CalculateImpactDotProduct(agent);
            

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
            !agent.isCurrFallingBackward && !agent.isCurrFallingForward )
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

        Vector3 agentForward = agent.transform.forward;
        agentForward.y = 0f;
        agentForward.Normalize();

        agent.dotProduct = Vector3.Dot(agent.thrownDirection, agentForward);

        if (agent.dotProduct < 0) // - är träffad framifrån, + är träffad bakifrån
        {
            agent.isCurrFallingBackward = true;
        }
        else
        {
            agent.isCurrFallingForward = true;
        }
    }

    private void CheckIfBoxOnGround(CustomerManager agent)
    {

    }

}
