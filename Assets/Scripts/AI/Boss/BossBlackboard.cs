using System.Collections.Generic;
using UnityEngine;

public class BossBlackboard
{

    public InspectionData Inspection = new InspectionData();
    public StoreData Store = new StoreData();

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

    public class StoreData
    {
        public Transform inspectTrashPos;
        public Transform inspectTrashRotPos;
        public Transform inspectRegisterPos;
        public Transform inspectRegisterRotPos;
    }
}
