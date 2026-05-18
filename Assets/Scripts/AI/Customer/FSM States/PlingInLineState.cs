using UnityEngine;

public class PlingInLineState : FSMBaseState
{


    public override void EnterState(CustomerManager agent)
    {
        agent.C_Functions.SetTimer(7); //Detta blir patienceTimer sen.
    }
    public override void UpdateState(CustomerManager agent)
    {
        agent.navigation.SetDestination(agent.currentQueuePos);
        if (agent.C_Functions.TickTimer(Time.deltaTime))
        {
            QueueManager.Instance.AdvanceQueue();
            agent.SwitchState(agent.enterStoreState);
        }


    }
}
