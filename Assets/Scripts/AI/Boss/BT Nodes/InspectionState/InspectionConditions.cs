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

        agent.BB.Inspection.isCurrInspectingShelf = true;

        //// 50, 30, 20?
        //if (randPercent < 50f)
        //{
        //    agent.BB.Inspection.inspectShelfChosen = true;
        //}
        //else if (randPercent >= 50f && randPercent < 80f)
        //{
        //    agent.BB.Inspection.inspectTrashChosen = true;
        //}
        //else
        //{
        //    agent.BB.Inspection.inspectRegLineChosen = true;
        //}
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
