using UnityEngine;
using UnityEngine.AI;

public class ChasePlayer : BTNode
{
    float distanceToTarget;

    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.chasePlayer;

        distanceToTarget = Vector3.Distance(agent.player.position, agent.transform.position);

        agent.navigation.speed = agent.runSpeed;
        agent.navigation.isStopped = false;

        agent.animator.SetState(AnimState.Chase);

        agent.C_Functions.CalculatePlayerDestination();

        if (distanceToTarget < 1.5f)
        {
            agent.BB.SFP.isCurrentlyChasing = false;
            agent.BB.SFP.isCurrentlyFollowing = true;

            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.RUNNING;
        }
    }

    
}