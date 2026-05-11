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

        if (distanceToTarget < 0.1f)
        {

            idleTime -= Time.deltaTime;

            if (idleTime < 0)
            {
                agent.SwitchState(agent.nothingState);
                agent.BTActivated = true;
            }
        }

        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;
        agent.navigation.SetDestination(storePosition);






    }
}

