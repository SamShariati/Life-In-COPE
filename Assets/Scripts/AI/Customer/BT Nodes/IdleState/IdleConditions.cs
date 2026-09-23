using UnityEngine;

public class IdleConditions : BTNode
{
    
    public override NodeState Evaluate(CustomerManager agent)
    {

        if (agent.idleStateAllowed && !agent.idleStateActivated)
        {

            agent.idleStateActivated = true;
            agent.C_Functions.SetTimer(agent.minIdleTime);

            return NodeState.SUCCESS;


        }
        else if (StateConditions(agent))
        {
            return NodeState.SUCCESS;
        }

        else
        {
            agent.idleStateAllowed = false;
            agent.idleStateActivated = false;
            return NodeState.FAILURE;
        }




    }

    //I create this in case other states need to interact with idle in the future other than GetHitState
    private bool StateConditions(CustomerManager agent)
    {

        if (agent.idleStateActivated && !agent.gotHitByBox)
        {
            return true;
        }
        else
        {
            return false;
        } 
    }
}
