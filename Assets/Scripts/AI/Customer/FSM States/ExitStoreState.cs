using UnityEngine;

public class ExitStoreState : CustomerFSMBaseState
{
    Vector3 targetPosition;
    float distanceToTarget;

    public override void EnterState(CustomerManager agent)
    {
        agent.BB.IKRigging.IKRiggingEnabled = true;

        targetPosition = agent.BB.Store.exitStorePos;
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

        if (agent.BB.GetHit.gotHitByBox)
        {
            agent.AI.BTActivated = true;
        }
    }
}
