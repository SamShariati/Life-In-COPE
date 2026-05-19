using UnityEngine;
public class EnterStoreState : FSMBaseState
{
    Vector3 storePosition;
    float distanceToTarget;

    float idleTime;

    public override void EnterState(CustomerManager agent)
    {
        storePosition = agent.enterStorePos.transform.position;
        idleTime = Random.Range(agent.minIdleTime, agent.maxIdleTime);
    }

    public override void UpdateState(CustomerManager agent)
    {
        

        distanceToTarget = Vector3.Distance(agent.transform.position, storePosition);

        if (distanceToTarget < 0.5f)
        {
            agent.animator.SetState(AnimState.Idle);
            idleTime -= Time.deltaTime;

            if (idleTime < 0)
            {
                agent.SwitchState(agent.nothingState);
                agent.BTActivated = true;
            }
        }
        else
        {
            agent.animator.SetState(AnimState.Walk);
        }

        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;
        agent.navigation.SetDestination(storePosition);






    }
}

