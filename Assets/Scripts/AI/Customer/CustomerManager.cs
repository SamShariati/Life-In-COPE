using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CustomerManager : MonoBehaviour
{

    //-------------DEBUGGING VARIABLES-----------------------------

    public enum CurrentBehaviour { nothing, goToShelfConditions, goToShelf, pickGoodsConditions, pickGoods, searchConditions,
    chasePlayerConditions, chasePlayer, followPlayerConditions, followPlayer, idleStareConditions, idleStare, goToLine, patroleAisle,
    patroleAisleConditions, checkWrongShelfConditions, checkWrongShelf, getHitConditions, fallBackwardConditions, fallBackward,
    fallForwardConditions, fallForward}

    public CurrentBehaviour currentBehavior = CurrentBehaviour.nothing;


    private BTNode rootNode;
    [HideInInspector] public NavMeshAgent navigation;
    [HideInInspector] public InitiateAllComponents initiateAllComponents;
    public CustomerFunctions C_Functions;
    [HideInInspector] public CashRegister cashRegister;
    [HideInInspector] public Transform player;
    [HideInInspector] public PlayerMovement playerMovement;
    [HideInInspector] public CustomerAnimator animator;

    //-------------------FSM STATES-------------------------

    [HideInInspector] public bool BTActivated = false;
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
    [HideInInspector] public Dictionary<string, Vector3> allShelfArrowPositions = new Dictionary<string, Vector3>();
    [HideInInspector] public Dictionary<string, Shelf> allShelfPositions = new Dictionary<string, Shelf>();
    public List<CardboardBoxData> remainingGoodsList = new List<CardboardBoxData>();
    public List<GameObject> goodsGathered = new List<GameObject>(); //används i CashRegister
    [HideInInspector] public Vector3 chosenShelfPosition;
    
    [HideInInspector] public bool shelfRouteChosen = false;
    [HideInInspector] public bool shelfRouteReached = false;
    [HideInInspector] public bool currentlyPickingGoods = false;

    //-------------CASH REGISTER VARIABLES-----------------------------

    [HideInInspector] public int assignedQueueSlot = -1;
    [HideInInspector] public Vector3 currentQueuePos;
    [HideInInspector] public bool transactionComplete = false;


    //---------------FSM VARIABLES------------------------------------
    [HideInInspector] public Vector3 spawnAgentPos;
    [HideInInspector] public Vector3 enterStorePos;
    [HideInInspector] public Vector3 walkToRegisterPos;
    [HideInInspector] public Vector3 exitStorePos;


    //-------------SEARCH FOR PLAYER VARIABLES-----------------------------

    [HideInInspector] public Transform headObject;
    [HideInInspector] public CustomerVision customerVision;
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private LayerMask playerMask;
    [HideInInspector] public bool allowedToChase = true;
    [HideInInspector] public bool spottedPlayer = false;
    [HideInInspector] public bool isCurrentlyChasing = false;
    [HideInInspector] public bool isCurrentlyFollowing = false;
    [HideInInspector] public bool isCurrentlyStaring = false;

    //-------------CONFUSED VARIABLES-----------------------------

    [HideInInspector] public PatroleAisle patroleAisle = new PatroleAisle();
    [HideInInspector] public CheckWrongShelf checkWrongShelf = new CheckWrongShelf();

    [HideInInspector] public bool confusedStateAllowed = true;
    [HideInInspector] public bool confusedStateActivated = false;
    [HideInInspector] public bool isCurrentlyPatrolling = false;
    [HideInInspector] public bool patroleRouteChosen = false;

    [HideInInspector] public bool isCurrCheckingWrongShelf = false;
    [HideInInspector] public bool wrongShelfChosen = false;

    [HideInInspector] public int aisleID = 0;
    [HideInInspector] public Dictionary<int, List<Transform>> allAislePositions = new Dictionary<int, List<Transform>>();
    [HideInInspector] public Vector3 wrongShelfArrowPos;
    [HideInInspector] public Shelf currentWrongShelf;
    [HideInInspector] public Vector3 chosenAislePos;


    //-------------GET HIT VARIABLES-----------------------------


    public bool getHitStateAllowed = true;
    public bool getHitStateActivated = false;
    public bool gotHitByBox = false;
    public Rigidbody collidingBoxRB;
    public bool isCurrFallingForward = false;
    public bool isCurrFallingBackward = false;
    public float dotProduct;
    public float forceRatio;
    public float targetRotationAngle;


    [Header("Customer Stats")]
    public float walkSpeed;
    public float runSpeed = 5f;
    public int nrGoodsNeeded = 2;
    public float maxIdleTime = 5f;
    public float minIdleTime = 2;
    public float wasteCustomerTime = 1.5f;
    public float gettingStunnedTime = 4f;
    public float searchForPlayerStateCD = 10;
    public float getHitStateCD = 0;
    

    [Header("Behaviour Chance %")]
    public float confusedChance = 100f;
    public float chasePlayerChance = 100f;


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
        initiateAllComponents = new InitiateAllComponents(this);
        C_Functions = new CustomerFunctions(this);
        animator = GetComponent<CustomerAnimator>();

    }
    void Start()
    {
        initiateAllComponents.GenerateAllComponents();
        customerVision = new CustomerVision(headObject, player, obstacleMask, playerMask); //Behöver ändras, dålig arkitektur placering
        //nrGoodsNeeded = Random.Range(1, 6);
        
        currentState = enterStoreState;
        currentState.EnterState(this);
        ConstructBT();
    }

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

    }

    public void SwitchState(FSMBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }

    // Agent got hit by a box
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GoodsBox") && BTActivated)
        {

            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();

            float impactSpeedThreshold = 2f; //testing number
            Vector3 boxVelocity = rb.linearVelocity;
            boxVelocity.y = 0f;

            if (boxVelocity.magnitude < impactSpeedThreshold)
            {
                return; // box is essentially stationary — not a throw, ignore it
            }
            gotHitByBox = true;
            collidingBoxRB = other.gameObject.GetComponent<Rigidbody>();

        }
    }

    private void ConstructBT()
    {

        //------GetHitState scripts ------

        GetHitConditions getHitConditions = new GetHitConditions();
        FallBackwardConditions fallBackwardConditions = new FallBackwardConditions();
        FallBackward fallBackward = new FallBackward();
        FallForwardConditions fallForwardConditions = new FallForwardConditions();
        FallForward fallForward = new FallForward();

        //------SearchForPlayerState scripts ------
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
        PatroleAisleConditions patroleAisleConditions = new PatroleAisleConditions();
        //PatroleAisle patroleAisle = new PatroleAisle(); - PUBLIC
        CheckWrongShelfConditions checkWrongShelfConditions = new CheckWrongShelfConditions();
        //CheckWrongShelf checkWrongShelf = new CheckWrongShelf(); - PUBLIC


        //----------------------------------------------------------------------------------------------------------------------

        //GETHIT STATE BRANCH

        Sequence fallBackwardState = new Sequence(new List<BTNode>() { fallBackwardConditions, fallBackward });
        Sequence fallForwardState = new Sequence(new List<BTNode>() { fallForwardConditions, fallForward });

        Selector fallingDirectionState = new Selector(new List<BTNode> { fallBackwardState, fallForwardState });
        Sequence getHitState = new Sequence(new List<BTNode>() { getHitConditions, fallingDirectionState });


        //SEARCHFORPLAYER STATE BRANCH

        Sequence chasePlayerState = new Sequence(new List<BTNode>() { chasePlayerConditions, chasePlayer });
        Sequence followPlayerState = new Sequence(new List<BTNode>() { followPlayerConditions, followPlayer });
        Sequence idleStareState = new Sequence(new List<BTNode>() { idleStareConditions, idleStare });

        Selector playerSpottedState = new Selector(new List<BTNode> { chasePlayerState, followPlayerState, idleStareState });
        Sequence searchForPlayerState = new Sequence(new List<BTNode>() { searchConditions, playerSpottedState });


        //CONFUSED STATE BRANCH

        Sequence patroleAisleState = new Sequence(new List<BTNode>() { patroleAisleConditions, patroleAisle });
        Sequence checkWrongShelfState = new Sequence(new List<BTNode>() { checkWrongShelfConditions, checkWrongShelf});

        Selector confusedTypeState = new Selector(new List<BTNode> { patroleAisleState, checkWrongShelfState});
        Sequence confusedState = new Sequence(new List<BTNode>() { confusedConditions, confusedTypeState});


        //SHELF STATE BRANCH
        Sequence goToShelfState = new Sequence(new List<BTNode>() { goToShelfConditions, goToShelf });
        Sequence pickGoodsState = new Sequence(new List<BTNode>() { pickGoodsConditions, pickGoods });


        Selector shelfBehaviour = new Selector(new List<BTNode> { goToShelfState, pickGoodsState });
        Sequence shelfState = new Sequence(new List<BTNode>() { shelfStateConditions, shelfBehaviour });



        rootNode = new Selector(new List<BTNode> { getHitState, searchForPlayerState, confusedState, shelfState});
    }

    private void OnDrawGizmosSelected() //used for checking CustomerVision raycasts
    {
        if (customerVision == null || !customerVision.drawDebugGizmos)
            return;

        customerVision.DrawGizmos();
    }
}
