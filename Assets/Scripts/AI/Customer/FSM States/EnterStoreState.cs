using UnityEngine;
public class EnterStoreState : FSMBaseState
{
    Vector3 storePosition;
    float distanceToTarget;
    bool finishedState = false;
    float timer = 3f;

    public override void EnterState(CustomerManager agent)
    {
        storePosition = agent.enterStorePos.transform.position;
    }

    public override void UpdateState(CustomerManager agent)
    {
        distanceToTarget = Vector3.Distance(agent.transform.position, storePosition);

        if (distanceToTarget < 0.1f)
        {
            timer -= Time.deltaTime;

            if (timer < 0)
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

