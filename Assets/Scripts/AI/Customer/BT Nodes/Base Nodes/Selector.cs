using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Selector : BTNode
{
    protected List<BTNode> nodes = new List<BTNode>();

    public Selector(List<BTNode> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate(CustomerManager agent)
    {

        foreach (BTNode node in nodes)
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
