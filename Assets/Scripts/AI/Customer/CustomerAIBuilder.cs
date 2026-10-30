using System.Collections.Generic;
using UnityEngine;

public class CustomerAIBuilder
{
    public bool BTActivated = false;
    public bool FSMActivated = false;

    public CustomerFiniteStateMachine FSM = new CustomerFiniteStateMachine();
    public CustomerBehaviourTree BT = new CustomerBehaviourTree();

    public class CustomerFiniteStateMachine
    {
        public CustomerFSMBaseState currentState;

        public EnterStoreState enterStoreState = new EnterStoreState();
        public NothingState nothingState = new NothingState();
        public StandInLineState standInLineState = new StandInLineState();
        public GoToLineState goToLineState = new GoToLineState();
        public FirstInLineState firstInLineState = new FirstInLineState();
        public ExitStoreState exitStoreState = new ExitStoreState();

        public void SwitchState(CustomerFSMBaseState state, CustomerManager agent)
        {
            currentState = state;
            state.EnterState(agent);
        }
    }

    public class CustomerBehaviourTree
    {

        public CustomerBTNode rootNode;

        //------Idle scripts-------------

        IdleConditions idleConditions = new IdleConditions();
        Idle idle = new Idle();

        //------GetHitState scripts ------

        GetHitConditions getHitConditions = new GetHitConditions();
        FallBackwardConditions fallBackwardConditions = new FallBackwardConditions();
        public FallBackward fallBackward = new FallBackward();
        FallForwardConditions fallForwardConditions = new FallForwardConditions();
        public FallForward fallForward = new FallForward();

        //------SabotageState scripts -------

        SabotageConditions sabotageConditions = new SabotageConditions();
        GoToStallConditions goToStallConditions = new GoToStallConditions();
        GoToStall goToStall = new GoToStall();
        HitFoodConditions hitFoodConditions = new HitFoodConditions();
        public HitFood hitFood = new HitFood();
        TauntPlayerConditions tauntPlayerConditions = new TauntPlayerConditions();
        public TauntPlayer tauntPlayer = new TauntPlayer();


        //------SearchForPlayerState scripts ------

        SearchConditions searchConditions = new SearchConditions();
        ChasePlayerConditions chasePlayerConditions = new ChasePlayerConditions();
        ChasePlayer chasePlayer = new ChasePlayer();
        FollowPlayerConditions followPlayerConditions = new FollowPlayerConditions();
        FollowPlayer followPlayer = new FollowPlayer();
        IdleStareConditions idleStareConditions = new IdleStareConditions();
        public IdleStare idleStare = new IdleStare();


        //------ConfusedState scripts ------

        ConfusedConditions confusedConditions = new ConfusedConditions();
        PatroleAisleConditions patroleAisleConditions = new PatroleAisleConditions();
        public PatroleAisle patroleAisle = new PatroleAisle();
        CheckWrongShelfConditions checkWrongShelfConditions = new CheckWrongShelfConditions();
        public CheckWrongShelf checkWrongShelf = new CheckWrongShelf();


        //------ShelfState scripts ------

        GrabItemConditions grabItemConditions = new GrabItemConditions();
        GoToShelfConditions goToShelfConditions = new GoToShelfConditions();
        GoToShelf goToShelf = new GoToShelf();
        PickGoodsConditions pickGoodsConditions = new PickGoodsConditions();
        public PickGoods pickGoods = new PickGoods();

        public void ConstructBT()
        {

            //IDLE STATE BRANCH

            CustomerSequence standIdle = new CustomerSequence(new List<CustomerBTNode>() { idleConditions, idle });

            //GETHIT STATE BRANCH

            CustomerSequence fallBackwardState = new CustomerSequence(new List<CustomerBTNode>() { fallBackwardConditions, fallBackward });
            CustomerSequence fallForwardState = new CustomerSequence(new List<CustomerBTNode>() { fallForwardConditions, fallForward });

            CustomerSelector fallingDirectionBehaviour = new CustomerSelector(new List<CustomerBTNode> { fallBackwardState, fallForwardState });
            CustomerSequence getHit = new CustomerSequence(new List<CustomerBTNode>() { getHitConditions, fallingDirectionBehaviour });


            //SABOTAGE BRANCH

            CustomerSequence goToStallState = new CustomerSequence(new List<CustomerBTNode>() { goToStallConditions, goToStall });
            CustomerSequence hitFoodState = new CustomerSequence(new List<CustomerBTNode>() { hitFoodConditions, hitFood });
            CustomerSequence tauntPlayerState = new CustomerSequence(new List<CustomerBTNode>() { tauntPlayerConditions, tauntPlayer });

            CustomerSelector sabotageBehaviour = new CustomerSelector(new List<CustomerBTNode>() { goToStallState, hitFoodState, tauntPlayerState });
            CustomerSequence sabotage = new CustomerSequence(new List<CustomerBTNode>() { sabotageConditions, sabotageBehaviour });


            //SEARCHFORPLAYER STATE BRANCH

            CustomerSequence chasePlayerState = new CustomerSequence(new List<CustomerBTNode>() { chasePlayerConditions, chasePlayer });
            CustomerSequence followPlayerState = new CustomerSequence(new List<CustomerBTNode>() { followPlayerConditions, followPlayer });
            CustomerSequence idleStareState = new CustomerSequence(new List<CustomerBTNode>() { idleStareConditions, idleStare });

            CustomerSelector playerSpottedBehaviour = new CustomerSelector(new List<CustomerBTNode> { chasePlayerState, followPlayerState, idleStareState });
            CustomerSequence searchForPlayer = new CustomerSequence(new List<CustomerBTNode>() { searchConditions, playerSpottedBehaviour });


            //CONFUSED STATE BRANCH

            CustomerSequence patroleAisleState = new CustomerSequence(new List<CustomerBTNode>() { patroleAisleConditions, patroleAisle });
            CustomerSequence checkWrongShelfState = new CustomerSequence(new List<CustomerBTNode>() { checkWrongShelfConditions, checkWrongShelf });

            CustomerSelector confusedBehaviour = new CustomerSelector(new List<CustomerBTNode> { patroleAisleState, checkWrongShelfState });
            CustomerSequence confused = new CustomerSequence(new List<CustomerBTNode>() { confusedConditions, confusedBehaviour });


            //SHELF STATE BRANCH
            CustomerSequence goToShelfState = new CustomerSequence(new List<CustomerBTNode>() { goToShelfConditions, goToShelf });
            CustomerSequence pickGoodsState = new CustomerSequence(new List<CustomerBTNode>() { pickGoodsConditions, pickGoods });


            CustomerSelector grabItemBehaviour = new CustomerSelector(new List<CustomerBTNode> { goToShelfState, pickGoodsState });
            CustomerSequence grabItem = new CustomerSequence(new List<CustomerBTNode>() { grabItemConditions, grabItemBehaviour });



            rootNode = new CustomerSelector(new List<CustomerBTNode> { standIdle, getHit, sabotage, searchForPlayer, confused, grabItem });
        }

    }
}
