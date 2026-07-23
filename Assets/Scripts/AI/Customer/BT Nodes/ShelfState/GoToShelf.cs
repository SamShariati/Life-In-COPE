using UnityEngine;
using UnityEngine.AI;

public class GoToShelf : BTNode
{
    float distanceTarget;
    public override NodeState Evaluate(CustomerManager agent)
    {
        agent.currentBehavior = CustomerManager.CurrentBehaviour.goToShelf;

        agent.animator.SetState(AnimState.Walk);

        agent.navigation.speed = agent.walkSpeed;
        agent.navigation.isStopped = false;
        agent.navigation.SetDestination(agent.chosenShelfPosition);

        distanceTarget = Vector3.Distance(agent.transform.position, agent.chosenShelfPosition);
        Debug.Log(distanceTarget);
        Debug.Log("AI Position: " + agent.transform.position);
        Debug.Log("chosenShelfPosition: " + agent.chosenShelfPosition);

        if (distanceTarget < 0.2f)
        {
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
