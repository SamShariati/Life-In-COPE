using System.Collections.Generic;
using UnityEngine;

public class BossBlackboard
{

    public InspectionData Inspection = new InspectionData();

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
        //public bool inspectRegLineChosen = false;

    }
}
