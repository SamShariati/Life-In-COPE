using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Android;

public class InspectTrashConditions : BossBTNode
{

    public override NodeState Evaluate(BossManager agent)
    {
        
        if (StateConditions(agent))
        {
            agent.Functions.ChooseTrashToInspect();
            agent.BB.Inspection.trashToInspectChosen = true;

            return NodeState.SUCCESS;
        }
        else if (agent.BB.Inspection.isCurrInspectingTrash)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }

    }
    
    private bool StateConditions(BossManager agent)
    {
        if (!agent.BB.Inspection.trashToInspectChosen && agent.BB.Inspection.isCurrInspectingTrash)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    

}
