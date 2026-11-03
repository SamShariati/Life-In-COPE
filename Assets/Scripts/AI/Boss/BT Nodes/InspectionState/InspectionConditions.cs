using UnityEngine;

public class InspectionConditions : BossBTNode
{
    public override NodeState Evaluate(BossManager agent)
    {
        
        if (StateConditions(agent))
        {
            agent.BB.Inspection.stateActivated = true;
            RollInspectionType(agent);

            return NodeState.SUCCESS;
        }
        else if (agent.BB.Inspection.stateActivated)
        {

            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }








    }

    private void RollInspectionType(BossManager agent)
    {
        float randPercent = Random.Range(0f, 100f);

        //agent.BB.Inspection.isCurrInspectingShelf = true;
        //agent.BB.Inspection.isCurrInspectingTrash = true;

        //// 50, 30, 20?
        if (randPercent < 70f)
        {
            agent.BB.Inspection.isCurrInspectingShelf = true;
        }
        else
        {
            agent.BB.Inspection.isCurrInspectingTrash = true;
        }

    }

    private bool StateConditions(BossManager agent)
    {
        if (agent.BB.Inspection.stateAllowed && !agent.BB.Inspection.stateActivated)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
