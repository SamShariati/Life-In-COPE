using System.Collections.Generic;
using UnityEngine;

public class BossBlackboard
{

    public InspectionData Inspection = new InspectionData();
    public StoreNavigationData StoreNav = new StoreNavigationData();
    public ComplainData Complain = new ComplainData();

    public class InspectionData
    {

        public bool stateAllowed = true;
        public bool stateActivated = false;
        public Dictionary<Shelf, Vector3> allShelfArrowPositions = new Dictionary<Shelf, Vector3>();
        public Dictionary<Shelf, Vector3> allShelfPositions = new Dictionary<Shelf, Vector3>();
        public Dictionary<Shelf, bool> shelvesFullyStockedStatus = new Dictionary<Shelf, bool>();
        public Shelf chosenShelf;

        public bool shelfToInspectChosen = false;
        public bool isCurrInspectingShelf = false;
        public bool trashToInspectChosen = false;
        public bool isCurrInspectingTrash = false;
        public bool isCurrInspectingRegister = false;
        public Transform chosenTrash;

        //public bool inspectRegLineChosen = false;

        public GameObject trashParent;
    }

    public class StoreNavigationData
    {
        public Transform inspectTrashPos;
        public Transform inspectTrashRotPos;
        public Transform inspectRegisterPos;
        public Transform inspectRegisterRotPos;
        public Vector3 chosenTarget;
        public bool reachedTargetDestination = true;
    }

    public class ComplainData
    {
        public bool stateAllowed = true;
        public bool stateActivated = false;
        public string complainTypeChosen;

        public bool complainedAboutTrash = false;
        public bool complainedAboutShelves = false;
        public bool complainedAboutRegister = false;
        public bool complainedAboutCustomerHits = false;
        public bool complainedAboutBossHits = false;
    }
    private List<string> GetAvailableComplainTypes(BossManager agent)
    {

        List<string> list = new List<string>();

        if (BossDataTracker.Instance.nrTrashOnGround >= agent.trashOnFloorComplainLimit)
        {
            list.Add("trash");
        }
        if (BossDataTracker.Instance.nrEmptyShelvesLeft > 0)
        {
            list.Add("shelf");
        }
        if (BossDataTracker.Instance.nrTimesBossGotHit >= agent.hitBossComplainLimit)
        {
            list.Add("hitBoss");
        }
        if (BossDataTracker.Instance.nrTimesCustomersGotHit >= agent.hitCustomersComplainLimit)
        {
            list.Add("hitCustomers");
        }
        if (BossDataTracker.Instance.nrCustomersInLine >= agent.customersInLineComplainLimit)
        {
            list.Add("register");
        }
        return list;
    }
}
