using UnityEngine;

public class GoToPlayerConditions : BossBTNode
{

    public override NodeState Evaluate(BossManager agent)
    {
        
        if (!agent.BB.StoreNav.reachedPlayerDestination)
        {
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }
    }
}
