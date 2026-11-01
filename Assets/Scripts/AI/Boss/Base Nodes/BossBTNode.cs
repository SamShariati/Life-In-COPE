using UnityEngine;

public abstract class BossBTNode
{
    protected NodeState _nodeState;
    //protected Transform targetedPlayer;

    public NodeState nodeState { get { return _nodeState; } }

    public abstract NodeState Evaluate(BossManager agent);
}

