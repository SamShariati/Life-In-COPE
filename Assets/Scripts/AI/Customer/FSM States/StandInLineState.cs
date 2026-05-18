using UnityEngine;
using UnityEngine.InputSystem;

public class StandInLineState : FSMBaseState
{
    bool firstInLine = false;
    float distanceToTarget;
    public override void EnterState(CustomerManager agent)
    {
        //QueueManager.Instance.JoinQueue(agent);
        agent.navigation.SetDestination(agent.walkToRegisterPos);
    }

    public override void UpdateState(CustomerManager agent)
    {
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.walkToRegisterPos);
        agent.navigation.SetDestination(agent.currentQueuePos);






        if (agent.navigation.pathPending || distanceToTarget < 0.1f)
        {
            if (agent.assignedSlot == 0)
            {
                firstInLine = true;
            }
        }

        if (firstInLine && Keyboard.current.xKey.wasPressedThisFrame)
        {
            QueueManager.Instance.AdvanceQueue(agent);
            agent.SwitchState(agent.enterStoreState);
        }

    }
}
