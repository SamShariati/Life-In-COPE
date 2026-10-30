using System.Collections.Generic;
using UnityEngine;

public class BossBlackboard : MonoBehaviour
{

    public InspectionData Inspection = new InspectionData();

    public class InspectionData
    {

        public bool stateAllowed = true;
        public bool stateActivated = false;
        public Dictionary<Shelf, Vector3> allShelfArrowPositions = new Dictionary<Shelf, Vector3>();
        public Dictionary<Shelf, Vector3> allShelfPositions = new Dictionary<Shelf, Vector3>();
        public Dictionary<Shelf, bool> shelvesFullyStockedStatus = new Dictionary<Shelf, bool>();
        public bool inspectShelfChosen = false;
        public bool inspectTrashChosen = false;
        public bool inspectRegLineChosen = false;

    }
}
