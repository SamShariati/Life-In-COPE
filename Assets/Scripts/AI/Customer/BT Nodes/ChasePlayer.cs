using UnityEngine;

public class ChasePlayer : BTNode
{
    float distanceToTarget;

    public override NodeState Evaluate(CustomerManager agent)
    {
        distanceToTarget = Vector3.Distance(agent.player.position, agent.transform.position);

        agent.navigation.speed = agent.runSpeed;
        agent.navigation.isStopped = false;
        agent.navigation.SetDestination(agent.player.position);
        agent.animator.SetState(AnimState.Chase);

        if (distanceToTarget < 0.1f)
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
