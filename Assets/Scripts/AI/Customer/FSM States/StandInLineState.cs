using UnityEngine;
using UnityEngine.InputSystem;

public class StandInLineState : FSMBaseState
{
    //bool firstInLine = false;
    float distanceToTarget;

    public override void EnterState(CustomerManager agent)
    {
        if (agent.goodsGathered.Count > 0)
        {
            QueueManager.Instance.JoinQueue(agent);
        }
        else
        {
            agent.SwitchState(agent.exitStoreState);
        }
     
    }

    public override void UpdateState(CustomerManager agent)
    {
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.currentQueuePos);
        agent.navigation.SetDestination(agent.currentQueuePos);
        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;

        if (distanceToTarget < 0.2f)
        {
            agent.animator.SetState(AnimState.Idle);
            if (agent.assignedQueueSlot == 0)
            {
                agent.SwitchState(agent.plingInLineState);
            }
        }
        else
        {
            agent.animator.SetState(AnimState.Walk);
        }

    }
}
