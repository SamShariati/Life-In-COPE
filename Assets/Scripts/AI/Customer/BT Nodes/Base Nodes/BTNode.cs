using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public abstract class BTNode
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

