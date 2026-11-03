using UnityEngine;
using System.Collections.Generic;

public class BossAIBuilder
{

    public bool BTActivated = true;
    public bool FSMActivated = false;

    public BossFiniteStateMachine FSM = new BossFiniteStateMachine();
    public BossBehaviourTree BT = new BossBehaviourTree();


}

public class BossFiniteStateMachine
{

}

public class BossBehaviourTree
{
    public BossBTNode rootNode;

    //----INSPECTION SCRIPTS------
    
    InspectionConditions inspectionConditions = new InspectionConditions();
    InspectShelfConditions inspectShelfConditions = new InspectShelfConditions();
    public InspectShelf inspectShelf = new InspectShelf();
    InspectTrashConditions inspectTrashConditions = new InspectTrashConditions();
    public InspectTrash inspectTrash = new InspectTrash();
    InspectRegisterConditions inspectRegisterConditions = new InspectRegisterConditions();
    public InspectRegister inspectRegister = new InspectRegister();



    public void ConstructBT()
    {

        //---INSPECTION BRANCH------
        BossSequence inspectShelfState = new BossSequence(new List<BossBTNode>() { inspectShelfConditions, inspectShelf });
        BossSequence inspectTrashState = new BossSequence(new List<BossBTNode>() { inspectTrashConditions, inspectTrash });
        BossSequence inspectRegisterState = new BossSequence(new List<BossBTNode>() { inspectRegisterConditions, inspectRegister });

        BossSelector inspectionBehaviour = new BossSelector(new List<BossBTNode>() { inspectShelfState, inspectTrashState, inspectRegisterState });
        BossSequence inspection = new BossSequence(new List<BossBTNode>() { inspectionConditions, inspectionBehaviour });

        rootNode = new BossSequence(new List<BossBTNode>() { inspection });
    }
}
