using UnityEngine;
using UnityEngine.InputSystem;

public class StandInLineState : FSMBaseState
{
    //bool firstInLine = false;
    float distanceToTarget;
    public override void EnterState(CustomerManager agent)
    {
        QueueManager.Instance.JoinQueue(agent);
    }

    public override void UpdateState(CustomerManager agent)
    {
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.currentQueuePos);
        agent.navigation.SetDestination(agent.currentQueuePos);
        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;

        if (distanceToTarget < 0.2f)
        {
            if (agent.assignedSlot == 0)
            {
                agent.SwitchState(agent.plingInLineState);
            }
        }

    }
}
