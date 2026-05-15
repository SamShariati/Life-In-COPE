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

    //-------------------FSM STATES-------------------------

    private FSMBaseState currentState;
    EnterStoreState enterStoreState = new EnterStoreState();
    [HideInInspector] public NothingState nothingState = new NothingState();
    [HideInInspector] public IdleState idleState = new IdleState();

    //-------------------SHELF BRANCH VARIABLES------------------------

    [HideInInspector] public CardboardBoxData currentChosenGood;
    [HideInInspector] public Dictionary<CardboardBoxData, Vector3> shelfPosPairs = new Dictionary<CardboardBoxData, Vector3>();
    [HideInInspector] public Dictionary<string, Shelf> shelfIDPairs = new Dictionary<string, Shelf>();
    public List<CardboardBoxData> remainingGoodsList = new List<CardboardBoxData>();
    [HideInInspector] public Vector3 chosenShelfPosition;
    public int nrGoodsFound = 0;
    [HideInInspector] public bool BTActivated = false;
    [HideInInspector] public bool shelfRouteChosen = false;
    [HideInInspector] public bool shelfRouteReached = false;
    [HideInInspector] public bool currentlyPickingGoods = false;

    //-----------------------------------------------------------------

    [Header("Objects Needed")]
    [HideInInspector] public GameObject spawnAgentPos;
    [HideInInspector] public GameObject enterStorePos;
    

    [Header("Customer Stats")]
    public float walkSpeed;
    public float runSpeed;
    public int nrGoodsNeeded = 3;
    public float maxIdleTime = 5f;
    public float minIdleTime = 2;


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
        spawnAgentPos = GameObject.Find("spawnAgentPos");
        enterStorePos = GameObject.Find("enterStorePos");
        C_Functions = new CustomerFunctions(this);
    }
    void Start()
    {
        C_Functions.GenerateSpecificGoods();
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
