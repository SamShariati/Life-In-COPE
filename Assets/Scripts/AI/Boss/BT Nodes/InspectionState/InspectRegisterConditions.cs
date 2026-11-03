using UnityEngine;

public class InspectRegisterConditions : BossBTNode
{
    public override NodeState Evaluate(BossManager agent)
    {
        
        if (agent.BB.Inspection.isCurrInspectingRegister)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }

}
