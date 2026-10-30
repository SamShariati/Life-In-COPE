using UnityEngine;

public class FirstInLineState : CustomerFSMBaseState
{
    float distanceToTarget;
    private enum Phase { IdleStanding, ThankAnimation, Exit}
    private Phase phase = Phase.IdleStanding;
    private bool thankStarted = false;


    public override void EnterState(CustomerManager agent)
    {
        //agent.C_Functions.SetTimer(7); //Detta blir patienceTimer sen.
        agent.BB.IKRigging.IKRiggingEnabled = true;

        agent.cashRegister.customerFirstInLine = agent; //Dålig arkitektur. Alla agenter kommer åt register, men register håller bara en
        agent.cashRegister.itemsLeftToScan = agent.BB.GSI.goodsGathered.Count;
        agent.cashRegister.itemsToScanList = agent.BB.GSI.goodsGathered;
        agent.cashRegister.PlaceGoodsOnRegister();
        agent.cashRegister.StartCustomerGoodsScan();
        agent.animator.SetState(AnimState.GrabItem);
    }
    public override void UpdateState(CustomerManager agent)
    {

        switch (phase)
        {

            case Phase.IdleStanding:

                distanceToTarget = Vector3.Distance(agent.BB.CashRegister.currentQueuePos, agent.transform.position);
                agent.animator.SetState(AnimState.Idle);
                agent.navigation.SetDestination(agent.BB.CashRegister.currentQueuePos);

                if (distanceToTarget < 0.2f)
                {
                    RotateTowardsRegister(agent);
                }

                if (agent.BB.CashRegister.transactionComplete)
                {
                    phase = Phase.ThankAnimation;            
                }

                break;

            case Phase.ThankAnimation:

                agent.C_Functions.RotateTowardsPlayer();

                if (!thankStarted)
                {
                    agent.animator.SetState(AnimState.Thank);
                    thankStarted = true;
                    break;
                }

                if (agent.animator.IsStateFinished("Thank"))
                {
                    phase = Phase.Exit;
                }
                break;

            case Phase.Exit:


                QueueManager.Instance.AdvanceQueue();
                agent.SwitchState(agent.AI.FSM.exitStoreState);

                break;

        }
        if (agent.BB.GetHit.gotHitByBox)
        {
            agent.AI.BTActivated = true;
        }


    }


    private void RotateTowardsRegister(CustomerManager agent)
    {
        Vector3 direction = (agent.cashRegister.interactColliderPos - agent.transform.position).normalized;

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, lookRotation, Time.deltaTime * (agent.navigation.angularSpeed / 60));
    }
}

