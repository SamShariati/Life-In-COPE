using UnityEngine;

public class ExitStoreState : FSMBaseState
{
    Vector3 targetPosition;
    float distanceToTarget;

    public override void EnterState(CustomerManager agent)
    {
        targetPosition = agent.exitStorePos;
    }

    public override void UpdateState(CustomerManager agent)
    {

        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;
        agent.navigation.SetDestination(targetPosition);

        distanceToTarget = Vector3.Distance(agent.transform.position, targetPosition);

        if (distanceToTarget < 0.4f)
        {
            Object.Destroy(agent.gameObject);
        }
        else
        {
            agent.animator.SetState(AnimState.Walk);
        }

    }
}
