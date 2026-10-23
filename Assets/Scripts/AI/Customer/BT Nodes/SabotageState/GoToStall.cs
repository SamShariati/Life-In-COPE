using UnityEngine;

public class GoToStall : BTNode
{
    float distanceToTarget;

    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.animator.SetState(AnimState.Walk);
        agent.navigation.isStopped = false;
        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.SetDestination(agent.chosenFoodStallPos.position);

        distanceToTarget = Vector3.Distance(agent.transform.position, agent.chosenFoodStallPos.position);

        if (distanceToTarget < 0.25f)
        {
            agent.isCurrentlyGoingToStall = false;
            agent.isCurrentlyHittingStall = true;
            return NodeState.SUCCESS;
        }
        return NodeState.RUNNING;
    }
}
