using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BossSequence : BossBTNode
{
    protected List<BossBTNode> nodes = new List<BossBTNode>();

    public BossSequence(List<BossBTNode> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate(BossManager agent)
    {

        foreach (BossBTNode node in nodes)
        {
            switch (node.Evaluate(agent))
            {
                case NodeState.RUNNING:
                    _nodeState = NodeState.RUNNING;
                    return _nodeState;

                case NodeState.SUCCESS:
                    break;

                case NodeState.FAILURE:
                    _nodeState = NodeState.FAILURE;
                    return _nodeState;

                default:
                    break;
            }

        }
        _nodeState = NodeState.SUCCESS;
        return _nodeState;
    }
}
