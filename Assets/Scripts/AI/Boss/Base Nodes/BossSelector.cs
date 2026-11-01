using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BossSelector : BossBTNode
{
    protected List<BossBTNode> nodes = new List<BossBTNode>();

    public BossSelector(List<BossBTNode> nodes)
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
                    _nodeState = NodeState.SUCCESS;
                    return _nodeState;

                case NodeState.FAILURE:
                    break;

                default:
                    break;
            }

        }
        _nodeState = NodeState.FAILURE;
        return _nodeState;
    }
}
