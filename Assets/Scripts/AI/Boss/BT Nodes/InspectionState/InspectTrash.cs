using UnityEngine;

public class InspectTrash : BossBTNode
{
    public enum Phase { Instantiate, WalkToTrash, Inspect, ResultAnimation, Exit }
    public Phase phase = Phase.Instantiate;

    private Vector3 destination;
    private float distanceToTarget;

    public override NodeState Evaluate(BossManager agent)
    {
        agent.currentBehavior = BossManager.CurrentBehaviour.InspectTrash;

        switch (phase)
        {

            case Phase.Instantiate:

                destination = agent.BB.Inspection.chosenTrash.position;
                agent.Functions.SetTimer(3f);

                phase = Phase.WalkToTrash;

                return NodeState.RUNNING;


            case Phase.WalkToTrash:

                if (agent.BB.Inspection.chosenTrash == null)
                {
                    agent.Functions.ChooseTrashToInspect();
                    destination = agent.BB.Inspection.chosenTrash.position;
                }

                agent.navigation.isStopped = false;
                agent.navigation.speed = agent.walkSpeed;
                agent.navigation.SetDestination(destination);
                agent.animator.SetState(AnimState.Walk);
                distanceToTarget = Vector3.Distance(agent.transform.position, destination);
                
                if (distanceToTarget < 1.5f)
                {
                    phase = Phase.Inspect;
                }

                return NodeState.RUNNING;


            case Phase.Inspect:

                agent.navigation.isStopped = true;
                agent.animator.SetState(AnimState.Inspect);

                if (agent.BB.Inspection.chosenTrash != null)
                {
                    agent.Functions.RotateTowardsTarget(agent.BB.Inspection.chosenTrash.position);
                }

                if (agent.Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.ResultAnimation;
                }

                return NodeState.RUNNING;


            case Phase.ResultAnimation:

                if (destination == agent.BB.Store.inspectTrashPos.position)
                {
                    agent.animator.SetState(AnimState.Pleased);
                    agent.Functions.RotateTowardsTarget(agent.BB.Store.inspectTrashRotPos.position);
                }
                else
                {
                    agent.animator.SetState(AnimState.Dissappoint);

                    if (agent.BB.Inspection.chosenTrash != null)
                    {
                        agent.Functions.RotateTowardsTarget(agent.BB.Inspection.chosenTrash.position);
                    }
                }

                if (agent.animator.IsStateFinished("Pleased") || agent.animator.IsStateFinished("Dissappoint"))
                {
                    phase = Phase.Exit;
                }

                return NodeState.RUNNING;


            case Phase.Exit:

                agent.BB.Inspection.isCurrInspectingTrash = false;
                agent.BB.Inspection.trashToInspectChosen = false;
                agent.BB.Inspection.stateActivated = false;
                phase = Phase.Instantiate;

                return NodeState.SUCCESS;


        }

        return NodeState.RUNNING;
    }
}
