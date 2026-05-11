using UnityEngine;

public class IdleState : FSMBaseState
{
    float idleTime;
    public override void EnterState(CustomerManager agent)
    {
        idleTime = Random.Range(agent.minIdleTime, agent.maxIdleTime);
        //Debug.Log("Entered IdleState");
    }

    public override void UpdateState(CustomerManager agent)
    {
        agent.navigation.isStopped = true;
        idleTime -= Time.deltaTime;

        if (idleTime < 0)
        {
            agent.SwitchState(agent.nothingState);
            agent.BTActivated = true;
            //Debug.Log("Finished IdleState");
        }


    }
}
