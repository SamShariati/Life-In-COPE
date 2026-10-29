using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class CustomerBlackboard
{

    public GetHitData GetHit = new GetHitData();
    public IdleData Idle = new IdleData();
    public SabotageData Sabotage = new SabotageData();
    public SearchForPlayerData SFP = new SearchForPlayerData();
    public ConfusedData Confused = new ConfusedData();
    public GrabShelfItemData GSI = new GrabShelfItemData();
    public StorePositionData Store = new StorePositionData();
    public CashRegisterData CashRegister = new CashRegisterData();
    public IKRiggingData IKRigging = new IKRiggingData();


    public class GetHitData
    {
        public bool getHitStateAllowed = true;
        public bool getHitStateActivated = false;
        public bool gotHitByBox = false;
        public Rigidbody collidingBoxRB;
        public bool isCurrFallingForward = false;
        public bool isCurrFallingBackward = false;
        public float dotProduct;
        public float forceRatio;
        public float targetRotationAngle;
        public Vector3 thrownDirection;
        public CardboardBoxObject thrownBox;
        public Quaternion impactTargetRotation;
    }

    public class IdleData
    {
        public bool idleStateAllowed = true;
        public bool idleStateActivated = false;
    }

    public class SabotageData
    {
        public bool sabotageStateAllowed = true;
        public bool sabotageStateDisabledByHit = false;
        public bool sabotageStateActivated = false;
        public bool recentlyHitFood = false;
        public bool isCurrentlyGoingToStall = false;
        public bool isCurrentlyHittingStall = false;
        public bool isCurrentlyTaunting = false;
        public bool seenPlayerDuringSabotage = false;
        public Transform chosenFoodStallPos;
        public FoodStall chosenFoodStall;
        public List<Transform> foodStallList = new List<Transform>();
    }

    public class SearchForPlayerData
    {
        public Transform headObject;
        public CustomerVision customerVision;
        public LayerMask obstacleMask;
        public LayerMask playerMask;
        public bool currentlyTouchingPlayer = false;
        public bool allowedToChase = true;
        public bool spottedPlayer = false;
        public bool isCurrentlyChasing = false;
        public bool isCurrentlyFollowing = false;
        public bool isCurrentlyStaring = false;
    }

    public class ConfusedData
    {
        public bool confusedStateAllowed = true;
        public bool confusedStateActivated = false;
        public bool isCurrentlyPatrolling = false;
        public bool patroleRouteChosen = false;
        public bool isCurrCheckingWrongShelf = false;
        public bool wrongShelfChosen = false;
        public int aisleID = 0;
        public Dictionary<int, List<Transform>> allAislePositions = new Dictionary<int, List<Transform>>();
        public Vector3 wrongShelfArrowPos;
        public Shelf currentWrongShelf;
        public Vector3 chosenAislePos;
    }

    public class GrabShelfItemData
    {
        public bool shelfStateAllowed = true;
        public CardboardBoxData currentChosenGood;
        public Dictionary<string, Vector3> allShelfArrowPositions = new Dictionary<string, Vector3>();
        public Dictionary<string, Shelf> allShelfPositions = new Dictionary<string, Shelf>();
        public List<CardboardBoxData> remainingGoodsList = new List<CardboardBoxData>();
        public List<GameObject> goodsGathered = new List<GameObject>(); //används i CashRegister
        public Vector3 chosenShelfPosition;
        public bool shelfRouteReached = false;
        public bool currentlyPickingGoods = false;
    }

    public class StorePositionData
    {
        [HideInInspector] public Vector3 spawnAgentPos;
        [HideInInspector] public Vector3 enterStorePos;
        [HideInInspector] public Vector3 walkToRegisterPos;
        [HideInInspector] public Vector3 exitStorePos;
    }

    public class CashRegisterData
    {
        [HideInInspector] public int assignedQueueSlot = -1;
        [HideInInspector] public Vector3 currentQueuePos;
        [HideInInspector] public bool transactionComplete = false;
    }

    public class IKRiggingData
    {

        public MultiAimConstraint chestRig;
        public MultiAimConstraint headRig;
        public bool IKRiggingEnabled = true;
        public Transform IKAimTarget;
        public Transform cameraTransform;
    }
}
