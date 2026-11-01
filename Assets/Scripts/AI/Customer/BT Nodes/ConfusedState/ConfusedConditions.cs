using UnityEngine;

public class ConfusedConditions : CustomerBTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.BB.IKRigging.IKRiggingEnabled = true;
            agent.BB.Confused.confusedStateActivated = true;
            agent.BB.Sabotage.sabotageStateAllowed = false;

            RollConfusedType(agent);

            return NodeState.SUCCESS;
        }

        else if (agent.BB.Confused.confusedStateActivated)
        {
            
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

    

    // Checking to see if AI enters PatroleAisle- or CheckWrongShelf State.
    
    private bool StateConditions(CustomerManager agent)
    {
        if (agent.BB.Confused.confusedStateAllowed && !agent.BB.Confused.confusedStateActivated)
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

    // Checking to see if the Confused State should be activated or not based on Customer Personality.
    private bool RollConfusedChance(CustomerManager agent)
    {
        float randPercent = Random.Range(0, 100);

        if (randPercent <= agent.confusedChance)
        {
            agent.BB.Confused.confusedStateAllowed = true;
            return true;

        }
        else
        {
            agent.BB.Confused.confusedStateAllowed = false;
            return false;
        }

    }

    private void RollConfusedType(CustomerManager agent)
    {
        float randPercent = Random.Range(0, 100);

        if (randPercent < 66.7f)
        {
            agent.BB.Confused.isCurrentlyPatrolling = true;
        }
        else
        {
            agent.BB.Confused.isCurrCheckingWrongShelf = true;
        }
            
    }
}
