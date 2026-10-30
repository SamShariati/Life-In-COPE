using UnityEngine;

public class SabotageConditions : CustomerBTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        
        if (Stateconditions(agent))
        {
            agent.BB.Sabotage.sabotageStateActivated = true;
            return NodeState.SUCCESS;

        }

        else if (agent.BB.Sabotage.sabotageStateActivated)
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

        if (agent.BB.Sabotage.sabotageStateAllowed && !agent.BB.Sabotage.sabotageStateActivated && !agent.BB.Sabotage.sabotageStateDisabledByHit &&
            agent.C_Functions.CheckSabotageCD())
        {

            if (RollSabotageChance(agent))
            {
                return true;
            }
            else
            {
                agent.BB.Sabotage.sabotageStateAllowed = false;
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
