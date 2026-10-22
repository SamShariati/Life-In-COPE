using UnityEngine;

public class SabotageConditions : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        
        if (Stateconditions(agent))
        {

            return NodeState.SUCCESS;

        }

        else if (agent.sabotageStateActivated)
        {

            return NodeState.SUCCESS;

        }

        else
        {

            return NodeState.FAILURE;

        }

    }

    private bool Stateconditions(CustomerManager agent)
    {

        if (agent.sabotageStateAllowed && !agent.sabotageStateActivated && !agent.sabotageStateDisabledByHit &&
            agent.C_Functions.CheckSabotageCD())
        {

            if (RollSabotageChance(agent))
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }

    }

    private bool RollSabotageChance(CustomerManager agent)
    {
        float randPercent = Random.Range(0f, 100f);
        agent.C_Functions.ResetSabotageStateCD();

        if (randPercent <= agent.sabotageChance)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

}
