using UnityEngine;

public class PlingInLineState : FSMBaseState
{
    float distanceToTarget;
    public override void EnterState(CustomerManager agent)
    {
        agent.C_Functions.SetTimer(7); //Detta blir patienceTimer sen.
        agent.cashRegister.customerFirstInLine = agent; //kan behöva ändras
        agent.cashRegister.itemsLeftToScan = agent.goodsGathered.Count;
        agent.cashRegister.itemsToScanList = agent.goodsGathered;
        //agent.cashRegister.PlaceGoodsOnRegister();
    }
    public override void UpdateState(CustomerManager agent)
    {
        distanceToTarget = Vector3.Distance(agent.currentQueuePos, agent.transform.position);
        agent.animator.SetState(AnimState.Idle);
        agent.navigation.SetDestination(agent.currentQueuePos);

        if (distanceToTarget < 0.2f)
        {
            RotateTowardsRegister(agent);
        }

        if (agent.C_Functions.TickTimer(Time.deltaTime))
        {
            QueueManager.Instance.AdvanceQueue();
            agent.SwitchState(agent.exitStoreState);
        }
    }


    private void RotateTowardsRegister(CustomerManager agent)
    {
        Vector3 direction = (agent.cashRegister.registerPos - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed / 60));
    }
}
