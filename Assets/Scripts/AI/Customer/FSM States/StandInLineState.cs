using UnityEngine;
using UnityEngine.InputSystem;

public class StandInLineState : CustomerFSMBaseState
{
    //bool firstInLine = false;
    float distanceToTarget;


    public override void EnterState(CustomerManager agent)
    {
        if (agent.BB.GSI.goodsGathered.Count > 0)
        {
            QueueManager.Instance.JoinQueue(agent);
        }
        else
        {
            agent.SwitchState(agent.AI.FSM.exitStoreState);
        }
     
    }

    public override void UpdateState(CustomerManager agent)
    {



        if (agent.BB.CashRegister.assignedQueueSlot == -1)
        {
            QueueManager.Instance.JoinQueue(agent);
        }

        distanceToTarget = Vector3.Distance(agent.transform.position, agent.BB.CashRegister.currentQueuePos);
        agent.navigation.SetDestination(agent.BB.CashRegister.currentQueuePos);
        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;

        if (distanceToTarget < 0.2f)
        {
            agent.animator.SetState(AnimState.Idle);
            if (agent.BB.CashRegister.assignedQueueSlot == 0)
            {
                agent.SwitchState(agent.AI.FSM.firstInLineState);
            }
            
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
