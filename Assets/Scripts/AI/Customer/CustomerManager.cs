using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CustomerManager : MonoBehaviour
{

    //-------------DEBUGGING VARIABLES-----------------------------

    public enum CurrentBehaviour { nothing, goToShelfConditions, goToShelf, goToShelf2, pickGoodsConditions, pickGoods, searchConditions,
    chasePlayerConditions, chasePlayer, followPlayerConditions, followPlayer, idleStareConditions, idleStare, goToLine}

    public CurrentBehaviour currentBehavior = CurrentBehaviour.nothing;


    private BTNode rootNode;
    [HideInInspector] public NavMeshAgent navigation;
    public CustomerFunctions C_Functions;
    [HideInInspector] public CashRegister cashRegister;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerMovement playerMovement;

    //-------------------FSM STATES-------------------------

    private FSMBaseState currentState;
    [HideInInspector] public EnterStoreState enterStoreState = new EnterStoreState();
    [HideInInspector] public NothingState nothingState = new NothingState();
    [HideInInspector] public IdleState idleState = new IdleState();
    [HideInInspector] public StandInLineState standInLineState = new StandInLineState();
    [HideInInspector] public GoToLineState goToLineState = new GoToLineState();
    [HideInInspector] public FirstInLineState firstInLineState = new FirstInLineState();
    [HideInInspector] public ExitStoreState exitStoreState = new ExitStoreState();

    //-------------------SHELF BRANCH VARIABLES------------------------

    [HideInInspector] public bool shelfStateAllowed = true;

    public CardboardBoxData currentChosenGood;
    [HideInInspector] public Dictionary<CardboardBoxData, Vector3> shelfPosPairs = new Dictionary<CardboardBoxData, Vector3>();
    [HideInInspector] public Dictionary<string, Shelf> shelfIDPairs = new Dictionary<string, Shelf>();
    public List<CardboardBoxData> remainingGoodsList = new List<CardboardBoxData>();
    public List<GameObject> goodsGathered = new List<GameObject>(); //används i CashRegister
    [HideInInspector] public Vector3 chosenShelfPosition;
    //public int nrGoodsFound = 0;
    [HideInInspector] public bool BTActivated = false;
    [HideInInspector] public bool shelfRouteChosen = false;
    [HideInInspector] public bool shelfRouteReached = false;
    [HideInInspector] public bool currentlyPickingGoods = false;

    //-------------CASH REGISTER VARIABLES-----------------------------

    public int assignedQueueSlot = -1;
    [HideInInspector] public Vector3 currentQueuePos;
    [HideInInspector] public bool transactionComplete = false;

    //-----------------------------------------------------------------


    [Header("Objects Needed")]
    [HideInInspector] public Vector3 spawnAgentPos;
    [HideInInspector] public Vector3 enterStorePos;
    [HideInInspector] public Vector3 walkToRegisterPos;
    [HideInInspector] public Vector3 exitStorePos;
    [HideInInspector] public CustomerAnimator animator;

    //-------------SEARCH FOR PLAYER VARIABLES-----------------------------

    public Transform headObject;
    public CustomerVision customerVision;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playerMask;
    [HideInInspector] public bool allowedToChase = true;
    [HideInInspector] public bool playerSpotted = false;
    [HideInInspector] public bool isCurrentlyChasing = false;
    [HideInInspector] public bool isCurrentlyFollowing = false;
    [HideInInspector] public bool isCurrentlyStaring = false;

    

    



    [Header("Customer Stats")]
    public float walkSpeed;
    public float runSpeed = 5f;
    public int nrGoodsNeeded = 2;
    public float maxIdleTime = 5f;
    public float minIdleTime = 2;
    public float chaseCooldown = 10;
    public float WasteCustomerTime = 1.5f;


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
        C_Functions = new CustomerFunctions(this);
        animator = GetComponent<CustomerAnimator>();

    }
    void Start()
    {
        C_Functions.GenerateAllComponents();
        customerVision = new CustomerVision(headObject, player, obstacleMask, playerMask); //Behöver ändras, dålig arkitektur placering
        //nrGoodsNeeded = Random.Range(1, 6);
        
        currentState = enterStoreState;
        currentState.EnterState(this);
        ConstructBT();
    }

    // Update is called once per frame
    void Update()
    {
        if (BTActivated) // BT
        {
            rootNode.Evaluate(this);
        }
        else //FSM
        {
            currentState.UpdateState(this);
        }

        //customerVision.CanSeePlayer();
    }

    public void SwitchState(FSMBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
    private void ConstructBT()
    {
        
        //------SearchForPlayer scripts ------
        SearchConditions searchConditions = new SearchConditions();
        ChasePlayerConditions chasePlayerConditions = new ChasePlayerConditions();
        ChasePlayer chasePlayer = new ChasePlayer();
        FollowPlayerConditions followPlayerConditions = new FollowPlayerConditions();
        FollowPlayer followPlayer = new FollowPlayer();
        IdleStareConditions idleStareConditions = new IdleStareConditions();
        IdleStare idleStare = new IdleStare();
        //------ShelfState scripts ------
        ShelfStateConditions shelfStateConditions = new ShelfStateConditions();
        GoToShelfConditions goToShelfConditions = new GoToShelfConditions();
        GoToShelf goToShelf = new GoToShelf();
        PickGoodsConditions pickGoodsConditions = new PickGoodsConditions();
        PickGoods pickGoods = new PickGoods();
        //------ConfusedState scripts ------
        ConfusedConditions confusedConditions = new ConfusedConditions();
        GoToAisleConditions goToSectionConditions = new GoToAisleConditions();
        GoToAisle goToSection = new GoToAisle();
        GoToSpotConditions goToSpotConditions = new GoToSpotConditions();
        GoToSpot goToSpot = new GoToSpot();


        //----------------------------------------------------------------------------------------------------------------------

        //SHELF BRANCH
        Sequence goToShelfState = new Sequence(new List<BTNode>() { goToShelfConditions, goToShelf });
        Sequence pickGoodsState = new Sequence(new List<BTNode>() { pickGoodsConditions, pickGoods });

        Selector shelfBehaviour = new Selector(new List<BTNode> { goToShelfState, pickGoodsState });
        Sequence shelfState = new Sequence(new List<BTNode>() { shelfStateConditions, shelfBehaviour });

        //SEARCHFORPLAYER BRANCH

        Sequence chasePlayerState = new Sequence(new List<BTNode>() { chasePlayerConditions, chasePlayer });
        Sequence followPlayerState = new Sequence(new List<BTNode>() { followPlayerConditions, followPlayer });
        Sequence idleStareState = new Sequence(new List<BTNode>() { idleStareConditions, idleStare });

        Selector playerSpottedState = new Selector(new List<BTNode> { chasePlayerState, followPlayerState, idleStareState});
        Sequence searchForPlayerState = new Sequence(new List<BTNode>() { searchConditions, playerSpottedState});


        rootNode = new Selector(new List<BTNode> { searchForPlayerState, shelfState});
    }

    private void OnDrawGizmosSelected() //used for checking CustomerVision raycasts
    {
        if (customerVision == null || !customerVision.drawDebugGizmos)
            return;

        customerVision.DrawGizmos();
    }
}
