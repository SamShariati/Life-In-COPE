using UnityEngine;
public class EnterStoreState : FSMBaseState
{
    Vector3 storePosition;
    float distanceToTarget;
    float idleTime;

    public override void EnterState(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.enterStore;

        agent.FSMStateActivated = true;

        storePosition = agent.enterStorePos;
        idleTime = Random.Range(agent.minIdleTime, agent.maxIdleTime);

        agent.IKRiggingEnabled = true;
        
    }

    public override void UpdateState(CustomerManager agent)
    {


        agent.animator.SetState(AnimState.Walk);

        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;
        agent.navigation.SetDestination(storePosition);

        distanceToTarget = Vector3.Distance(agent.transform.position, storePosition);

        if (distanceToTarget < 0.5f)
        {
            agent.idleStateAllowed = true;
            agent.FSMStateActivated = false;
            agent.C_Functions.ChooseShelfRoute(agent);
            agent.SwitchState(agent.nothingState);
            agent.BTActivated = true;


        }
        // In case agent needs to enter GetHitState when FSM is activated.
        else if (distanceToTarget > 0.5f && agent.gotHitByBox)
        {
            agent.BTActivated = true;
        }

    }
}

