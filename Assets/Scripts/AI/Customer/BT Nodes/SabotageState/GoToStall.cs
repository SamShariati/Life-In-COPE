using UnityEngine;

public class GoToStall : BTNode
{
    float distanceToTarget;

    public override NodeState Evaluate(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.goToStall;

        agent.animator.SetState(AnimState.Walk);
        agent.navigation.isStopped = false;
        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.SetDestination(agent.BB.Sabotage.chosenFoodStallPos.position);

        distanceToTarget = Vector3.Distance(agent.transform.position, agent.BB.Sabotage.chosenFoodStallPos.position);

        if (distanceToTarget < 0.25f)
        {
            agent.BB.Sabotage.isCurrentlyGoingToStall = false;
            agent.BB.Sabotage.isCurrentlyHittingStall = true;
            return NodeState.SUCCESS;
        }
        return NodeState.RUNNING;
    }
}
