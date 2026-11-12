using JetBrains.Annotations;
using UnityEngine;

public class GoToPlayer : BossBTNode
{
    float distanceToTarget;

    public override NodeState Evaluate(BossManager agent)
    {

        agent.Functions.SetPlayerDestination();
        ChooseAnimation(agent);
        distanceToTarget = Vector3.Distance(agent.transform.position, agent.player.position);

        if (distanceToTarget < 1.5f)
        {
            string chosenText = BossDialogueData.Instance.GetComplainDialogue(agent.BB.Complain.complainTypeChosen);
            //string chosenText = "Hejhej mitt namn är sam och jag är en riktig alfahane";
            agent.Speech.Speak(chosenText, 10f);
            agent.navigation.isStopped = true;
            agent.BB.StoreNav.reachedPlayerDestination = true;
            return NodeState.SUCCESS;
        }

        return NodeState.RUNNING;
    }

    private void ChooseAnimation(BossManager agent)
    {
        if (agent.navigation.speed == agent.walkSpeed)
        {
            agent.animator.SetState(AnimState.Walk);
        }
        else if (agent.navigation.speed == agent.runSpeed)
        {
            //animation
        }
        else if (agent.navigation.speed == agent.sprintSpeed)
        {
            //animation
        }
    }
}
