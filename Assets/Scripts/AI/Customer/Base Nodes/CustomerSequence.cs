using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerSequence : CustomerBTNode
{
    protected List<CustomerBTNode> nodes = new List<CustomerBTNode>();

    public CustomerSequence(List<CustomerBTNode> nodes)
    {
        this.nodes = nodes;
    }

    public override NodeState Evaluate(CustomerManager agent)
    {

        foreach (CustomerBTNode node in nodes)
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
