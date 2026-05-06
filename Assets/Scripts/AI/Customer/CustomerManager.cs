using UnityEngine;
using UnityEngine.AI;

public class CustomerManager : MonoBehaviour
{
    //-------------------FSM STATES-------------------------

    private FSMBaseState currentState;
    EnterStoreState enterStoreState = new EnterStoreState();
    [HideInInspector] public NothingState nothingState = new NothingState();

    //------------------------------------------------------

    [HideInInspector] public NavMeshAgent navigation;

    [Header("Objects Needed")]
    [HideInInspector] public GameObject spawnAgentPos;
     public GameObject enterStorePos;


    [Header("Customer Stats")]
    public float walkSpeed;
    public float runSpeed;

    [Header("Booleans")]

    [HideInInspector] public bool BTActivated = false;


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
        spawnAgentPos = GameObject.Find("spawnAgentPos");
        enterStorePos = GameObject.Find("enterStorePos");
    }
    void Start()
    {

        currentState = enterStoreState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (BTActivated) // BT
        {
            //BTträd
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

    }
}
