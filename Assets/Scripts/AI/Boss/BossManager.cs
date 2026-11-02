using UnityEngine;
using UnityEngine.AI;

public class BossManager : MonoBehaviour
{

    public enum CurrentBehaviour
    {
        Nothing, InspectShelf, 
    }

    public CurrentBehaviour currentBehavior = CurrentBehaviour.Nothing;

    [HideInInspector] public BossBlackboard BB = new BossBlackboard();
    [HideInInspector] public BossAIBuilder AI = new BossAIBuilder();
    [HideInInspector] public BossDataTracker dataTracker = new BossDataTracker();
    [HideInInspector] public BossFunctions Functions;
    //[HideInInspector] public BossAIBuilder AI;
    public NavMeshAgent navigation;
    [HideInInspector] InitiateBossComponents initiateComponents;
    [HideInInspector] public BossAnimator animator;


    [Header("Boss Stats")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;


    private void Awake()
    {
        initiateComponents = new InitiateBossComponents(this);
        navigation = GetComponent<NavMeshAgent>();
        animator = GetComponent<BossAnimator>();
        Functions = new BossFunctions(this);
    }

    private void Start()
    {
        initiateComponents.GenerateAllComponents();
        AI.BT.ConstructBT();
    }

    void Update()
    {
        if (AI.BTActivated) // BT
        {
            AI.BT.rootNode.Evaluate(this);

        }
    }



}
