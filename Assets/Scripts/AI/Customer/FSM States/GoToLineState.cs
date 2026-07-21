using UnityEngine;
using UnityEngine.InputSystem;

public class GoToLineState : FSMBaseState
{

    float distanceToTarget;
    Vector3 targetPos;
    public override void EnterState(CustomerManager agent)
    {
        targetPos = agent.walkToRegisterPos;

    }
    public override void UpdateState(CustomerManager agent)
    {

        agent.currentBehavior = CustomerManager.CurrentBehaviour.goToLine;

        agent.animator.SetState(AnimState.Walk);
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.walkToRegisterPos);
        agent.navigation.SetDestination(targetPos);

        if (distanceToTarget < 0.5f)
        {
            agent.SwitchState(agent.standInLineState);
        }

    }
}


