using UnityEngine;
using UnityEngine.AI;

public class GoToShelf : BTNode
{
    bool runOnce = false;
    float distanceTarget;
    public override NodeState Evaluate(CustomerManager agent)
    {
        if (!runOnce)
        {
            runOnce = true;
            Debug.Log("Entered GoToShelf");
        }
        agent.animator.SetState(AnimState.Walk);

        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;
        agent.navigation.SetDestination(agent.chosenShelfPosition);

        distanceTarget = Vector3.Distance(agent.transform.position, agent.chosenShelfPosition);
        
        if (distanceTarget < 0.2f)
        {
            //RemoveItemFromGoodsList(agent);
            agent.shelfRouteReached = true;

            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
        
    }

    private void RemoveItemFromGoodsList(CustomerManager agent)
    {

        agent.remainingGoodsList.Remove(agent.currentChosenGood);

        //foreach (CardboardBoxData goods in agent.remainingGoodsList)
        //{
        //    agent.remainingGoodsList.Remove(agent.currentChosenGood);
        //    if (goods == agent.currentChosenGood)
        //    {
        //        agent.remainingGoodsList.Remove(agent.currentChosenGood);
        //    }
        //}
    }
}
