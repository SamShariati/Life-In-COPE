
using UnityEngine;

public class GetHitConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {

        if (StateConditions(agent))
        {
            agent.BB.IKRigging.IKRiggingEnabled = false;
            agent.BB.GetHit.getHitStateActivated = true;
            agent.C_Functions.GetHitResetFlags();
            CalculateImpactDotProduct(agent);
            

            return NodeState.SUCCESS;
        }

        else if (agent.BB.GetHit.getHitStateActivated)
        {

            return NodeState.SUCCESS;
        }

        // Used to make sure that once FSM states are activated, GetHitState can still be activated, But will disable BT again once it is finished.
        else if (agent.AI.FSMActivated)
        {
            agent.AI.BTActivated = false;
            return NodeState.FAILURE;
        }

        else
        {
            return NodeState.FAILURE;
        }
    }

    private bool StateConditions(CustomerManager agent)
    {
        if (agent.BB.GetHit.getHitStateAllowed && !agent.BB.GetHit.getHitStateActivated && agent.BB.GetHit.gotHitByBox &&
            !agent.BB.GetHit.isCurrFallingBackward && !agent.BB.GetHit.isCurrFallingForward )
        {

            //Completely disables the sabotage behaviour if agent gets hit while sabotaging.
            if (agent.BB.Sabotage.recentlyHitFood)
            {
                agent.BB.Sabotage.sabotageStateDisabledByHit = true;
            }

            return true;
        }
        else
        {
            agent.BB.GetHit.gotHitByBox = false;
            return false;
        }
    }

    private void CalculateImpactDotProduct(CustomerManager agent)
    {

        Vector3 agentForward = agent.transform.forward;
        agentForward.y = 0f;
        agentForward.Normalize();

        agent.BB.GetHit.dotProduct = Vector3.Dot(agent.BB.GetHit.thrownDirection, agentForward);

        if (agent.BB.GetHit.dotProduct < 0) // - är träffad framifrån, + är träffad bakifrån
        {
            agent.BB.GetHit.isCurrFallingBackward = true;
        }
        else
        {
            agent.BB.GetHit.isCurrFallingForward = true;
        }
    }


}
