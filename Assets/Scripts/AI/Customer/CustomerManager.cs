using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CustomerManager : MonoBehaviour
{
    private BTNode rootNode;
    [HideInInspector] public NavMeshAgent navigation;
    public CustomerFunctions C_Functions;
    [HideInInspector] public CashRegister cashRegister;

    //-------------------FSM STATES-------------------------

    private FSMBaseState currentState;
    [HideInInspector] public EnterStoreState enterStoreState = new EnterStoreState();
    [HideInInspector] public NothingState nothingState = new NothingState();
    [HideInInspector] public IdleState idleState = new IdleState();
    [HideInInspector] public StandInLineState standInLineState = new StandInLineState();
    [HideInInspector] public GoToLineState goToLineState = new GoToLineState();
    [HideInInspector] public PlingInLineState plingInLineState = new PlingInLineState();

    //-------------------SHELF BRANCH VARIABLES------------------------

    [HideInInspector] public CardboardBoxData currentChosenGood;
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


    //-----------------------------------------------------------------

    [Header("Objects Needed")]
    [HideInInspector] public GameObject spawnAgentPos;
    [HideInInspector] public GameObject enterStorePos;
    [HideInInspector] public Vector3 walkToRegisterPos;
    [HideInInspector] public CustomerAnimator animator;




    [Header("Customer Stats")]
    public float walkSpeed;
    public float runSpeed;
    public int nrGoodsNeeded = 2;
    public float maxIdleTime = 5f;
    public float minIdleTime = 2;


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
        C_Functions = new CustomerFunctions(this);
        C_Functions.GenerateNavPositions();
        animator = GetComponent<CustomerAnimator>();

    }
    void Start()
    {
        nrGoodsNeeded = Random.Range(1, 6);
        C_Functions.GenerateSpecificGoods();
        C_Functions.GetCashRegister();
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
    }

    public void SwitchState(FSMBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
    private void ConstructBT()
    {
        GoToShelfConditions goToShelfConditions = new GoToShelfConditions();
        GoToShelf goToShelf = new GoToShelf();
        PickGoodsConditions pickGoodsConditions = new PickGoodsConditions();
        PickGoods pickGoods = new PickGoods();

        //SHELF BRANCH
        Sequence goToShelfState = new Sequence(new List<BTNode>() { goToShelfConditions, goToShelf });
        Sequence pickGoodsState = new Sequence(new List<BTNode>() { pickGoodsConditions, pickGoods });

        Selector shelfState = new Selector(new List<BTNode> { goToShelfState, pickGoodsState });

        rootNode = new Selector(new List<BTNode> { shelfState});
    }
}
