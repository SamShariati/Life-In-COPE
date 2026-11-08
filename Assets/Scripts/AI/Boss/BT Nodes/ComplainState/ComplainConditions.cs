using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class ComplainConditions : BossBTNode
{

    public override NodeState Evaluate(BossManager agent)
    {

        if (StateConditions(agent))
        {
            if (!ChooseComplainType(agent))
            {
                return NodeState.FAILURE;
            }

            agent.BB.Complain.stateActivated = true;
            agent.BB.StoreNav.reachedPlayerDestination = false;
            agent.navigation.speed = agent.walkSpeed;

            return NodeState.SUCCESS;
        }
        else if (agent.BB.Complain.stateActivated)
        {

            return NodeState.SUCCESS;
        }
        else
        {
            return NodeState.FAILURE;
        }

    }

    private bool StateConditions(BossManager agent)
    {

        if (agent.BB.Complain.stateAllowed && !agent.BB.Complain.stateActivated)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool ChooseComplainType(BossManager agent)
    {

        if (agent.BB.Complain.complainedAboutTrash && agent.BB.Complain.complainedAboutShelves && agent.BB.Complain.complainedAboutBossHits
            && agent.BB.Complain.complainedAboutCustomerHits && agent.BB.Complain.complainedAboutRegister)
        {
            agent.BB.Complain.complainedAboutTrash = false;
            agent.BB.Complain.complainedAboutShelves = false;
            agent.BB.Complain.complainedAboutBossHits = false;
            agent.BB.Complain.complainedAboutCustomerHits = false;
            agent.BB.Complain.complainedAboutRegister = false;
        }

        List<string> typeList = GetAvailableComplainTypes(agent);
        int randNr = 0;

        if (typeList.Count != 0)
        {
            randNr = Random.Range(0, typeList.Count);
            agent.BB.Complain.complainTypeChosen = typeList[randNr];
            return true;
        }
        else
        {
            return false;
        }

    }

    private List<string> GetAvailableComplainTypes(BossManager agent)
    {

        List<string> list = new List<string>();

        if (BossDataTracker.Instance.nrTrashOnGround >= agent.trashOnFloorComplainLimit && !agent.BB.Complain.complainedAboutTrash)
        {
            agent.BB.Complain.complainedAboutTrash = true;
            list.Add("trash");
        }
        if (BossDataTracker.Instance.nrEmptyShelvesLeft > 0 && !agent.BB.Complain.complainedAboutShelves)
        {
            agent.BB.Complain.complainedAboutShelves = true;
            list.Add("shelf");
        }
        if (BossDataTracker.Instance.nrTimesBossGotHit >= agent.hitBossComplainLimit && !agent.BB.Complain.complainedAboutBossHits)
        {
            agent.BB.Complain.complainedAboutBossHits = true;
            list.Add("hitBoss");
        }
        if (BossDataTracker.Instance.nrTimesCustomersGotHit >= agent.hitCustomersComplainLimit && !agent.BB.Complain.complainedAboutCustomerHits)
        {
            agent.BB.Complain.complainedAboutCustomerHits = true;
            list.Add("hitCustomers");
        }
        if (BossDataTracker.Instance.nrCustomersInLine >= agent.customersInLineComplainLimit && !agent.BB.Complain.complainedAboutRegister)
        {
            agent.BB.Complain.complainedAboutRegister = true;
            list.Add("register");
        }
        return list;
    }
}
