using Unity.VisualScripting;
using UnityEngine;

public class InspectShelfConditions : BossBTNode
{

    public override NodeState Evaluate(BossManager agent)
    {
        
        if (StateConditions(agent))
        {
            agent.BB.Inspection.shelfToInspectChosen = true;
            ChooseShelfToInspect(agent);
            return NodeState.SUCCESS;
        }
        else if (agent.BB.Inspection.isCurrInspectingShelf)
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

        if (!agent.BB.Inspection.shelfToInspectChosen && agent.BB.Inspection.isCurrInspectingShelf)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ChooseShelfToInspect(BossManager agent)
    {

        int randNr = Random.Range(0, ShelfManager.Instance.shelfList.Count);

        agent.BB.Inspection.chosenShelf = ShelfManager.Instance.shelfList[(randNr)];

    }

}
