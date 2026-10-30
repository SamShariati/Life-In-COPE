using UnityEngine;

public abstract class CustomerBTNode
{
    protected NodeState _nodeState;
    //protected Transform targetedPlayer;

    public NodeState nodeState { get { return _nodeState; } }

    public abstract NodeState Evaluate(CustomerManager agent);
}

public enum NodeState
{

    RUNNING, SUCCESS, FAILURE,
}
