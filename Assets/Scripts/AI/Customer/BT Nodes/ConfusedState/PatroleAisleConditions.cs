using UnityEngine;
using UnityEngine.AI;

public class PatroleAisleConditions : BTNode
{
    

    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.patroleAisleConditions;
        if (StateConditions(agent))
        {
            agent.patroleRouteChosen = true;
            agent.C_Functions.GetCorrectAisle();
            ChooseAislePatrolePoint(agent);

            return NodeState.SUCCESS;
        }
        else if (agent.isCurrentlyPatrolling)
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
        if (!agent.patroleRouteChosen && agent.isCurrentlyPatrolling)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ChooseAislePatrolePoint(CustomerManager agent)
    {
        int randomIndex = Random.Range(0, agent.allAislePositions[agent.aisleID].Count);
        agent.chosenAislePos = agent.allAislePositions[agent.aisleID][randomIndex].position;
    }
}
