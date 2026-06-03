using UnityEngine;

public class PlingInLineState : FSMBaseState
{

    public override void EnterState(CustomerManager agent)
    {
        agent.C_Functions.SetTimer(7); //Detta blir patienceTimer sen.
        agent.cashRegister.customerFirstInLine = agent; //kan behöva ändras
        agent.cashRegister.itemsLeftToScan = agent.goodsGathered.Count;
        agent.cashRegister.itemsToScanList = agent.goodsGathered;
    }
    public override void UpdateState(CustomerManager agent)
    {
        agent.animator.SetState(AnimState.Idle);
        agent.navigation.SetDestination(agent.currentQueuePos);
        if (agent.C_Functions.TickTimer(Time.deltaTime))
        {
            QueueManager.Instance.AdvanceQueue();
            agent.SwitchState(agent.enterStoreState);
        }


    }
}
