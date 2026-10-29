using UnityEngine;

public class IdleConditions : BTNode
{
    
    public override NodeState Evaluate(CustomerManager agent)
    {

        if (agent.BB.Idle.idleStateAllowed && !agent.BB.Idle.idleStateActivated)
        {
            agent.BB.IKRigging.IKRiggingEnabled = true;
            agent.BB.Idle.idleStateActivated = true;
            agent.C_Functions.SetTimer(Random.Range(2f, 3f));

            return NodeState.SUCCESS;


        }
        else if (StateConditions(agent))
        {
            return NodeState.SUCCESS;
        }

        else
        {
            agent.BB.Idle.idleStateAllowed = false;
            agent.BB.Idle.idleStateActivated = false;
            return NodeState.FAILURE;
        }




    }

    //I create this in case other states need to interact with idle in the future other than GetHitState
    private bool StateConditions(CustomerManager agent)
    {

        if (agent.BB.Idle.idleStateActivated && !agent.BB.GetHit.gotHitByBox)
        {
            return true;
        }
        else
        {
            return false;
        } 
    }
}
