using UnityEngine;
public class EnterStoreState : CustomerFSMBaseState
{
    Vector3 storePosition;
    float distanceToTarget;
    float idleTime;

    public override void EnterState(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.enterStore;

        agent.AI.FSMActivated = true;

        storePosition = agent.BB.Store.enterStorePos;
        idleTime = Random.Range(agent.minIdleTime, agent.maxIdleTime);

        agent.BB.IKRigging.IKRiggingEnabled = true;
        
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
            agent.BB.Idle.idleStateAllowed = true;
            agent.AI.FSMActivated = false;
            agent.C_Functions.ChooseShelfRoute(agent);
            agent.SwitchState(agent.AI.FSM.nothingState);
            agent.AI.BTActivated = true;


        }
        // In case agent needs to enter GetHitState when FSM is activated.
        else if (distanceToTarget > 0.5f && agent.BB.GetHit.gotHitByBox)
        {
            agent.AI.BTActivated = true;
        }

    }
}

