using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;


public class CustomerManager : MonoBehaviour
{

    //-------------DEBUGGING VARIABLES-----------------------------

    public enum CurrentBehaviour { nothing, goToShelf, pickGoods, 
    chasePlayer, followPlayer, idleStare, goToLine, patroleAisle,
    checkWrongShelfConditions, checkWrongShelf, fallBackward,
    fallForward, idle, enterStore, goToStall, hitFood, tauntPlayer}

    public CurrentBehaviour currentBehavior = CurrentBehaviour.nothing;

    [HideInInspector] public CustomerBlackboard BB = new CustomerBlackboard();
    [HideInInspector] public CustomerAIBuilder AI = new CustomerAIBuilder();

    [HideInInspector] public NavMeshAgent navigation;
    [HideInInspector] public InitiateCustomerComponents initiateComponents;
    public CustomerFunctions C_Functions;
    [HideInInspector] public CashRegister cashRegister;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public CustomerAnimator animator;
    [HideInInspector] CustomerIKRigging IKRigging;

    [Header("Customer Stats")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public int nrGoodsNeeded = 2;
    public float minIdleTime = 2;
    public float maxIdleTime = 5f;
    public float viewDistanceRange = 10f;
    public float FOVAngleRange = 45f;
    public float playerAwernessRange = 10f;
    public float wasteCustomerTime = 1.5f;
    public float gettingStunnedTime = 4f;
    public float searchForPlayerStateCD = 10;
    //public float getHitStateCD = 0;
    public float sabotageStateCD = 0;
    
    [Header("Behaviour Chance %")]
    public float confusedChance = 100f;
    public float chasePlayerChance = 100f;
    public float sabotageChance = 100f;


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();

        initiateComponents = new InitiateCustomerComponents(this);
        C_Functions = new CustomerFunctions(this);
        IKRigging = new CustomerIKRigging(this);
        animator = GetComponent<CustomerAnimator>();

    }
    void Start()
    {
        initiateComponents.GenerateAllComponents();
        BB.SFP.customerVision = new CustomerVision(this); //Behöver ändras, dålig arkitektur placering
        //nrGoodsNeeded = Random.Range(1, 6);
        
        AI.FSM.currentState = AI.FSM.enterStoreState;
        AI.FSM.currentState.EnterState(this);
        AI.BT.ConstructBT();
    }

    void Update()
    {

        IKRigging.UpdateIK();

        if (AI.BTActivated) // BT
        {
            AI.BT.rootNode.Evaluate(this);
            
        }
        else //FSM
        {
            AI.FSM.currentState.UpdateState(this);
        }
        
    }

    public void SwitchState(FSMBaseState state)
    {
        AI.FSM.currentState = state;
        state.EnterState(this);
    }

    // Agent got hit by a box
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoodsBox"))
        {
            var box = other.GetComponentInParent<CardboardBoxObject>();
            if (box == null || !box.IsInFlight) return;

            BB.GetHit.gotHitByBox = true;
            BB.GetHit.thrownBox = box;                 // store the box, not just its RB
            BB.GetHit.thrownDirection = box.ThrownDirection;
        }
        else if (other.CompareTag("Player"))
        {
            BB.SFP.currentlyTouchingPlayer = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BB.SFP.currentlyTouchingPlayer = false;
        }
    }

    private void OnDrawGizmosSelected() //used for checking CustomerVision raycasts
    {
        if (BB.SFP.customerVision == null || !BB.SFP.customerVision.drawDebugGizmos)
            return;

        BB.SFP.customerVision.DrawGizmos();
    }
}
