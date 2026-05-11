using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class CustomerManager : MonoBehaviour
{
    private BTNode rootNode;

    //-------------------FSM STATES-------------------------

    private FSMBaseState currentState;
    EnterStoreState enterStoreState = new EnterStoreState();
    [HideInInspector] public NothingState nothingState = new NothingState();
    [HideInInspector] public IdleState idleState = new IdleState();


    //------------------------------------------------------

    [HideInInspector] public NavMeshAgent navigation;
    [HideInInspector] public List<CardboardBoxData> remainingGoodsList = new List<CardboardBoxData>();
    [HideInInspector] public Dictionary<CardboardBoxData, Vector3> goodsShelfPairs = new Dictionary<CardboardBoxData, Vector3>();
    [HideInInspector] public Vector3 chosenShelfPosition;


    [Header("Objects Needed")]
    [HideInInspector] public GameObject spawnAgentPos;
    public GameObject enterStorePos;
    [SerializeField] private GameObject shelfObjectParent;
    [SerializeField] private GameObject palletObject;
    [SerializeField] private List<CardboardBoxData> palletDataList;
    


    [Header("Customer Stats")]
    public float walkSpeed;
    public float runSpeed;
    private int nrGoodsNeeded = 3;
    public float maxIdleTime = 5f;
    public float minIdleTime = 2;

    [Header("Booleans")]

    [HideInInspector] public bool BTActivated = false;
    [HideInInspector] public bool shelfRouteChosen = false;

    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
        spawnAgentPos = GameObject.Find("spawnAgentPos");
        enterStorePos = GameObject.Find("enterStorePos");
    }
    void Start()
    {
        GenerateSpecificGoods();
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

    

    private void GenerateSpecificGoods()
    {
        shelfObjectParent = GameObject.Find("Shelfs");
        palletObject = GameObject.Find("KolonialPallet");

        palletDataList = palletObject.GetComponent<KolonialPallet>().boxDataList;

        for (int i = 0; i < nrGoodsNeeded; i++)
        {
            int rand = Random.Range(0, palletDataList.Count);
            remainingGoodsList.Add(palletDataList[rand]);
            palletDataList.RemoveAt(rand);
        }
        GetShelfPositions();
    }

    private void GetShelfPositions()
    {
        Shelf[] shelves = shelfObjectParent.GetComponentsInChildren<Shelf>();

        foreach (CardboardBoxData box in remainingGoodsList)
        {
            foreach (Shelf shelf in shelves)
            {
                string shelfGoodsType = shelf.goodsType.ToString();

                if (shelfGoodsType == box.boxID)
                {
                    Transform shelfArrow = shelf.transform.Find("shelfArrow");
                    goodsShelfPairs[box] = shelfArrow.position;
                }
            }
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

        //SHELF BRANCH
        Sequence goToShelfState = new Sequence(new List<BTNode>() { goToShelfConditions, goToShelf });
        Selector shelfState = new Selector(new List<BTNode> { goToShelfState });

        rootNode = new Selector(new List<BTNode> { shelfState });
    }
}
