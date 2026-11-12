using UnityEngine;
using UnityEngine.AI;

public class BossManager : MonoBehaviour
{

    public enum CurrentBehaviour
    {
        Nothing, InspectShelf, InspectTrash 
    }

    public CurrentBehaviour currentBehavior = CurrentBehaviour.Nothing;

    [HideInInspector] public BossBlackboard BB = new BossBlackboard();
    [HideInInspector] public BossAIBuilder AI = new BossAIBuilder();
    [HideInInspector] public BossFunctions Functions;
    [HideInInspector] public NavMeshAgent navigation;
    [HideInInspector] InitiateBossComponents initiateComponents;
    [HideInInspector] public BossAnimator animator;
    [HideInInspector] public Transform player;
    [HideInInspector] public BossSpeechBubble Speech;
 

    [Header("Boss Stats")]
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float sprintSpeed = 10f;
    public int customersInLineComplainLimit = 2;
    public int firstInLinePlingRageLimit = 100;
    public int trashOnFloorComplainLimit = 1;
    public int trashOnFloorWarnLimit = 8;
    public int trashOnFloorRageLimit = 12;
    public int hitCustomersComplainLimit = 2;
    public int hitCustomersWarnLimit = 10;
    public int hitCustomersRageLimit = 14;
    public int hitBossComplainLimit = 1;
    public int hitBossWarnLimit = 2;
    public int hitBossRageLimit = 3;



    private void Awake()
    {
        initiateComponents = new InitiateBossComponents(this);
        navigation = GetComponent<NavMeshAgent>();
        animator = GetComponent<BossAnimator>();
        Functions = new BossFunctions(this);
        Speech = GetComponent<BossSpeechBubble>();
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
