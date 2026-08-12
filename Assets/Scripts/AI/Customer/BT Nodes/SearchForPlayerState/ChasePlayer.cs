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

        if (distanceToTarget < 2f)
        {
            agent.isCurrentlyChasing = false;
            agent.isCurrentlyFollowing = true;

            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.RUNNING;
        }
    }

    
}