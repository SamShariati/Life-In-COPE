using UnityEngine;
using UnityEngine.AI;

public class CustomerManager : MonoBehaviour
{
    //-------------------FSM STATES-------------------------

    private FSMBaseState currentState;
    EnterStoreState enterStoreState = new EnterStoreState();

    //------------------------------------------------------

    [SerializeField] private GameObject spawnAgentPos;
    [SerializeField] private GameObject walkIntoStorePos;
    [SerializeField] private NavMeshAgent navigation;


    private void Awake()
    {
        navigation = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        currentState = enterStoreState;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ConstructFSM()
    {
        
    }
    private void ConstructBT()
    {

    }
}
