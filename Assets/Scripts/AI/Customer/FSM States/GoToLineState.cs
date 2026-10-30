using UnityEngine;
using UnityEngine.InputSystem;

public class GoToLineState : CustomerFSMBaseState
{

    float distanceToTarget;
    Vector3 targetPos;
    public override void EnterState(CustomerManager agent)
    {
        agent.BB.IKRigging.IKRiggingEnabled = true;

        targetPos = agent.BB.Store.walkToRegisterPos;
        agent.AI.FSMActivated = true;

        

    }
    public override void UpdateState(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.goToLine;

        agent.animator.SetState(AnimState.Walk);
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.BB.Store.walkToRegisterPos);
        agent.navigation.SetDestination(targetPos);

        if (distanceToTarget < 0.5f)
        {
            agent.SwitchState(agent.AI.FSM.standInLineState);
        }

        // In case agent needs to enter GetHitState when FSM is activated.
        else if (distanceToTarget > 0.5f && agent.BB.GetHit.gotHitByBox)
        {
            agent.AI.BTActivated = true;
        }

    }
}


