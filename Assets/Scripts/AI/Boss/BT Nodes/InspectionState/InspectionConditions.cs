using UnityEngine;

public class InspectionConditions : BossBTNode
{
    public override NodeState Evaluate(BossManager agent)
    {
        throw new System.NotImplementedException();
    }

    private void RollConfusedType(CustomerManager agent)
    {
        float randPercent = Random.Range(0, 100);

        if (randPercent < 66.7f)
        {
            agent.BB.Confused.isCurrentlyPatrolling = true;
        }
        else
        {
            agent.BB.Confused.isCurrCheckingWrongShelf = true;
        }

    }
}
