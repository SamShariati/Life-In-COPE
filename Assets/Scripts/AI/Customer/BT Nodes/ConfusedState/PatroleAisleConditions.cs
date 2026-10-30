using UnityEngine;
using UnityEngine.AI;

public class PatroleAisleConditions : CustomerBTNode
{
    

    public override NodeState Evaluate(CustomerManager agent)
    {
        if (StateConditions(agent))
        {
            agent.BB.Confused.patroleRouteChosen = true;
            agent.C_Functions.GetCorrectAisle();
            ChooseAislePatrolePoint(agent);

            return NodeState.SUCCESS;
        }
        else if (agent.BB.Confused.isCurrentlyPatrolling)
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
        if (!agent.BB.Confused.patroleRouteChosen && agent.BB.Confused.isCurrentlyPatrolling)
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
        int randomIndex = Random.Range(0, agent.BB.Confused.allAislePositions[agent.BB.Confused.aisleID].Count);
        agent.BB.Confused.chosenAislePos = agent.BB.Confused.allAislePositions[agent.BB.Confused.aisleID][randomIndex].position;
    }
}
