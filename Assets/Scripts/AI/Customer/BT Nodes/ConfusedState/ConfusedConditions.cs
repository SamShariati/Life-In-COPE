using UnityEngine;

public class ConfusedConditions : BTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.confusedStateActivated = true;
            agent.C_Functions.ChooseShelfRoute(agent);
            RollConfusedType(agent);

            return NodeState.SUCCESS;
        }

        else if (agent.confusedStateActivated)
        {
            
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    // Checking to see if the Confused State should be activated or not based on Customer Personality.
    private bool RollConfusedChance(CustomerManager agent)
    {
        float randPercent = Random.Range(0, 100);

        if (randPercent <= agent.confusedChance)
        {
            agent.confusedStateAllowed = true;
            return true;

        }
        else
        {
            agent.confusedStateAllowed = false;
            return false;
        }

    }

    // Checking to see if AI enters PatroleAisle- or CheckWrongShelf State.
    private void RollConfusedType(CustomerManager agent)
    {
        agent.isCurrentlyPatrolling = true; //ska slumpas sen.
    }
    private bool StateConditions(CustomerManager agent)
    {
        if (agent.confusedStateAllowed && !agent.confusedStateActivated) // +RollConfusedChance()
        {
            if (RollConfusedChance(agent))
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
}
