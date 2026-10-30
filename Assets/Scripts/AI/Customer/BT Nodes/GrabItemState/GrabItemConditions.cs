using UnityEngine;

public class GrabItemConditions : CustomerBTNode
{
    public override NodeState Evaluate(CustomerManager agent)
    {

        if (agent.BB.GSI.shelfStateAllowed)
        {
            agent.BB.Sabotage.sabotageStateAllowed = false;
            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }

    }
}
