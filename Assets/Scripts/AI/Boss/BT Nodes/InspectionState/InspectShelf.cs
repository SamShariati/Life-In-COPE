using UnityEngine;

public class InspectShelf : BossBTNode
{

    public enum Phase { Instantiate, WalkToShelf, Inspect, ResultAnimation, Exit }
    public Phase phase = Phase.Instantiate;
    private Vector3 destination;
    private Vector3 shelfPosition;
    private float distanceToTarget;

    public override NodeState Evaluate(BossManager agent)
    {

        agent.currentBehavior = BossManager.CurrentBehaviour.InspectShelf;
        switch (phase)
        {
            case Phase.Instantiate:

                destination = agent.BB.Inspection.allShelfArrowPositions[agent.BB.Inspection.chosenShelf];
                shelfPosition = agent.BB.Inspection.allShelfPositions[agent.BB.Inspection.chosenShelf];
                agent.Functions.SetTimer(4f);

                phase = Phase.WalkToShelf;

                return NodeState.RUNNING;


            case Phase.WalkToShelf:


                agent.navigation.isStopped = false;
                agent.navigation.speed = agent.walkSpeed;
                agent.animator.SetState(AnimState.Walk);
                agent.navigation.SetDestination(destination);
                distanceToTarget = Vector3.Distance(agent.transform.position, destination);

                if (distanceToTarget < 0.2f)
                {
                    phase = Phase.Inspect;
                }

                return NodeState.RUNNING;


            case Phase.Inspect:

                agent.navigation.isStopped = true;
                agent.Functions.RotateTowardsTarget(shelfPosition);


                agent.animator.SetState(AnimState.Inspect);

                if (agent.Functions.TickTimer(Time.deltaTime))
                {
                    phase = Phase.ResultAnimation;
                }

                return NodeState.RUNNING;



            case Phase.ResultAnimation:

                if (agent.BB.Inspection.chosenShelf.shelfFullyStocked)
                {
                    agent.animator.SetState(AnimState.Pleased);

                    if (agent.animator.IsStateFinished("Pleased"))
                    {
                        phase = Phase.Exit;
                    }
                }
                else
                {
                    agent.animator.SetState(AnimState.Dissappoint);

                    if (agent.animator.IsStateFinished("Dissappoint"))
                    {
                        phase = Phase.Exit;
                    }
                }

                return NodeState.RUNNING;

            

            case Phase.Exit:

                agent.BB.Inspection.stateActivated = false;
                agent.BB.Inspection.isCurrInspectingShelf = false;
                agent.BB.Inspection.shelfToInspectChosen = false;
                phase = Phase.Instantiate;

                return NodeState.SUCCESS;

        }
        return NodeState.RUNNING;

    }
}
