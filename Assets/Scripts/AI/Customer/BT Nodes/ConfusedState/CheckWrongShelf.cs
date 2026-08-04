using UnityEngine;

public class CheckWrongShelf : BTNode
{

    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.checkWrongShelf;

        return NodeState.SUCCESS;
    }
}
